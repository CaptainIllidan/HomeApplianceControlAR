using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HomeApplianceControl.Common.DeviceCommands;
using HomeApplianceControl.Domain.BO;

namespace HomeApplianceControl.Common.LgCallerHelper
{
    public class LgCallerHelper : ILgCallerHelper
    {
        private const string WideqPath = @"C:/Repos/wideq/example.py";
        private readonly Regex stateRegex = new Regex(@"- (\w+): (.+)");

        private string RunCommand(string cmd)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:/Python38/python.exe";
            start.Arguments = string.Format("{0} {1}", WideqPath, cmd);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            var result = string.Empty;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }

        public void ExecuteCommand(Device device, Command command, string args)
        {
            var availableCommands = CommandsHelper.GetAvailableCommands(device);
            if (!availableCommands.Contains(command))
                throw new ArgumentException($"Unsupported command {command} for device {device.Id} {device.DeviceType}");

            switch (command)
            {
                case Command.SetFreezerTemperature:
                    SetFreezerTemperature(device, args);
                    break;
                case Command.SetTemperature:
                    SetRefrigeratorTemperature(device, args);
                    break;
                default:
                    throw new ArgumentException($"Cannot execute unknown command {command} for device {device.Id}");
            }
        }

        public IDictionary<string, string> GetState(Device device)
        {
            var result = RunCommand($"mon {device.Id}");
            return result.Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .ToDictionary(
                    s => stateRegex.Match(s).Groups[1].ToString(),
                    s => stateRegex.Match(s).Groups[2].ToString());
        }

        private void SetRefrigeratorTemperature(Device device, string temperature)
        {
            RunCommand($"set-temp {device.Id} {temperature}");
        }

        private void SetFreezerTemperature(Device device, string temperature)
        {
            RunCommand($"set-temp-freezer {device.Id} {temperature}");
        }

    }
}
