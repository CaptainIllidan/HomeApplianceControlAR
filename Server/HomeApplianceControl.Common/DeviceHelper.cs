using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HomeApplianceControl.Domain.BO;
using HomeApplianceControl.Domain.DAL;

namespace HomeApplianceControl.Common
{
    public static class DeviceHelper
    {
        public static Device GetDeviceInternal(HomeApplianceControlContext context, string deviceId, Assertion assertion)
        {
            var device = context.Devices.SingleOrDefault(d => d.Id == deviceId);
            Assert(device, assertion);
            return device;
        }

        private static void Assert(Device device, Assertion assertion)
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

        public enum Assertion
        {
            Exist = 0,
            NotExist = 1,
        }
    }
}
