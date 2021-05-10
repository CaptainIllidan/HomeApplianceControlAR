using System;
using System.Collections.Generic;
using System.Text;
using HomeApplianceControl.Domain.BO;

namespace HomeApplianceControl.Common.DeviceCommands
{
    public static class CommandsHelper
    {
        private static Dictionary<DeviceType, IEnumerable<Command>> commandsDictionary
            = new Dictionary<DeviceType, IEnumerable<Command>>
            {
                [DeviceType.Refrigerator] = new[]
                    { Command.SetTemperature, Command.GetState, Command.SetFreezerTemperature },
                [DeviceType.WaterHeater] = new[]
                    { Command.GetState, Command.SetTemperature, Command.TurnOff, Command.TurnOn },
            };

        public static IEnumerable<Command> GetAvailableCommands(Device device)
        {
            return commandsDictionary.ContainsKey(device.DeviceType)
                ? commandsDictionary[device.DeviceType]
                : new Command[] { };
        }
    }
}
