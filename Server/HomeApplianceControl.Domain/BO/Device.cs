using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HomeApplianceControl.Domain.BO
{
    public class Device : IDevice
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public Device(string id, DeviceType deviceType, IntegrationType integrationType)
        {
            Id = id;
            DeviceType = deviceType;
            IntegrationType = integrationType;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual string Id { get; set; }

        public virtual DeviceType DeviceType { get; set; }

        public virtual IntegrationType IntegrationType { get; set; }
    }
}
