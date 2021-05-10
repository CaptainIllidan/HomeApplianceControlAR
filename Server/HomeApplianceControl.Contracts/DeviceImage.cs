using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using HomeApplianceControl.Domain.BO;

namespace HomeApplianceControl.Contracts
{
    [DataContract(Name = "deviceImage")]
    public class DeviceImage
    {
        public DeviceImage(string deviceId, byte[] imageData)
        {
            DeviceId = deviceId;
            ImageData = imageData;
        }

        public DeviceImage(DeviceData deviceData)
        {
            DeviceId = deviceData.DeviceId;
            ImageData = deviceData.ImageData;
            WidthInMeters = deviceData.WidthInMeters;
        }
        

        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }
        [DataMember(Name="imageData")]
        public byte[] ImageData { get; set; }
        [DataMember(Name = "widthInMeters")] 
        public float WidthInMeters { get; set; }
    }
}
