using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly JWTService jwtService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly EmailService emailService;
        private readonly IConfiguration configuration;

        public AdminController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JWTService jwtService,
            EmailService emailService,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtService = jwtService;
            this.emailService = emailService;
            this.configuration = configuration;
        }

        [HttpPost("Login_Admin")]
        public async Task<ActionResult<UserDTO>> LoginAdmin(CreatedLoginRequest request)
        {
            var user = await userManager.FindByNameAsync(request.Username);

            if (user == null)
                return Unauthorized("Invalid Username or Password");

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
                return BadRequest("User is not an Admin");

            if (!user.EmailConfirmed)
                return Unauthorized("Please confirm your email!");

            var result = await signInManager.CheckPasswordSignInAsync(
                user,
                request.Password,
                false);

            if (!result.Succeeded)
                return Unauthorized("Invalid Username or Password");

            return await CreateApplicationUserDTO(user);
        }

        [Authorize(Roles = "Admin")]
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
                Email = request.Email,
                EmailConfirmed = true,
                DateCreated = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(admin, "Admin");

            return Ok("Admin created successfully");
        }

        [HttpPost("ForgotPassword/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Invalid email.");

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Ok(new
                {
                    message = "If an account exists, a reset email has been sent."
                });
            }

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                return Ok(new
                {
                    message = "If an account exists, a reset email has been sent."
                });
            }

            if (!user.EmailConfirmed)
                return Unauthorized("Please confirm your email first!");

            var emailSent = await SendForgotPasswordEmail(user);

            if (!emailSent)
                return BadRequest("Failed to send email.");

            return Ok(new
            {
                title = "Reset Link Sent",
                message = "Please check your email to reset your password."
            });
        }

        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return BadRequest("Invalid request.");

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
                return BadRequest("Invalid request.");

            try
            {
                var decodedToken =
                    Encoding.UTF8.GetString(
                        WebEncoders.Base64UrlDecode(request.Token));

                var result = await userManager.ResetPasswordAsync(
                    user,
                    decodedToken,
                    request.NewPassword);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(new
                {
                    title = "Password Reset",
                    message = "Password has been reset successfully."
                });
            }
            catch
            {
                return BadRequest("Password could not be reset.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(CreateChangePassword request)
        {
            if (request.NewPassword != request.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("Admin not found.");

            var result = await userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                title = "Password Changed",
                message = "Your password has been changed successfully."
            });
        }

        #region Private Methods

        private async Task<UserDTO> CreateApplicationUserDTO(User user)
        {
            return new UserDTO
            {
                FullName = $"{user.FirstName} {user.LastName}",
                JWT = await jwtService.CreateJWT(user)
            };
        }

        private async Task<bool> SendForgotPasswordEmail(User user)
        {
            var token =
                await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken =
                WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(token));

            var resetLink =
                $"{configuration["JwtToken:ClientURL"]}/" +
                $"{configuration["Email:ResetPasswordPath"]}" +
                $"?token={encodedToken}&email={user.Email}";

            var body =
                $"<p>Hello {user.FirstName},</p>" +
                "<p>Please click the link below to reset your password.</p>" +
                $"<p><a href=\"{resetLink}\">Reset Password</a></p>" +
                "<p>Thank You!</p>" +
                $"<br><p>{configuration["Email:ApplicationName"]}</p>";

            var email =
                new SendEmailDTO(
                    user.Email,
                    "Reset Your Password",
                    body);

            return await emailService.SendEmailAsync(email);
        }

        #endregion
    }
}