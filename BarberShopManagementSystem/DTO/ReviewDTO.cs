namespace BarberShopManagementSystem.API.DTO
{
    public class ReviewDTO
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string BarberId { get; set; }
        public string BarberName { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
