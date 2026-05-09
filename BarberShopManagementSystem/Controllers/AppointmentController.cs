using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static BarberShopManagementSystem.API.DTO.CreatedRequest.CreatedAppointmentRequest;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberScheduleService _barberScheduleService;
        private readonly IServicesService _serviceService;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(
            UserManager<User> userManager,
            IAppointmentService appointmentService,
            IBarberScheduleService barberScheduleService,
            IServicesService serviceService,
            IMapper mapper,
            ILogger<AppointmentController> logger)
        {
            _userManager = userManager;
            _appointmentService = appointmentService;
            _barberScheduleService = barberScheduleService;
            _serviceService = serviceService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDTO), StatusCodes.Status201Created)]
        public async Task<ActionResult<AppointmentDTO>> CreateAppointment(
    [FromBody] CreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _userManager.GetUserAsync(User);
            if (customer == null)
                return Unauthorized();

            var barber = await _userManager.FindByNameAsync(request.BarberUsername);
            if (barber == null || !await _userManager.IsInRoleAsync(barber, "Barber"))
                return BadRequest("Invalid barber");

            if (string.IsNullOrWhiteSpace(barber.TimeZoneId))
                return BadRequest("Barber timezone not configured");

            var service = await _serviceService.GetServiceById(request.ServiceId);
            if (service == null)
                return BadRequest("Service not found");

            // 1️⃣ Local datetime (barber local time)
            var localStart = request.Day.ToDateTime(request.StartTime);

            // 2️⃣ Convert to UTC 
            TimeZoneInfo barberTimeZone;
            try
            {
                barberTimeZone = TimeZoneInfo.FindSystemTimeZoneById(barber.TimeZoneId);
            }
            catch
            {
                return BadRequest("Invalid barber timezone");
            }

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, barberTimeZone);
            var utcEnd = utcStart.Add(service.DurationInMinutes);

            // 3️⃣ Past check (UTC)
            _logger.LogInformation("localStart: {localStart}, Kind: {kind}", localStart, localStart.Kind);
            _logger.LogInformation("utcStart: {utcStart}", utcStart);
            _logger.LogInformation("UtcNow: {utcNow}", DateTime.UtcNow);
            _logger.LogInformation("BarberTZ: {tz}", barberTimeZone.Id);

            if (utcStart < DateTime.UtcNow)
                return BadRequest("Cannot schedule appointment in the past");

            // 4️⃣ Check barber schedule
            var schedule = (await _barberScheduleService.GetAllBarberSchedules())
                .FirstOrDefault(s =>
                s.BarberId == barber.Id &&
                    s.Day.Date == request.Day.ToDateTime(TimeOnly.MinValue).Date &&
                    !s.IsDayOff);

            if (schedule == null || !schedule.StartHour.HasValue || !schedule.EndHour.HasValue)
                return BadRequest("Barber not working on this day");

            var scheduleStartUtc = TimeZoneInfo.ConvertTimeToUtc(
                request.Day.ToDateTime(TimeOnly.FromTimeSpan(schedule.StartHour.Value)),
                barberTimeZone);

            var scheduleEndUtc = TimeZoneInfo.ConvertTimeToUtc(
                request.Day.ToDateTime(TimeOnly.FromTimeSpan(schedule.EndHour.Value)),
                barberTimeZone);

            if (utcStart < scheduleStartUtc || utcEnd > scheduleEndUtc)
                return BadRequest("Appointment outside working hours");

            // 5️⃣ Overlap check (RESPECTS DURATION)
            var hasOverlap = _appointmentService.Query().Any(a =>
                a.BarberId == barber.Id &&
                utcStart < a.EndTime &&
                utcEnd > a.StartTime
            );

            if (hasOverlap)
                return Conflict("Time slot already booked");

            // 6️⃣ Create & save
            var appointment = new Appointment
            {
                CustomerId = customer.Id,
                BarberId = barber.Id,
                ServiceId = service.Id,
                StartTime = utcStart,
                EndTime = utcEnd,
                CreatedAt = DateTime.UtcNow

            };

            await _appointmentService.AddAppointment(appointment);
            await _appointmentService.SaveAppointment();

            return CreatedAtAction(
                nameof(GetAppointmentById),
                new { id = appointment.Id },
                _mapper.Map<AppointmentDTO>(appointment));
        }




        [HttpPut("{id}")]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointment(
    Guid id,
    [FromBody] CreateAppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appointment = await _appointmentService.GetAppointmentById(id);
            if (appointment == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            if (appointment.CustomerId != currentUser.Id && !User.IsInRole("Admin"))
                return Forbid();

            var barber = await _userManager.FindByNameAsync(request.BarberUsername);
            if (barber == null || !await _userManager.IsInRoleAsync(barber, "Barber"))
                return BadRequest("Invalid barber");

            if (string.IsNullOrWhiteSpace(barber.TimeZoneId))
                return BadRequest("Barber timezone not configured");

            var service = await _serviceService.GetServiceById(request.ServiceId);
            if (service == null)
                return BadRequest("Service not found");

            var localStart = request.Day.ToDateTime(request.StartTime);

            TimeZoneInfo barberTimeZone;
            try
            {
                barberTimeZone = TimeZoneInfo.FindSystemTimeZoneById(barber.TimeZoneId);
            }
            catch
            {
                return BadRequest("Invalid barber timezone");
            }

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(localStart, barberTimeZone);
            var utcEnd = utcStart.Add(service.DurationInMinutes);

            if (utcStart < DateTime.UtcNow)
                return BadRequest("Cannot schedule appointment in the past");

            var hasOverlap = _appointmentService.Query().Any(a =>
                a.BarberId == barber.Id &&
                a.Id != id &&
                utcStart < a.EndTime &&
                utcEnd > a.StartTime
            );

            if (hasOverlap)
                return Conflict("Time slot already booked");

            appointment.BarberId = barber.Id;
            appointment.ServiceId = service.Id;
            appointment.StartTime = utcStart;
            appointment.EndTime = utcEnd;

            await _appointmentService.SaveAppointment();

            return Ok(_mapper.Map<AppointmentDTO>(appointment));
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppointmentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentDTO>> GetAppointmentById([FromRoute] Guid id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);

            if (appointment == null)
                return NotFound(new { Message = "Appointment not found", AppointmentId = id });

            var appointmentDto = _mapper.Map<AppointmentDTO>(appointment);
            return Ok(appointmentDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAppointment([FromRoute] Guid id)
        {
            // 1. Validate ID
            if (id == Guid.Empty)
                return BadRequest(new { Message = "Invalid appointment ID" });

            // 2. Check appointment exists
            var appointment = await _appointmentService.GetAppointmentById(id);
            if (appointment == null)
                return NotFound(new { Message = "Appointment not found", AppointmentId = id });

            // 3. Authorization check
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized(new { Message = "User not authenticated" });

            // Only the customer who owns it, the barber, or admin can delete
            var isOwner = appointment.CustomerId == currentUser.Id;
            var isBarber = appointment.BarberId == currentUser.Id;
            var isAdmin = User.IsInRole("Admin");

            if (!isOwner && !isBarber && !isAdmin)
                return Forbid(); // 403 Forbidden

            // 4. Business rule: Cannot delete appointments that already started
            if (appointment.StartTime < DateTime.UtcNow)
                return BadRequest(new
                {
                    Message = "Cannot delete appointments that have already started or passed",
                    AppointmentStartTime = appointment.StartTime
                });

            // 5. Optional: Check if appointment is too close (e.g., within 2 hours)
            var minimumCancellationTime = TimeSpan.FromHours(2);
            if (appointment.StartTime - DateTime.UtcNow < minimumCancellationTime)
                return BadRequest(new
                {
                    Message = $"Appointments must be cancelled at least {minimumCancellationTime.TotalHours} hours in advance",
                    AppointmentStartTime = appointment.StartTime,
                    MinimumCancellationTime = minimumCancellationTime.ToString()
                });

            // 6. Delete the appointment
            try
            {
                _appointmentService.DeleteAppointment(id);
                await _appointmentService.SaveAppointment();
            }
            catch (Exception ex)
            {
                // Log the exception here (use ILogger)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "An error occurred while deleting the appointment" });
            }

            // 7. Return 204 No Content (standard for successful deletion)
            return NoContent();
        }

        [HttpGet("availability")]
        [ProducesResponseType(typeof(List<TimeOnly>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TimeOnly>>> GetAvailability(
    [FromQuery] string barberId,
    [FromQuery] DateOnly date)
        {
            var barber = await _userManager.FindByIdAsync(barberId);
            if (barber == null || !await _userManager.IsInRoleAsync(barber, "Barber"))
                return BadRequest("Invalid barber");

            if (string.IsNullOrWhiteSpace(barber.TimeZoneId))
                return BadRequest("Barber timezone not configured");

            var schedule = (await _barberScheduleService.GetAllBarberSchedules())
                .FirstOrDefault(s =>
                    s.BarberId == barberId &&
                    s.Day.Date == date.ToDateTime(TimeOnly.MinValue).Date &&
                    !s.IsDayOff);

            if (schedule == null || !schedule.StartHour.HasValue || !schedule.EndHour.HasValue)
                return Ok(new List<TimeOnly>());

            TimeZoneInfo barberTz;
            try
            {
                barberTz = TimeZoneInfo.FindSystemTimeZoneById(barber.TimeZoneId);
            }
            catch
            {
                return BadRequest("Invalid timezone");
            }

            const int slotStep = 15; // universal granularity

            var start = TimeOnly.FromTimeSpan(schedule.StartHour.Value);
            var end = TimeOnly.FromTimeSpan(schedule.EndHour.Value);
            var buffer = TimeSpan.FromMinutes(5);
            var current = start;

            var availableSlots = new List<TimeOnly>();

            while (current.AddMinutes(slotStep) <= end)
            {
                var utcStart = TimeZoneInfo.ConvertTimeToUtc(date.ToDateTime(current), barberTz);
                var utcEnd = utcStart.AddMinutes(slotStep);

                // slot is free if no appointment overlaps this 15-min window
                var hasOverlap = _appointmentService.Query().Any(a =>
                    a.BarberId == barberId &&
                    utcStart < a.EndTime &&
                    utcEnd > a.StartTime);

                if (!hasOverlap && utcStart > DateTime.UtcNow.Add(buffer))
                    availableSlots.Add(current);

                current = current.AddMinutes(slotStep);
            }

            return Ok(availableSlots);
        }
    }
}