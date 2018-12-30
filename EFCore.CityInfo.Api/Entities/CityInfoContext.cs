using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCore.CityInfo.Api.Entities
{
    public class CityInfoContext:DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options):base(options)
        {
            Database.Migrate();
          //  Database.EnsureCreated();
        }
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }
    }
}
