namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedServiceRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TimeSpan DurationInMinutes { get; set; }
    }
}
