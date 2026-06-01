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
    public class ServiceController : ControllerBase
    {
        private readonly IServicesService _servicesService;
        private readonly IProfessionService _professionService;
        private readonly IMapper _mapper;

        public ServiceController(
            IServicesService servicesService,
            IProfessionService professionService,
            IMapper mapper)
        {
            _servicesService = servicesService;
            _professionService = professionService;
            _mapper = mapper;
        }



        [HttpGet]
        [Authorize(Roles = "Employee, Admin, Customer")]
        public async Task<ActionResult<IEnumerable<ServiceDTO>>> GetAllServices([FromQuery] Guid? professionId)
        {
            IEnumerable<Service> services;

            if (professionId.HasValue)
            {
                var profession = await _professionService.GetProfessionById(professionId.Value);
                if (profession is null)
                    return NotFound($"Profession with ID '{professionId}' was not found.");

                services = await _servicesService.GetServicesByProfession(
                    professionId.Value,
                    s => s.Profession);
            }
            else
            {
                services = await _servicesService.GetAllServicesWithIncludes(s => s.Profession);
            }

            var result = _mapper.Map<IEnumerable<ServiceDTO>>(services);
            return Ok(result);
        }


        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Employee, Admin, Customer")]
        public async Task<ActionResult<ServiceDTO>> GetServiceById(Guid id)
        {
            var service = await _servicesService.GetServiceWithIncludes(
                filter: s => s.Id == id,
                s => s.Profession);

            if (service is null)
                return NotFound("Service not found.");

            var result = _mapper.Map<ServiceDTO>(service);
            return Ok(result);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceDTO>> AddService([FromBody] CreatedServiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var profession = await _professionService.GetProfessionById(request.ProfessionId);
            if (profession is null)
                return NotFound($"Profession with ID '{request.ProfessionId}' was not found.");

            var service = _mapper.Map<Service>(request);
            service.Id = Guid.NewGuid();

            await _servicesService.AddService(service);
            await _servicesService.Save();

            var created = await _servicesService.GetServiceWithIncludes(
                filter: s => s.Id == service.Id,
                s => s.Profession);

            var result = _mapper.Map<ServiceDTO>(created);
            return CreatedAtAction(nameof(GetServiceById), new { id = result.Id }, result);
        }


        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ServiceDTO>> UpdateService(Guid id, [FromBody] CreatedServiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _servicesService.GetServiceById(id);
            if (existing is null)
                return NotFound("Service not found.");

            if (request.ProfessionId != existing.ProfessionId)
            {
                var profession = await _professionService.GetProfessionById(request.ProfessionId);
                if (profession is null)
                    return NotFound($"Profession with ID '{request.ProfessionId}' was not found.");
            }

            _mapper.Map(request, existing);
            await _servicesService.UpdateService(existing);
            await _servicesService.Save();

            var updated = await _servicesService.GetServiceWithIncludes(
                filter: s => s.Id == id,
                s => s.Profession);

            var result = _mapper.Map<ServiceDTO>(updated);
            return Ok(result);
        }



        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var service = await _servicesService.GetServiceById(id);
            if (service is null)
                return NotFound("Service not found.");

            await _servicesService.DeleteService(id);
            await _servicesService.Save();

            return NoContent();
        }
    }
}
