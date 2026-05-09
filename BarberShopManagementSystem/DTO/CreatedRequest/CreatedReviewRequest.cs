using System.ComponentModel.DataAnnotations;

namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedReviewRequest
    {
        public Guid AppointmentId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string Token { get; set; }
    }
}
