using AutoMapper;
using BarberShopManagementSystem.API.DTO;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.API.Services.IServices;
using BarberShopManagementSystem.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarberShopManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionController : ControllerBase
    {
        private readonly IProfessionService _professionService;
        private readonly IMapper _mapper;

        public ProfessionController(IProfessionService professionService, IMapper mapper)
        {
            _professionService = professionService;
            _mapper = mapper;
        }

        // -------------------------------------------------------------------------
        // GET /api/Profession
        // Returns all professions. Accessible publicly (no auth required) so that
        // customers and employees can browse available profession types.
        // -------------------------------------------------------------------------
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var professions = await _professionService.GetAllProfessions();
            var result = _mapper.Map<IEnumerable<ProfessionDto>>(professions);
            return Ok(result);
        }

        // -------------------------------------------------------------------------
        // GET /api/Profession/{id}
        // Returns a single profession by ID. Also publicly accessible.
        // -------------------------------------------------------------------------
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var profession = await _professionService.GetProfessionById(id);

            if (profession is null)
                return NotFound(new { message = $"Profession with ID '{id}' was not found." });

            var result = _mapper.Map<ProfessionDto>(profession);
            return Ok(result);
        }

        // -------------------------------------------------------------------------
        // POST /api/Profession
        // Creates a new profession. Admin only.
        // -------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatedProfessionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var nameAlreadyExists = await _professionService.GetProfessionByName(request.professionName);
            if (nameAlreadyExists.Any())
                return Conflict(new { message = $"A profession named '{request.professionName}' already exists." });

            var profession = _mapper.Map<Profession>(request);
            profession.Id = Guid.NewGuid();

            await _professionService.AddProfession(profession);
            await _professionService.SaveProfession();

            var result = _mapper.Map<ProfessionDto>(profession);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // -------------------------------------------------------------------------
        // PUT /api/Profession/{id}
        // Updates an existing profession. Admin only.
        // -------------------------------------------------------------------------
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreatedProfessionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var profession = await _professionService.GetProfessionById(id);
            if (profession is null)
                return NotFound(new { message = $"Profession with ID '{id}' was not found." });

            // Only check for name conflict when the name is actually being changed.
            bool nameIsChanging = !string.Equals(
                profession.Name,
                request.professionName,
                StringComparison.OrdinalIgnoreCase);

            if (nameIsChanging)
            {
                var nameAlreadyExists = await _professionService.GetProfessionByName(request.professionName);
                if (nameAlreadyExists.Any())
                    return Conflict(new { message = $"A profession named '{request.professionName}' already exists." });
            }

            _mapper.Map(request, profession);
            await _professionService.UpdateProfession(profession);
            await _professionService.SaveProfession();


            var result = _mapper.Map<ProfessionDto>(profession);
            return Ok(result);
        }

        // -------------------------------------------------------------------------
        // DELETE /api/Profession/{id}
        // Deletes a profession. Admin only.
        // Blocked if any services or employees are still linked to it.
        // -------------------------------------------------------------------------
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var profession = await _professionService.GetProfessionById(id);
            if (profession is null)
                return NotFound(new { message = $"Profession with ID '{id}' was not found." });

            bool hasLinkedServices = await _professionService.HasLinkedServices(id);
            if (hasLinkedServices)
                return Conflict(new
                {
                    message = "Cannot delete this profession because one or more services are linked to it. " +
                              "Reassign or delete those services first."
                });

            bool hasLinkedEmployees = await _professionService.HasLinkedEmployees(id);
            if (hasLinkedEmployees)
                return Conflict(new
                {
                    message = "Cannot delete this profession because one or more employees are assigned to it. " +
                              "Remove those assignments first."
                });

            bool deleted = await _professionService.DeleteProfession(profession.Id);
            await _professionService.SaveProfession();


            return NoContent();
        }
    }
}
