using System.ComponentModel.DataAnnotations;

namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreateUnassignProfessionRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public Guid ProfessionId { get; set; }
    }
}
