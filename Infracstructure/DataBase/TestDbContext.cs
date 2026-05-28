using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.DataBase
{
    public class TestDbContext : IdentityDbContext
    {
        #region Constructeur
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected TestDbContext()
        {
        }
        #endregion

        #region override method
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\loulo\\Desktop\\KronoGeo\\TestMauiFaisabilite-KronoGeo\\Infracstructure\\Data\\TestIdentity.db;");
            //base.OnConfiguring(optionsBuilder);
        }
        #endregion

    }
}
