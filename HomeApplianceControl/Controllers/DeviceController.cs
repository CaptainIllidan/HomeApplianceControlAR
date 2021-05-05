using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeApplianceControl.Common;
using HomeApplianceControl.Common.LgCallerHelper;
using HomeApplianceControl.Contracts;
using HomeApplianceControl.Domain.DAL;
using log4net;
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
        public Device Get(Guid id)
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
        public void Delete(Guid deviceId)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            context.Devices.Remove(device);
            context.SaveChanges();
        }

        
    }
}
