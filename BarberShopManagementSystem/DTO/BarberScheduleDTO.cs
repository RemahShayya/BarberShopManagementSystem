namespace BarberShopManagementSystem.API.DTO
{
    public class BarberScheduleDTO
    {
        public Guid Id { get; set; }
        public string BarberId { get; set; }
        public string BarberName { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
