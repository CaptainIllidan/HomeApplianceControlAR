using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HomeApplianceControl.Common;
using HomeApplianceControl.Contracts;
using HomeApplianceControl.Domain.BO;
using HomeApplianceControl.Domain.DAL;
using log4net;
using Microsoft.AspNetCore.Http;
using Device = HomeApplianceControl.Domain.BO.Device;

namespace HomeApplianceControl.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceController : ControllerBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(DeviceController));

        private readonly HomeApplianceControlContext context;

        public DeviceController(HomeApplianceControlContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public Device Get(string id)
        {
            return DeviceHelper.GetDeviceInternal(context, id, DeviceHelper.Assertion.Exist);
        }

        [HttpPost]
        public Device Post(Device device)
        {
            DeviceHelper.GetDeviceInternal(context, device.Id, DeviceHelper.Assertion.NotExist);
            var deviceDal = context.Devices.Add(device)?.Entity;
            context.SaveChanges();
            return deviceDal;
        }

        [HttpPut]
        public Device Put(Device device)
        {
            var deviceDal = DeviceHelper.GetDeviceInternal(context, device.Id, DeviceHelper.Assertion.Exist);
            deviceDal.DeviceType = device.DeviceType;
            deviceDal.IntegrationType = device.IntegrationType;
            context.Devices.Update(deviceDal);
            context.SaveChanges();
            return deviceDal;
        }

        [HttpDelete]
        public void Delete(string deviceId)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            context.Devices.Remove(device);
            context.SaveChanges();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task UpdateImage(string deviceId, float widthInMeters, IFormFile image)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            using (var memoryStream = new MemoryStream())
            {
                await image.CopyToAsync(memoryStream);
                var deviceData = new DeviceData(device.Id, memoryStream.ToArray(), widthInMeters);

                if (context.DeviceData.Any(d => d.DeviceId == device.Id))
                    context.DeviceData.Update(deviceData);
                else
                    context.DeviceData.Add(deviceData);
                context.SaveChanges();
            }
        }

        [Route("[action]")]
        [HttpGet]
        public DeviceImage[] GetAllImages()
        {
            return context.DeviceData.Select(d => new DeviceImage(d)).ToArray();
        }
    }
}
