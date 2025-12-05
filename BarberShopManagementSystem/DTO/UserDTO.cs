namespace BarberShopManagementSystem.API.DTO
{
    public class UserDTO
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string JWT { get; set; } = string.Empty;
    }
}
