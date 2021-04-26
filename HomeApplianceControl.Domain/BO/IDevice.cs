using System;
using System.Collections.Generic;
using System.Text;

namespace HomeApplianceControl.Domain.BO
{
    public interface IDevice
    {
        Guid Id { get; set; }

        DeviceType DeviceType { get; set; }

        IntegrationType IntegrationType { get; set; }
    }
}
