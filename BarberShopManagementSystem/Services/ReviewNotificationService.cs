using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace BarberShopManagementSystem.API.Services
{
    public class ReviewNotificationService
    {
        private readonly IArchivedAppointmentService _archivedAppointmentService;
        private readonly UserManager<User> _userManager;
        private readonly EmailHelper _emailHelper;

        public ReviewNotificationService(
            IArchivedAppointmentService archivedAppointmentService,
            UserManager<User> userManager,
            EmailHelper emailHelper)
        {
            _archivedAppointmentService = archivedAppointmentService;
            _userManager = userManager;
            _emailHelper = emailHelper;
        }

        public async Task SendPendingReviewEmails()
        {
            var now = DateTime.UtcNow;

            var archivedappointments = await _archivedAppointmentService.GetAllArchivedAppointmentsWithIncludes();

            var ready = archivedappointments
                .Where(a =>
                    a.EndTime < now.AddMinutes(-1) &&
                    !a.ReviewEmailSent);

            foreach (var appointment in ready)
            {
                var user = await _userManager.FindByIdAsync(appointment.CustomerId);

                if (user == null)
                    continue;
                if(!_userManager.IsInRoleAsync(user, "Customer").Result)
                    continue;

                var employee = await _userManager.FindByIdAsync(appointment.EmployeeId);

                if (employee == null)
                    continue;
                if (!_userManager.IsInRoleAsync(employee, "Barber").Result)
                    continue;

                await _emailHelper.SendReviewEmailAsync(user, appointment, employee);

                appointment.ReviewEmailSent = true;

                _archivedAppointmentService.UpdateArchivedAppointment(appointment);
                await _archivedAppointmentService.SaveArchivedAppointment();
            }
        }
    }
}
