using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivedAppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public ArchivedAppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

    }
}
