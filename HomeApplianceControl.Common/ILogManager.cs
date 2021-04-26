using System;
using JetBrains.Annotations;
using log4net;

namespace HomeApplianceControl.Common
{
    public interface ILogManager
    {
        [NotNull]
        ILog GetLogger([NotNull] string name);

        [NotNull]
        ILog GetLogger([NotNull] Type type);
    }
}
