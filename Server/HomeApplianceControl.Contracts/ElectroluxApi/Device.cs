using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HomeApplianceControl.Contracts.ElectroluxApi
{
    [DataContract(Name="device")]
    public class Device
    {
        [DataMember(Name="tempid")]
        public string TempId { get; set; }

        [DataMember(Name = "state")]
        public int State { get; set; }

        [DataMember(Name = "current_temp")]
        public string CurrentTemp { get; set; }

        [DataMember(Name = "temp_goal")]
        public string TempGoal { get; set; }
    }
}
