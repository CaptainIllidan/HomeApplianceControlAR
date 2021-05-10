using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HomeApplianceControl.Contracts
{
    [DataContract]
    public class TemperatureDevice : Device
    {
        public int Temperature { get; set; }
    }
}
