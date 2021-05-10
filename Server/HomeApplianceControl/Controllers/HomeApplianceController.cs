using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeApplianceControl.Common;
using HomeApplianceControl.Common.CallerHelpers;
using HomeApplianceControl.Common.DeviceCommands;
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

        private readonly CallerHelperManager CallerHelperManager;

        public HomeApplianceController(HomeApplianceControlContext context, CallerHelperManager callerHelperManager)
        {
            this.context = context;
            CallerHelperManager = callerHelperManager;
        }

        [HttpGet]
        public IEnumerable<Command> GetAvailableCommands(string deviceId)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            return CommandsHelper.GetAvailableCommands(device);
        }

        [HttpGet]
        public void ExecuteCommand(string deviceId, Command command, string args)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            CallerHelperManager.GetCallerHelper(device)
                .ExecuteCommand(device, command, args);
        }

        [HttpGet]
        public IDictionary<string, string> GetState(string deviceId)
        {
            var device = DeviceHelper.GetDeviceInternal(context, deviceId, DeviceHelper.Assertion.Exist);
            return CallerHelperManager.GetCallerHelper(device)
                .GetState(device);
        }
    }
}
