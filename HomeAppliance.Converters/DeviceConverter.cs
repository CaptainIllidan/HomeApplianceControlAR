using HomeApplianceControl.Common;
using HomeApplianceControl.Contracts;

namespace HomeApplianceControl.Converters
{
    public static class DeviceConverter
    {
        public static Device ToDto(this HomeApplianceControl.Domain.BO.Device device)
        {
            return new Device
            {
                Id = device.Id,
                DeviceType =
                    EnumUtils.Convert<HomeApplianceControl.Domain.BO.DeviceType, DeviceType>(device.DeviceType),
                IntegrationType =
                    EnumUtils.Convert<HomeApplianceControl.Domain.BO.IntegrationType, IntegrationType>(
                        device.IntegrationType),
            };
        }

        public static HomeApplianceControl.Domain.BO.Device ToBo(this Device device)
        {
            return new HomeApplianceControl.Domain.BO.Device(device.Id,
                EnumUtils.Convert<DeviceType, HomeApplianceControl.Domain.BO.DeviceType>(device.DeviceType),
                EnumUtils.Convert<IntegrationType, HomeApplianceControl.Domain.BO.IntegrationType>(
                    device.IntegrationType));
        }
    }
}
