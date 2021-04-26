using System;
using System.Runtime.Serialization;

namespace HomeApplianceControl.Contracts
{
    [DataContract]
    public class Device
    {
        [DataMember]
        public Guid Id { get; set; }
    }
}
