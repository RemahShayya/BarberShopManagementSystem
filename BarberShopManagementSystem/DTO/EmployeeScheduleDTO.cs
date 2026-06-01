namespace BarberShopManagementSystem.API.DTO
{
    public class EmployeeScheduleDTO
    {
        public Guid Id { get; set; }
        public string EmployeeName { get; set; }
        public string Username { get; set; }
        public DateTime Day { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public bool IsDayOff { get; set; }
    }
}
