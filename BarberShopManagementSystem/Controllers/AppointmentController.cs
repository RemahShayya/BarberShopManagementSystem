using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberScheduleService _barberService;
        private readonly IServicesService _servicesService;
        private readonly IMapper _mapper;
        public AppointmentController(IAppointmentService appointmentService, IMapper mapper)
        {
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetAppointmentById(Guid id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);
            if (appointment == null) { return NotFound("Appointment Not Found"); }
            var appointmentDTO = _mapper.Map<AppointmentDTO>(appointment);
            return Ok(appointmentDTO);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointments();
            var appointmentsDTO = _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
            return Ok(appointmentsDTO);
        }

        [HttpPost]
        public async Task<ActionResult<AppointmentDTO>> CreateAppointment([FromBody] CreatedAppointmentRequest request)
        {
            var createdAppointment = _mapper.Map<Appointment>(request);
            await _appointmentService.AddAppointment(createdAppointment);
            await _appointmentService.SaveAppointment(createdAppointment);
            var AppointmentDTO = _mapper.Map<AppointmentDTO>(createdAppointment);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = AppointmentDTO.Id }, AppointmentDTO);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointment(Guid id, [FromBody] CreatedAppointmentRequest request)
        {
            var existingAppointment = await _appointmentService.GetAppointmentById(id);
            if (existingAppointment == null)
            {
                return NotFound("Appointment Not Found");
            }
            var updatedAppointment = _mapper.Map<Appointment>(request);
            var isUpdated = _appointmentService.UpdateAppointment(updatedAppointment);
            if (!isUpdated)
            {
                return BadRequest("Failed to Update Appointment");
            }
            await _appointmentService.SaveAppointment(updatedAppointment);
            var updatedAppointmentDTO = _mapper.Map<AppointmentDTO>(updatedAppointment);
            return Ok(updatedAppointmentDTO);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var existingAppointment = await _appointmentService.GetAppointmentById(id);
            if (existingAppointment == null)
            {
                return NotFound("Appointment Not Found");
            }
            _appointmentService.DeleteAppointment(existingAppointment);
            await _appointmentService.SaveAppointment(existingAppointment);
            return NoContent();
        }
    }
}