using System;
using System.Collections.Generic;
using System.Text;
using HomeApplianceControl.Domain.BO;
using Microsoft.Extensions.Options;

namespace HomeApplianceControl.Common.CallerHelpers
{
    public class CallerHelperManager
    {
        private readonly ILgCallerHelper LgCallerHelper;

        private readonly IElectroluxCallerHelper ElectroluxCallerHelper;

        public CallerHelperManager(ILgCallerHelper lgCaller, IElectroluxCallerHelper electroluxCaller)
        {
            LgCallerHelper = lgCaller;
            ElectroluxCallerHelper = electroluxCaller;
        }
        
        public ICallerHelper GetCallerHelper(Device device)
        {
            switch (device.IntegrationType)
            {
                case IntegrationType.Lg:
                    return LgCallerHelper;
                case IntegrationType.Electrolux:
                    return ElectroluxCallerHelper;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported integration type {device.IntegrationType}");
            }
        }
    }
}
