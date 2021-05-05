using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeApplianceControl.Common;
using HomeApplianceControl.Common.DeviceCommands;
using HomeApplianceControl.Common.LgCallerHelper;
using HomeApplianceControl.Domain.BO;
using HomeApplianceControl.Domain.DAL;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeApplianceControl.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeApplianceController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HomeApplianceController));

        private readonly HomeApplianceControlContext context;

        private readonly ILgCallerHelper lgCallerHelper;

        public HomeApplianceController(HomeApplianceControlContext context, ILgCallerHelper lgCallerHelper)
        {
            this.context = context;
            this.lgCallerHelper = lgCallerHelper;
        }

        [HttpGet]
        public IEnumerable<Command> GetAvailableCommands(Guid deviceId)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            return CommandsHelper.GetAvailableCommands(device);
        }

        [HttpGet]
        public void ExecuteCommand(Guid deviceId, Command command, string args)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            if (device.IntegrationType == IntegrationType.Lg)
                lgCallerHelper.ExecuteCommand(device, command, args);
        }

        [HttpGet]
        public IDictionary<string, string> GetState(Guid deviceId)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            if (device.IntegrationType == IntegrationType.Lg)
                return lgCallerHelper.GetState(device);

            return new Dictionary<string, string>();
        }
    }
}
