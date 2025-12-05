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
    public class ServiceController : ControllerBase
    {
        private readonly IServicesService _servicesService;
        private readonly IMapper _mapper;
        public ServiceController(IServicesService servicesService, IMapper mapper)
        {
            _servicesService = servicesService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDTO>>> GetAllServices()
        {
            var services = await _servicesService.GetAllServices();
            var servicesDTO = _mapper.Map<IEnumerable<ServiceDTO>>(services);
            return Ok(servicesDTO);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDTO>> GetServiceById(Guid id)
        {
            var service = await _servicesService.GetServiceById(id);
            if (service == null)
            {
                return NotFound("Service Not Found");
            }
            var serviceDTO = _mapper.Map<ServiceDTO>(service);
            return Ok(serviceDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceDTO>> AddService([FromBody] CreatedServiceRequest request)
        {
            var addedService = _mapper.Map<Service>(request);
            if (addedService == null)
            {
                return BadRequest("Invalid Service Data");
            }
            var createdService = await _servicesService.AddService(addedService);
            await _servicesService.SaveService(createdService);
            var createdServiceDTO = _mapper.Map<ServiceDTO>(createdService);
            return CreatedAtAction(nameof(GetServiceById), new { id = createdServiceDTO.Id }, createdServiceDTO);
        }

        [HttpPut]
        public async Task<ActionResult<ServiceDTO>> UpdateService(Guid id, [FromBody] CreatedServiceRequest request)
        {
            var existingService = await _servicesService.GetServiceById(id);
            if (existingService == null)
            {
                return NotFound("Service Not Found");
            }
            _mapper.Map(request, existingService);
            var success = _servicesService.UpdateService(existingService);
            if (!success) return BadRequest("Update failed");
            await _servicesService.SaveService(existingService); 
            var updatedServiceDTO = _mapper.Map<ServiceDTO>(existingService);
            return Ok(updatedServiceDTO);

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var service = await _servicesService.GetServiceById(id);
            if (service == null)
            {
                return NotFound("Service Not Found");
            }
            _servicesService.DeleteService(service);
            await _servicesService.SaveService(service);
            return Ok("Service deleted successfuly");
        }
    }
}
