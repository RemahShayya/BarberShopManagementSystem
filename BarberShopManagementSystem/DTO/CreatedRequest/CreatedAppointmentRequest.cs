namespace BarberShopManagementSystem.API.DTO.CreatedRequest
{
    public class CreatedAppointmentRequest
    {
        public class CreateAppointmentRequest
        {
            public string BarberUsername { get; set; }
            public Guid ServiceId { get; set; }
            public DateOnly Day { get; set; }
            public TimeOnly StartTime { get; set; }
        }

    }
}
