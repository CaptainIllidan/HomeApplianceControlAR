using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using HomeApplianceControl.Domain.BO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeApplianceControl.Common.DeviceCommands
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Command
    {
        SetTemperature = 0,
        SetFreezerTemperature = 1,
        GetState = 2,
        TurnOn = 3,
        TurnOff = 4,
    }
}
