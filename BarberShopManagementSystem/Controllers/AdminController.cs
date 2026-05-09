using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly JWTService jwtService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        
        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager, JWTService jwtService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtService = jwtService;
        }

        [HttpPost("Login_Admin")]
        public async Task<ActionResult<UserDTO>> LoginAdmin(CreatedLoginRequest request)
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null) { return NotFound("User Not Found!"); }
            var isCustomer = await userManager.IsInRoleAsync(user, "Admin");
            if (!isCustomer)
            {
                return BadRequest("User is not an Admin");
            }
            if (user == null) return Unauthorized("Invalid Username or Password");
            if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email!");
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid Username or Password!");

            return await CreateApplicationUserDTO(user);
        }

        [HttpPost("Create_Admin")]
        public async Task<IActionResult> CreateAdmin(RegisterAdminRequest request)
        {
            if (await userManager.FindByNameAsync(request.Username) != null)
                return BadRequest("Username already exists.");

            var admin = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Username,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(admin, "Admin");

            return Ok("Admin created successfully");
        }


        #region
        private async Task<UserDTO> CreateApplicationUserDTO(User user)
        {
            return new UserDTO
            {
                FullName = $"{user.FirstName} {user.LastName}",
                JWT = await jwtService.CreateJWT(user)
            };
        }
        #endregion
    }
}
