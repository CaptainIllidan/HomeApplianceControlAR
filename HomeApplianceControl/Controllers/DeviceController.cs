using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return GetDeviceInternal(id, Assertion.Exist);
        }

        [HttpPost]
        public Device Post(Device device)
        {
            GetDeviceInternal(device.Id, Assertion.NotExist);
            var deviceDal = context.Devices.Add(device)?.Entity;
            context.SaveChanges();
            return deviceDal;
        }

        [HttpPut]
        public Device Put(Device device)
        {
            var deviceDal = GetDeviceInternal(device.Id, Assertion.Exist);
            deviceDal.DeviceType = device.DeviceType;
            deviceDal.IntegrationType = device.IntegrationType;
            context.Devices.Update(deviceDal);
            context.SaveChanges();
            return deviceDal;
        }

        [HttpDelete]
        public void Delete(Guid deviceId)
        {
            var device = GetDeviceInternal(deviceId, Assertion.Exist);
            context.Devices.Remove(device);
            context.SaveChanges();
        }

        private Device GetDeviceInternal(Guid deviceId, Assertion assertion)
        {
            var device = context.Devices.SingleOrDefault(d => d.Id == deviceId);
            Assert(device, assertion);
            return device;
        }

        private void Assert(Device device, Assertion assertion)
        {
            switch (assertion)
            {
                case Assertion.Exist:
                    if (device == null)
                        throw new ApplicationException("Device not found");
                    break;
                case Assertion.NotExist:
                    if (device != null)
                        throw new ApplicationException($"Device with id [{device.Id}] already exist");
                    break;
                default:
                    throw new ArgumentException($"Unsupported assertion {assertion.GetType()}:{assertion}");
            }
        }

        private enum Assertion
        {
            Exist = 0,
            NotExist = 1,
        }
    }
}
