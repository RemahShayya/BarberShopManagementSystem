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
        private readonly IEmployeeScheduleService _employeeScheduleService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public EmployeeScheduleController(
            IEmployeeScheduleService employeeScheduleService,
            IMapper mapper,
            UserManager<User> userManager)
        {
            _employeeScheduleService = employeeScheduleService;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: api/BarberSchedule
        [HttpGet]
        [Authorize(Roles = "Employee, Admin, Customer")]
        public async Task<ActionResult<IEnumerable<EmployeeScheduleDTO>>> GetAll()
        {
            var schedules = await _employeeScheduleService.GetAllEmployeeSchedules();
            var schedulesDTO = _mapper.Map<IEnumerable<EmployeeScheduleDTO>>(schedules);
            return Ok(schedulesDTO);
        }

        [HttpGet("{username}/{day}")]
        [Authorize(Roles = "Employee, Admin, Customer")]
        public async Task<ActionResult<EmployeeScheduleDTO>> GetByEmployeeDay(string username, DateTime day)
        {
            var schedule = await _employeeScheduleService
                .GetAllEmployeeSchedules();

            var result = schedule.FirstOrDefault(s => s.Employee.UserName == username && s.Day.Date == day.Date);

            if (result == null)
                return NotFound();
            var scheduleDTO = _mapper.Map<EmployeeScheduleDTO>(result);

            return Ok(scheduleDTO);
        }


        [HttpPost("Add_Schedule")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<EmployeeScheduleDTO>>> CreateSchedule(
            [FromBody] List<CreatedEmployeeScheduleRequest> schedules)
        {
            if (schedules == null || !schedules.Any())
                return BadRequest("No schedules provided.");

            var createdSchedules = new List<EmployeeSchedule>();
            var existingSchedules = (await _employeeScheduleService.GetAllEmployeeSchedules()).ToList();

            foreach (var request in schedules)
            {
                var employee = await _userManager.FindByNameAsync(request.Username);
                if (employee == null)
                    return BadRequest($"Employee '{request.Username}' not found");

                if (!await _userManager.IsInRoleAsync(employee, "Employee"))
                    return BadRequest($"User '{request.Username}' is not an employee");
                    
                if (string.IsNullOrWhiteSpace(employee.TimeZoneId))
                    return BadRequest("Employee timezone not configured");

                // ✅ Use employee timezone instead of server time
                var employeeTimeZone = TimeZoneInfo.FindSystemTimeZoneById(employee.TimeZoneId);
                var employeeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, employeeTimeZone);

                if (request.Day.Date < employeeNow.Date)
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
                    s.EmployeeId == employee.Id &&
                    s.Day.Date == request.Day.Date))
                {
                    return BadRequest($"Schedule already exists for {request.Day:yyyy-MM-dd}");
                }

                var entity = _mapper.Map<EmployeeSchedule>(request);
                entity.EmployeeId = employee.Id;

                var created = await _employeeScheduleService.AddEmployeeSchedule(entity);
                createdSchedules.Add(created);
                existingSchedules.Add(created);
            }

            foreach (var s in createdSchedules)
                await _employeeScheduleService.SaveEmployeeSchedule();

            var result = _mapper.Map<IEnumerable<EmployeeScheduleDTO>>(createdSchedules);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update([FromBody] CreatedEmployeeScheduleRequest request)
        {
            var existing = (await _employeeScheduleService.GetAllEmployeeSchedules())
                .FirstOrDefault(s => s.Employee.UserName == request.Username && s.Day.Date == request.Day.Date);

            if (existing == null)
                return NotFound();

            if (request.Day.Date < DateTime.Today)
            {
                return BadRequest($"Cannot update schedule for a past date: {request.Day:yyyy-MM-dd}");
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

            _employeeScheduleService.UpdateEmployeeSchedule(existing);

            await _employeeScheduleService.SaveEmployeeSchedule();

            return Ok();
        }


        [HttpDelete]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> DeleteSchedule(string username, DateTime day)
        {
            var existing = (await _employeeScheduleService.GetAllEmployeeSchedules())
                .FirstOrDefault(s => s.Employee.UserName == username && s.Day.Date == day.Date);

            if (existing == null)
                return NotFound();

            _employeeScheduleService.DeleteEmployeeSchedule(existing);

            await _employeeScheduleService.SaveEmployeeSchedule();

            return NoContent();
        }

    }
}
