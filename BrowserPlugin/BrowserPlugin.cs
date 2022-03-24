using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Win32;

using PluginInterface;

namespace BrowserPlugin
{
    public class BrowserPlugin : PluginBase
    {
        private readonly BrowserPluginCommand[] BrowserCommands;
        private readonly Dictionary<string, Process> _processes = new Dictionary<string, Process>();
        private readonly string _canNotClose;
        private readonly string _notRunning;
        private readonly string _canNotRun;
        private readonly string _alreadyRunning;

        public BrowserPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<BrowserPluginSettings>($"{PluginPath}\\{PluginConfigFile}");

            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            BrowserCommands = configBuilder.ConfigStorage.Commands;

            if (BrowserCommands is PluginCommand[] newCmds)
            {
                _commands = newCmds;
            }

            _canNotClose = configBuilder.ConfigStorage.CanNotClose;
            _notRunning = configBuilder.ConfigStorage.NotRunning;
            _canNotRun = configBuilder.ConfigStorage.CanNotRun;
            _alreadyRunning = configBuilder.ConfigStorage.AlreadyRunning;
        }

        public override void Execute(string commandName, List<Token> commandTokens)
        {
            var command = BrowserCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
            {
                return;
            }

            var response = string.Empty;
            var processId = command.URL;

            if (command.isStopCommand)
            {
                if (_processes.TryGetValue(processId, out var proc))
                {
                    if (proc != null)
                    {
                        try
                        {
                            proc.Kill();
                            response = command.Response;
                        }
                        catch
                        {
                            response = _canNotClose;
                        }
                        finally
                        {
                            _processes.Remove(processId);
                        }
                    }
                    else
                    {
                        response = _notRunning;
                    }
                }
                else
                {
                    response = _notRunning;
                }
            }
            else
            {
                if (!_processes.TryGetValue(processId, out _))
                {
                    var process = OpenUrl(command.URL, command.useStandAloneBrowser);

                    if (process != null)
                    {
                        if (command.useStandAloneBrowser)
                        {
                            _processes.Add(processId, process);
                        }

                        response = command.Response;
                    }
                    else
                    {
                        response = _canNotRun;
                    }
                }
                else
                {
                    response = _alreadyRunning;
                }
            }
            AudioOut.Speak(response);
        }

        private Process OpenUrl(string url, bool standAloneBrowser = false)
        {
            Process proc = null;
            try
            {
                if (standAloneBrowser)
                {
                    string browser = string.Empty;
                    RegistryKey key = null;

                    try
                    {
                        key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command");

                        if (key != null)
                        {
                            // Get default Browser
                            browser = key.GetValue(null).ToString().ToLower().Trim(new[] { '"' });
                        }

                        if (!browser.EndsWith("exe"))
                        {
                            //Remove all after the ".exe"
                            browser = browser.Substring(0, browser.LastIndexOf(".exe", StringComparison.InvariantCultureIgnoreCase) + 4);
                        }
                    }
                    finally
                    {
                        if (key != null)
                        {
                            key.Close();
                        }
                    }

                    proc = Process.Start(browser, url);
                }
                else
                {
                    proc = Process.Start(url);
                }
            }
            catch
            {
                try
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        url = url.Replace("&", "^&");
                        proc = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

                        /*
                        Process myProcess = new Process();
                        try
                        {
                            // true is the default, but it is important not to set it to false
                            myProcess.StartInfo.UseShellExecute = true; 
                            myProcess.StartInfo.FileName = "http://some.domain.tld/bla";
                            myProcess.Start();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        */
                        //Windows.System.Launcher.LaunchUriAsync(new Uri("http://google.com"));
                        //Process.Start("explorer.exe", $"\"{uri}\"");

                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        proc = Process.Start("xdg-open", url);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        proc = Process.Start("open", url);
                    }
                    else
                    {
                        throw;
                    }
                }
                catch
                {
                    return null;
                }
            }

            return proc;
        }
    }
}
