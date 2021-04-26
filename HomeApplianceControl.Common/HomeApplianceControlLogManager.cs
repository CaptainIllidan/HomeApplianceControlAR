using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using log4net;
using log4net.Config;

namespace HomeApplianceControl.Common
{
    public static class HomeApplianceControlLogManager
    {
        private static ILogManager logManager;

        private static ILogManager LogManager => logManager ?? throw new ApplicationException("Log Manager not initialized");

        public static void Init([NotNull] string logConfigPath)
        {
            var logConfig = new FileInfo(logConfigPath);
            XmlConfigurator.Configure(log4net.LogManager.GetRepository(Assembly.GetCallingAssembly()), logConfig);
        }

        [NotNull]
        public static ILog GetLogger([NotNull] string name) => LogManager.GetLogger(name);

        public static ILog GetLogger([NotNull] Type type) => LogManager.GetLogger(type);
    }
}
