using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Logging;

namespace HomeApplianceControl.Common
{
    public static class HomeApplianceControlLogManager
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        public static void Init([NotNull] string logConfigPath)
        {
            var logConfig = new FileInfo(logConfigPath);
            XmlConfigurator.Configure(log4net.LogManager.GetRepository(Assembly.GetCallingAssembly()), logConfig);
        }

        [NotNull]
        public static ILogger GetLogger([NotNull] string name) => LoggerFactory.CreateLogger(name);

        public static ILogger GetLogger<T>() => LoggerFactory.CreateLogger<T>();
    }
}
