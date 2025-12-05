using System.ComponentModel.DataAnnotations;

namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedReviewRequest
    {
        public Guid AppointmentId { get; set; }
        public string CustomerId { get; set; }
        public string BarberId { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string BarberFirstName { get; set; }
        public string BarberLastName { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
