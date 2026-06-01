using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeProfessionController : ControllerBase
    {
        private readonly IEmployeeProfessionService _employeeProfessionService;
        private readonly IProfessionService _professionService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public EmployeeProfessionController(
            IEmployeeProfessionService employeeProfessionService,
            IProfessionService professionService,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _employeeProfessionService = employeeProfessionService;
            _professionService = professionService;
            _userManager = userManager;
            _mapper = mapper;
        }


        [HttpGet("employee/{username}")]
        public async Task<IActionResult> GetByEmployee(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return NotFound($"Employee with username '{username}' was not found.");

            var isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
            if (!isEmployee)
                return BadRequest("The specified user is not an employee.");

            var requesterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && requesterId != user.Id)
                return Forbid();

            var assignments = await GetAssignmentsByEmployeeId(user.Id);
            var result = _mapper.Map<IEnumerable<EmployeeProfessionResponseDto>>(assignments);
            return Ok(result);
        }


        [HttpGet("profession/{professionId}")]
        public async Task<IActionResult> GetByProfession(Guid professionId)
        {
            var profession = await _professionService.GetProfessionById(professionId);
            if (profession is null)
                return NotFound($"Profession with ID '{professionId}' was not found.");

            var assignments = await GetAssignmentsByProfessionId(professionId);
            var result = _mapper.Map<IEnumerable<EmployeeProfessionResponseDto>>(assignments);
            return Ok(result);
        }

        // -------------------------------------------------------------------------
        // POST /api/EmployeeProfession/assign
        // Assigns a profession to an employee. Admin only.
        // -------------------------------------------------------------------------
        [HttpPost("assign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Assign([FromBody] CreatedEmployeeProfessionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
                return NotFound($"Employee with username '{request.Username}' was not found.");

            var isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
            if (!isEmployee)
                return BadRequest("The specified user is not an employee.");

            var profession = await _professionService.GetProfessionById(request.ProfessionId);
            if (profession is null)
                return NotFound($"Profession with ID '{request.ProfessionId}' was not found.");

            var alreadyAssigned = await AssignmentExists(user.Id, request.ProfessionId);
            if (alreadyAssigned)
                return Conflict($"'{request.Username}' is already assigned to profession '{profession.Name}'.");

            var assignment = new EmployeeProfession
            {
                EmployeeId = user.Id,
                ProfessionId = request.ProfessionId
            };

            await _employeeProfessionService.Add(assignment);
            await _employeeProfessionService.Save();

            var result = _mapper.Map<EmployeeProfessionResponseDto>(assignment);
            return CreatedAtAction(nameof(GetByEmployee), new { username = request.Username }, result);
        }


        [HttpDelete("unassign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unassign([FromBody] CreateUnassignProfessionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
                return NotFound($"Employee with username '{request.Username}' was not found.");

            var isEmployee = await _userManager.IsInRoleAsync(user, "Employee");
            if (!isEmployee)
                return BadRequest("The specified user is not an employee.");

            var assignment = await GetAssignment(user.Id, request.ProfessionId);
            if (assignment is null)
                return NotFound(
                    $"No assignment found for employee '{request.Username}' " +
                    $"and profession '{request.ProfessionId}'.");

            await _employeeProfessionService.Delete(assignment.Id);
            await _employeeProfessionService.Save();

            return NoContent();
        }

        // =========================================================================
        // Private query helpers — composition lives here, not in the service.
        // All use Query() so EF Core translates them to SQL rather than loading
        // the full table into memory.
        // =========================================================================
        #region
        private async Task<List<EmployeeProfession>> GetAssignmentsByEmployeeId(string employeeId) =>
            await _employeeProfessionService.Query()
                .Where(ep => ep.EmployeeId == employeeId)
                .Include(ep => ep.Profession)
                .ToListAsync();

        private async Task<List<EmployeeProfession>> GetAssignmentsByProfessionId(Guid professionId) =>
            await _employeeProfessionService.Query()
                .Where(ep => ep.ProfessionId == professionId)
                .Include(ep => ep.Employee)
                .ToListAsync();

        private async Task<bool> AssignmentExists(string employeeId, Guid professionId) =>
            await _employeeProfessionService.Query()
                .AnyAsync(ep => ep.EmployeeId == employeeId && ep.ProfessionId == professionId);

        private async Task<EmployeeProfession?> GetAssignment(string employeeId, Guid professionId) =>
            await _employeeProfessionService.Query()
                .FirstOrDefaultAsync(ep => ep.EmployeeId == employeeId && ep.ProfessionId == professionId);

        #endregion
    }
}
