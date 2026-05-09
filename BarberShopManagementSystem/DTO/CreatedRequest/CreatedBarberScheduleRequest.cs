using System.ComponentModel.DataAnnotations;

namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedBarberScheduleRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime Day { get; set; }
        public TimeSpan? StartHour { get; set; }

        public TimeSpan? EndHour { get; set; }
        [Required]
        public bool IsDayOff { get; set; } = false;
    }
}
