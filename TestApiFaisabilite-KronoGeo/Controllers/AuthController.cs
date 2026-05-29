using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TestApiFaisabilite_KronoGeo.Infrastructure.ModelsDTO;
using TestApiFaisabilite_KronoGeo.Infrastructure.Security;

namespace TestApiFaisabilite_KronoGeo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController(SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<KeyBearer> keyBearer) : Controller
    {
        #region private properties
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly KeyBearer _keyBearer = keyBearer.Value;
        #endregion


        #region Authentification login
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var retour = this.BadRequest("Problem with login and password.");

            try
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(request.Login) ?? 
                    await _signInManager.UserManager.FindByNameAsync(request.Login);

                if (user is not null && !string.IsNullOrEmpty(request.Password) 
                    && !string.IsNullOrEmpty(request.Login))
                {
                    var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
                    if(result.Succeeded)
                    {
                        // - Generate a token or set up the session as needed
                        // - For demonstration, we will just return a success message
                        request.Token = await SecurityTokenGenerate.GenerateJwtToken(user, _keyBearer, _signInManager.UserManager);
                        return this.Ok(request);
                    }

                    if (result.IsLockedOut)
                    {
                        return this.BadRequest("Account is locked for 10 minutes, after 3 attempts.");
                    }

                }
            }
            catch (Exception ex)
            {
                // - Log the exception details for debugging purposes par la suite 
                Console.WriteLine("Error during login: " + ex.Message);
                return this.Problem("Internal error occurred.");
            }

            return retour;
        }
        #endregion

        #region create user
        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] LoginRequest request)
        {
            try
            {
                var user = new IdentityUser
                {
                    UserName = string.IsNullOrEmpty(request.Login) ? request.Email : request.Login,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber
                };
                var result = await _signInManager.UserManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    // for the first account is Admin and the others are User
                    if (IsFirstAccount())
                    {
                        // Assign Admin & User role to the first user
                        await _signInManager.UserManager.AddToRolesAsync(user,["Admin", "User"]);
                        //await _signInManager.UserManager.AddToRoleAsync(user, ); 
                    }
                    else
                        await _signInManager.UserManager.AddToRoleAsync(user, "User"); // Assign default role

                    request.Token = await SecurityTokenGenerate.GenerateJwtToken(user, _keyBearer, _signInManager.UserManager);
                    return this.Ok(request);
                }
                else
                {
                    return this.BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                // - Log the exception details for debugging purposes par la suite 
                Console.WriteLine("Error during user creation: " + ex.Message);
                return this.Problem("Internal error occurred.");
            }
        }
        #endregion

        #region Logout
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(request.Login) ?? 
                    await _signInManager.UserManager.FindByNameAsync(request.Login);

                if(user is not null)
                {
                    await _signInManager.SignOutAsync();
                    request.Token = await SecurityTokenGenerate.GenerateJwtToken(user, _keyBearer, _signInManager.UserManager);
                    return this.Ok(request);
                }
                return this.BadRequest("Logout failed.");
            }
            catch (Exception ex)
            {
                // - Log the exception details for debugging purposes par la suite 
                Console.WriteLine("Error during logout: " + ex.Message);
                return this.Problem("Internal error occurred.");
            }
        }
        #endregion

        #region Test Create Role
        [HttpGet]
        [Route("Roles")]
        //[AllowAnonymous]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRoles()
        {
            /*foreach(var role in _roleManager.Roles)
            {
                await _roleManager.DeleteAsync(role);
            }*/

            return this.Ok(_roleManager.Roles);
        }
        #endregion

        #region private methods
        /// <summary>
        /// method retoune si il n'existe le premier compte créé
        /// dans la base de données pour permettre la création du compte qui sera Admin
        /// </summary>
        /// <returns></returns>
        private bool IsFirstAccount()
        {
            return _signInManager.UserManager.Users.Count() == 1;
        }

        #endregion
    }
}
