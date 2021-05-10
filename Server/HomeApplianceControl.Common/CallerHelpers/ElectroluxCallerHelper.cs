using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HomeApplianceControl.Common.DeviceCommands;
using HomeApplianceControl.Common.Settings;
using HomeApplianceControl.Domain.BO;
using log4net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;

namespace HomeApplianceControl.Common.CallerHelpers
{
    public class ElectroluxCallerHelper : IElectroluxCallerHelper
    {
        private readonly string ElectroluxPhpApiCaller;
        private readonly Regex stateRegex = new Regex(@"(""result"":{""device"":)(\[{.+}\])(,""invalid"":).+\}{");
        private readonly bool UseLocal;
        private readonly string ApiUrl;
        private readonly JsonSerializer jsonSerializer;

        private readonly ILogger logger = HomeApplianceControlLogManager.GetLogger<ElectroluxCallerHelper>();

        public ElectroluxCallerHelper(IOptions<ElectroluxSettings> electroluxSettings)
        {
            ElectroluxPhpApiCaller = $"{System.IO.Path.GetFullPath(@"..\..\..")}/{electroluxSettings.Value.ThirdPartyPath}";
            ApiUrl = electroluxSettings.Value.ApiUrl;
            UseLocal = electroluxSettings.Value.UseLocal;
        }

        private string RunCommand(string cmd)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "php";
            start.Arguments = string.Format("{0} {1}", ElectroluxPhpApiCaller, cmd);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            var result = string.Empty;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    Task.Run(() => {
                        try
                        {
                            result = reader?.ReadToEnd();
                        }
                        catch (ObjectDisposedException) { }
                    }).Wait(5000);
                }
            }

            logger.LogDebug($"{typeof(ElectroluxCallerHelper)}: {cmd} - {result}");

            return result;
        }

        public void ExecuteCommand(Device device, Command command, string args)
        {
            var availableCommands = CommandsHelper.GetAvailableCommands(device);
            if (!availableCommands.Contains(command))
                throw new ArgumentException($"Unsupported command {command} for device {device.Id} {device.DeviceType}");

            switch (command)
            {
                case Command.SetTemperature:
                    SetTemperature(device, args);
                    break;
                case Command.TurnOn:
                    TurnOn(device);
                    break;
                case Command.TurnOff:
                    TurnOff(device);
                    break;
                default:
                    throw new ArgumentException($"Cannot execute unknown command {command} for device {device.Id}");
            }
        }

        private void SetTemperature(Device device, string temperature)
        {
            GetState(device);
            RunCommand($"update-device {device.Id} {{\\\"temp_goal\\\":\\\"{temperature}\\\"}}");
        }

        private void TurnOn(Device device)
        {
            GetState(device);
            RunCommand($"update-device {device.Id} {{\\\"state\\\":\\\"1\\\"}}");
        }

        private void TurnOff(Device device)
        {
            GetState(device);
            RunCommand($"update-device {device.Id} {{\\\"state\\\":\\\"0\\\"}}");
        }

        public IDictionary<string, string> GetState(Device device)
        {
            var result = RunCommand($"get-devices");
            var lastMatch = stateRegex.Matches(result).LastOrDefault();
            if (lastMatch == null || !lastMatch.Success)
                return new Dictionary<string, string>();

            var devicesJson = lastMatch.Groups[2].Value;
            var devices = JsonConvert.DeserializeObject<Contracts.ElectroluxApi.Device[]>(devicesJson);
            var deviceState = devices.FirstOrDefault(d => d.TempId == device.Id);

            if (deviceState == null)
                return new Dictionary<string, string>();

            return deviceState.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(deviceState, null).ToString());
        }
    }
}
