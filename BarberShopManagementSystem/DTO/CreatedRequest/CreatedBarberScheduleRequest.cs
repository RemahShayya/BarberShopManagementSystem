namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedBarberScheduleRequest
    {
        public string BarberId { get; set; }
        public string BarberFirstName { get; set; }
        public string BarberLastName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
