namespace BarberShopManagementSystem.API.DTO
{
    public class ProfessionDto
    {
        public Guid Id { get; set; }
        public string professionName { get; set; } = string.Empty;
        public string? professionDescription { get; set; }
    }
}
