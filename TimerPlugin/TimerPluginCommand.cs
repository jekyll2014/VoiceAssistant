﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginInterface;

namespace TimerPlugin
{
    public class TimerPluginCommand : PluginCommand
    {
        public string Response = "";
        public bool isStopCommand = false;
    }
}
