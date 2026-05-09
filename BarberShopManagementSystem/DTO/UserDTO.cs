namespace BarberShopManagementSystem.API.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string TimeZoneId { get; set; }
        public string PhoneNumber { get; set; }
        public string JWT { get; set; } = string.Empty;

    }
}
