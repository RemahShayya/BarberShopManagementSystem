namespace BarberShopManagementSystem.API.DTO
{
    public class AppointmentDTO
    {
        public Guid Id { get; set; }
        public string CustomerId { get; set; }
        public string BarberId { get; set; }
        public string ServiceId { get; set; }
        public string CustomerName { get; set; }
        public string BarberName { get; set; }
        public string ServiceName { get; set; }
        public DateTime AppointmentStart { get; set; }
        public TimeSpan AppointmentDuration { get; set; }
        public DateTime AppointmentEndTime => AppointmentStart.Add(AppointmentDuration);
        public decimal ServicePrice { get; set; }
    }
}
