namespace BarberShopManagementSystem.API.DTO
{
    public class BarberScheduleDTO
    {
        public Guid Id { get; set; }
        public string BarberName { get; set; }
        public string Username { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public bool IsDayOff { get; set; }
    }
}
