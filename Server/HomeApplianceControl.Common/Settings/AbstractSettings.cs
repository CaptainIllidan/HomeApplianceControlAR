using System;
using System.Collections.Generic;
using System.Text;

namespace HomeApplianceControl.Common.Settings
{
    public abstract class AbstractSettings
    {
        public bool UseLocal { get; set; }

        public string ApiUrl { get; set; }

        public string ThirdPartyPath { get; set; }
    }
}
