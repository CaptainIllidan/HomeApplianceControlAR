using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HomeApplianceControl.Domain.BO
{
    public class DeviceData
    {
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
        public DeviceData(string deviceId, byte[] imageData, float widthInMeters)
        {
            DeviceId = deviceId;
            ImageData = imageData;
            WidthInMeters = widthInMeters;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual string Id { get; set; }

        [ForeignKey("Device")]
        public virtual string DeviceId { get; set; }
        public virtual byte[] ImageData { get; set; }

        public virtual float WidthInMeters { get; set; }
    }
}
