using System.ComponentModel.DataAnnotations;

namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedProfessionRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string professionName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? professionDescription { get; set; }
    }
}
