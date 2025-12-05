namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedAppointmentRequest
    {
        public string ServiceName { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string BarberFirstName { get; set; }
        public string BarberLastName { get; set; }
        public decimal ServicePrice { get; set; }
        public DateTime AppointmentStart { get; set; }
        public DateTime AppointmentEnd { get; set; }
        public TimeSpan AppointmentDuration { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
