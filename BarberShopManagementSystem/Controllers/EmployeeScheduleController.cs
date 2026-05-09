using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
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
    public class EmployeeScheduleController : ControllerBase
    {
        private readonly IBarberScheduleService _barberScheduleService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public EmployeeScheduleController(
            IBarberScheduleService barberScheduleService,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _barberScheduleService = barberScheduleService;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: api/BarberSchedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BarberScheduleDTO>>> GetAll()
        {
            var schedules = await _barberScheduleService.GetAllBarberSchedules();
            var schedulesDTO = _mapper.Map<IEnumerable<BarberScheduleDTO>>(schedules);
            return Ok(schedulesDTO);
        }

        [HttpGet("{username}/{day}")]
        public async Task<ActionResult<BarberScheduleDTO>> GetByBarberDay(string username, DateTime day)
        {
            var schedule = await _barberScheduleService
                .GetAllBarberSchedules();

            var result = schedule.FirstOrDefault(s => s.Barber.UserName == username && s.Day.Date == day.Date);

            if (result == null)
                return NotFound();
            var scheduleDTO = _mapper.Map<BarberScheduleDTO>(result);

            return Ok(scheduleDTO);
        }


        [HttpPost("Add_Schedule")]
        public async Task<ActionResult<IEnumerable<BarberScheduleDTO>>> CreateSchedule(
            [FromBody] List<CreatedBarberScheduleRequest> schedules)
        {
            if (schedules == null || !schedules.Any())
                return BadRequest("No schedules provided.");

            var createdSchedules = new List<BarberSchedule>();
            var existingSchedules = (await _barberScheduleService.GetAllBarberSchedules()).ToList();

            foreach (var request in schedules)
            {
                var barber = await _userManager.FindByNameAsync(request.Username);
                if (barber == null)
                    return BadRequest($"Barber '{request.Username}' not found");

                if (!await _userManager.IsInRoleAsync(barber, "Barber"))
                    return BadRequest($"User '{request.Username}' is not a barber");

                if (string.IsNullOrWhiteSpace(barber.TimeZoneId))
                    return BadRequest("Barber timezone not configured");

                // ✅ Use barber timezone instead of server time
                var barberTimeZone = TimeZoneInfo.FindSystemTimeZoneById(barber.TimeZoneId);
                var barberNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, barberTimeZone);

                if (request.Day.Date < barberNow.Date)
                    return BadRequest($"Cannot create schedule for a past date: {request.Day:yyyy-MM-dd}");

                if (!request.IsDayOff)
                {
                    if (!request.StartHour.HasValue || !request.EndHour.HasValue)
                        return BadRequest($"StartHour and EndHour are required for {request.Day:yyyy-MM-dd}");

                    if (request.StartHour >= request.EndHour)
                        return BadRequest($"StartHour must be before EndHour for {request.Day:yyyy-MM-dd}");
                }
                else
                {
                    request.StartHour = null;
                    request.EndHour = null;
                }

                // ✅ Duplicate check (date only)
                if (existingSchedules.Any(s =>
                    s.BarberId == barber.Id &&
                    s.Day.Date == request.Day.Date))
                {
                    return BadRequest($"Schedule already exists for {request.Day:yyyy-MM-dd}");
                }

                var entity = _mapper.Map<BarberSchedule>(request);
                entity.BarberId = barber.Id;

                var created = await _barberScheduleService.AddBarberSchedule(entity);
                createdSchedules.Add(created);
                existingSchedules.Add(created);
            }

            foreach (var s in createdSchedules)
                await _barberScheduleService.SaveBarberSchedule();

            var result = _mapper.Map<IEnumerable<BarberScheduleDTO>>(createdSchedules);
            return Ok(result);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] CreatedBarberScheduleRequest request)
        {
            var existing = (await _barberScheduleService.GetAllBarberSchedules())
                .FirstOrDefault(s => s.Barber.UserName == request.Username && s.Day.Date == request.Day.Date);

            if (existing == null)
                return NotFound();

            if (request.Day.Date < DateTime.Today)
            {
                return BadRequest($"Cannot create schedule for a past date: {request.Day:yyyy-MM-dd}");
            }

            if (!request.IsDayOff)
            {
                if (!request.StartHour.HasValue || !request.EndHour.HasValue)
                    return BadRequest("StartHour and EndHour are required");

                if (request.StartHour >= request.EndHour)
                    return BadRequest("StartHour must be before EndHour");
            }
            else
            {
                request.StartHour = null;
                request.EndHour = null;
            }

            _mapper.Map(request, existing);

            _barberScheduleService.UpdateBarberSchedule(existing);

            await _barberScheduleService.SaveBarberSchedule();

            return Ok();
        }


        [HttpDelete]
        public async Task<ActionResult> DeleteSchedule(string username, DateTime day)
        {
            var existing = (await _barberScheduleService.GetAllBarberSchedules())
                .FirstOrDefault(s => s.Barber.UserName == username && s.Day.Date == day.Date);

            if (existing == null)
                return NotFound();

            _barberScheduleService.DeleteBarberSchedule(existing);

            await _barberScheduleService.SaveBarberSchedule();

            return NoContent();
        }

    }
}
