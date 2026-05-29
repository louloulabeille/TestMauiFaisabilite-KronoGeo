
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using TestApiFaisabilite_KronoGeo.Infrastructure.ExtendMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers( options =>
{
    // - on peut ajouter le AuthorizeFilter au niveau global pour que toutes les routes soient protégées par défaut
    // et il faudra ajouter l'attribut [AllowAnonymous] pour les routes qui ne nécessitent pas d'authentification
    options.Filters.Add(new AuthorizeFilter());
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region DbContext
builder.Services.AddCustomDbContext(builder.Configuration);
#endregion

#region Swagger
//ajout de swagger pour la documentation de l'API & il faut installer le package
//Swashbuckle.AspNetCore pour que ça fonctionne
builder.Services.AddSwaggerGen();
#endregion

#region Authentification & JTW-Bearer & Policy Role Claims
builder.Services.AddCustonIdentityUser();
builder.Services.AddCustomlsAuthentification(builder.Configuration);
builder.Services.AddAuthorizationPolicy();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();

    #region swagger & documentation de l'API que en mode développement
    // lancement du swagger
    app.UseSwagger();
    app.UseSwaggerUI(); // lien https://localhost:7220/swagger/index.html
    #endregion
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region ajout des roles par défaut à la base de données au démarrage de l'application
await app.InitializeRolesAsync();
#endregion

app.Run();
