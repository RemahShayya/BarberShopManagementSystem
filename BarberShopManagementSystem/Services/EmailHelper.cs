using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.Services
{
    public class EmailHelper
    {
        private readonly EmailService emailService;
        private readonly IConfiguration configuration;

        public EmailHelper(EmailService emailService, IConfiguration configuration)
        {
            this.emailService = emailService;
            this.configuration = configuration;
        }

        public async Task<bool> SendReviewEmailAsync(User user, ArchivedAppointment archivedAppointment, User barber)
        {
            var link = $"{configuration["JwtToken:ClientURL"]}/review?token={archivedAppointment.ReviewToken}";
            var subject = $"Rate your experience at our barbershop with {barber.FirstName} {barber.LastName}";
            var body = $"<p>Hello {user.FirstName},</p>" +
                       $"<p>Please rate your experience with {barber.FirstName} {barber.LastName}.</p>" +
                       $"<a href=\"{link}\">Leave a Review</a>";

            var email = new SendEmailDTO(user.Email, subject, body);

            return await emailService.SendEmailAsync(email);
        }
    }
}
