using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HomeApplianceControl.Contracts.ElectroluxApi
{
    [DataContract]
    public class CommandResult
    {
        [DataMember(Name="commandId")]
        public string CommandId { get; set; }

        [DataMember(Name = "result")]
        public bool Result { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }
    }

}
