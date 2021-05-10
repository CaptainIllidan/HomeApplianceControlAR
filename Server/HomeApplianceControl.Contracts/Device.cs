using System;
using System.Runtime.Serialization;
using HomeApplianceControl.Domain.BO;

namespace HomeApplianceControl.Contracts
{
    [DataContract]
    public class Device
    {
        [DataMember]
        public string Id { get; set; }

        public virtual DeviceType DeviceType { get; set; }

        public virtual IntegrationType IntegrationType { get; set; }
    }
}
