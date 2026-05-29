using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace TestApiFaisabilite_KronoGeo.Infrastructure.ExtendMethods
{
    public static class DbContextExtends
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddCustomDbContext(IConfiguration config)
            {
                // - on peut aussi ajouter une configuration de la base de données dans le settings
                // et la récupérer ici pour ne pas l'avoir en dur dans le code
                string connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
                services.AddDbContext<TestDbContext>(options =>
                {
                    options.UseSqlite(connectionString);
                });
                return services;

            }
        }
    }
}
