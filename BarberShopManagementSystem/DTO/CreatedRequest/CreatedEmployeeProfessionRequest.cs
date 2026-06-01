using System.ComponentModel.DataAnnotations;

namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedEmployeeProfessionRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public Guid ProfessionId { get; set; }
    }
}
