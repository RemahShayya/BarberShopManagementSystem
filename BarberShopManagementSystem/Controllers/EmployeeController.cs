using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using PhoneNumbers;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly JWTService jwtService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly EmailService emailService;
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        private readonly IMapper mapper;

        public EmployeeController(JWTService jwtService, SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration, IUserService userService, IMapper mapper, EmailService emailService)
        {
            this.jwtService = jwtService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.configuration = configuration;
            this.userService = userService;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        [HttpGet("get_barber/{username}")]
        public async Task<ActionResult<UserDTO>> GetBarberById(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return NotFound($"Barber with Username {username} not found");
            }

            var isBarber = await userManager.IsInRoleAsync(user, "Barber");
            if (!isBarber)
            {
                return BadRequest("User is not a barber");
            }

            var roles = await userManager.GetRolesAsync(user);

            var userDTO = mapper.Map<UserDTO>(user);
            userDTO.Role = roles.FirstOrDefault(); 

            return Ok(userDTO);
        }

        [HttpGet("get_all_barbers")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllEmployees(string? search, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var barbers = await userManager.GetUsersInRoleAsync("Barber");
            var barbersList = barbers.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                barbersList = barbersList
                    .Where(b => (b.FirstName + " " + b.LastName)
                        .Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var paginatedBarbers = barbersList
                .OrderBy(c => c.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userBarberDTO = new List<UserDTO>();

            foreach (var user in paginatedBarbers)
            {
                var roles = await userManager.GetRolesAsync(user);
                var dto = mapper.Map<UserDTO>(user);
                dto.Role = roles.FirstOrDefault();
                userBarberDTO.Add(dto);
            }

            return Ok(userBarberDTO);
        }

        [HttpPost("Register_Barber")]
        public async Task<IActionResult> Register(CreatedCustomerAndBarberSignUpRequest request)
        {
            if (await CheckIfEmailExist(request.Email))
                return Unauthorized($"An existing account is using {request.Email}");
            if(!IsPhoneValid(request.CountryCode, request.PhoneNumber))
                return BadRequest("Invalid phone number");

            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZoneId);
            }
            catch
            {
                return BadRequest("Invalid timezone ID");
            }

            var addedUser = new User
            {
                FirstName = request.Firstname.Trim(),
                LastName = request.Lastname.Trim(),
                Email = request.Email.ToLower(),
                PhoneNumber = request.PhoneNumber,
                UserName = $"{request.Firstname.ToLower().Replace(" ", "")}.{request.Lastname.ToLower().Replace(" ", "")}.{Guid.NewGuid().ToString()[..4]}",
                DateCreated = DateTime.UtcNow,
                TimeZoneId = request.TimeZoneId
            };

            var result = await userManager.CreateAsync(addedUser, request.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(addedUser, "Barber");

            var sent = await SendConfirmationEmailAsync(addedUser);

            if (!sent)
                return BadRequest("Failed to send confirmation email");

            return Ok(new
            {
                title = "Account Created",
                message = "Please confirm your email address before logging in.",
                username = addedUser.UserName
            });
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> login(CreatedLoginRequest request)
        {
            var user = await userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized("Invalid Username or Password");
            }
            if (user.EmailConfirmed == false)
            {
                return Unauthorized("Please confirm your email!");
            }
            var isBarber = await userManager.IsInRoleAsync(user, "Barber");
            if (!isBarber)
            {
                return BadRequest("User is not a barber");
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid Username or Password!");
            }
            var userDTO = mapper.Map<UserDTO>(user);
            userDTO.Role = "Barber";
            userDTO.JWT = await jwtService.CreateJWT(user);

            return Ok(userDTO);
        }

        [HttpPut("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDTO model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("Email has not been registered yet!");
            if (user.EmailConfirmed == true) return BadRequest("Your email address is already confirmed, please login to your account");

            try
            {
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
                var result = await userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new
                    {
                        title = "Email confirmed",
                        message = "Your email has been confirmed",
                        username = user.UserName
                    }));
                }
                return BadRequest("Email could not be confirmed!");
            }
            catch (Exception)
            {
                return BadRequest("Email could not be confirmed!");
            }
        }

        [HttpPost("ForgotPassword/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return BadRequest("Invalid email!");
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized("This email address hasn't been registered yet");
            if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email first!");

            try
            {
                if (await SendForgotUsernameOrPasswordEmail(user))
                {
                    return Ok(new JsonResult(new { title = "Reset link sent", message = "Please check your email to reset your username or password" }));
                }
                return BadRequest("Failed to send email!");
            }
            catch (Exception)
            {
                return BadRequest("Failed to send email!");

            }
        }

        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null) return BadRequest("Email has not been registered yet!");
            if (user.EmailConfirmed == false) return Unauthorized("Please confirm your email first!");

            try
            {
                var result = await userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Password reset succeeded", message = "Password have been reset" }));
                }
                return BadRequest("Password could not be reset!");
            }
            catch (Exception)
            {
                return BadRequest("Password could not be reset!");
            }
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


        private async Task<bool> CheckIfEmailExist(string email)
        {
            return await userManager.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        private async Task<bool> SendConfirmationEmailAsync(User user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{configuration["JwtToken:ClientURL"]}/{configuration["Email:ConfirmationEmailPath"]}?token={encodedToken}&email={user.Email}";
            var body = $"<p>Hello:{user.FirstName}</p>" +
                "<p>Please confirm your email address by clicking on the following link.</p>" +
                $"<p><a href=\"{confirmationLink}\">Click here</a></p>" +
                "<p>Thank You!</p>" +
                $"<br><p>{configuration["Email:ApplicationName"]}</p>";
            var emailSent = new SendEmailDTO(user.Email, "Confirm your email!", body);

            return await emailService.SendEmailAsync(emailSent);
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(User user)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{configuration["JWT:ClientURL"]}/{configuration["Email:ResetPasswordPath"]}?token={token}&email={user.Email}";
            var body = $"<p>Hello:{user.FirstName}</p>" +
                "<p>Please reset your username or password by clicking on the following link.</p>" +
                $"<p><a href=\"{resetLink}\">Click here</a></p>" +
                "<p>Thank You!</p>" +
                $"<br><p>{configuration["Email:ApplicationName"]}</p>";
            var emailSent = new SendEmailDTO(user.Email, "Reset your username or password!", body);
            return await emailService.SendEmailAsync(emailSent);
        }


        public bool IsPhoneValid(string countryCode, string phone)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();

            try
            {
                var number = phoneNumberUtil.Parse(countryCode + phone, null);
                return phoneNumberUtil.IsValidNumber(number);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}