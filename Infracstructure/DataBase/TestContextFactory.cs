using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infracstructure.DataBase
{
    public class TestContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseSqlite("C:\\Users\\loulo\\Desktop\\KronoGeo\\TestMauiFaisabilite-KronoGeo\\Infracstructure\\Data\\TestIdentity.db;");
            return new TestDbContext(optionsBuilder.Options);
        }
    
    }
}
