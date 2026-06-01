namespace BarberShopManagementSystem.API.DTO
{
    public class ReviewDTO
    {
        public Guid AppointmentId { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
