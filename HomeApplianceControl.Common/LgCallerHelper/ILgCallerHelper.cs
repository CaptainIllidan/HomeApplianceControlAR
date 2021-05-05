using System;
using System.Collections.Generic;
using System.Text;
using HomeApplianceControl.Common.DeviceCommands;
using HomeApplianceControl.Domain.BO;

namespace HomeApplianceControl.Common.LgCallerHelper
{
    public interface ILgCallerHelper
    {
        void ExecuteCommand(Device device, Command command, string args);

        IDictionary<string, string> GetState(Device device);
    }
}
