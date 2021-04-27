using System;
using System.Collections.Generic;
using System.Text;

namespace HomeApplianceControl.Domain.DAL
{
    public static class DbInitializer
    {
        public static void Initialize(HomeApplianceControlContext context)
        {
            context.Database.EnsureCreated();
        }
    }
}
