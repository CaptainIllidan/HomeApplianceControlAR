using System;
using HomeApplianceControl.Domain.BO;
using Microsoft.EntityFrameworkCore;

namespace HomeApplianceControl.Domain.DAL
{
    public class HomeApplianceControlContext : DbContext
    {
        public HomeApplianceControlContext(DbContextOptions<HomeApplianceControlContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().ToTable("Device");
        }
    }
}
