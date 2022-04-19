// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using PluginInterface;

namespace RunProgramPlugin
{
    public class RunProgramPlugin : PluginBase
    {
        private readonly RunProgramPluginCommand[] RunProgramCommands;
        private readonly Dictionary<string, Process> _processes = new Dictionary<string, Process>();
        private readonly string _canNotClose;
        private readonly string _notRunning;
        private readonly string _canNotRun;
        private readonly string _alreadyRunning;

        public RunProgramPlugin(IAudioOutSingleton audioOut, string currentCulture, string pluginPath) : base(audioOut, currentCulture, pluginPath)
        {
            var configBuilder = new Config<RunProgramPluginSettings>($"{PluginPath}\\{PluginConfigFile}");

            if (!File.Exists($"{PluginPath}\\{PluginConfigFile}"))
            {
                configBuilder.SaveConfig();
            }

            RunProgramCommands = configBuilder.ConfigStorage.Commands;

            if (RunProgramCommands is PluginCommand[] newCmds)
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
            var command = RunProgramCommands.FirstOrDefault(n => n.Name == commandName);

            if (command == null)
            {
                return;
            }

            var response = string.Empty;
            var processId = command.CommandLine;

            if (command.IsStopCommand)
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
                    var process = RunCommand(command.CommandLine);

                    if (process != null)
                    {
                        if (!command.AllowMultiple)
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

        private Process RunCommand(string command)
        {
            Process proc;
            try
            {
                proc = Process.Start(command);
            }
            catch
            {
                try
                {
                    // hack because of this: https://github.com/dotnet/corefx/issues/10361
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        command = command.Replace("&", "^&");
                        proc = Process.Start(new ProcessStartInfo(command) { UseShellExecute = true });

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
                        proc = Process.Start("xdg-open", command);
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        proc = Process.Start("open", command);
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
