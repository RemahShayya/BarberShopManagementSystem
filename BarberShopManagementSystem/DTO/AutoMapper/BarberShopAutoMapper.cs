using AutoMapper;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.Data.Entities;

namespace BarberShopManagementSystem.API.DTO.BarberShopAutoMapper
{
    public class BarberShopAutoMapper : Profile
    {
        public BarberShopAutoMapper()
        {
            CreateMap<CreatedBarberScheduleRequest, BarberSchedule>()
                .ForMember(dest => dest.StartHour, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndHour, opt => opt.MapFrom(src => src.EndTime))
                .ForPath(dest => dest.Barber.Id, opt => opt.MapFrom(src => src.BarberId))
                .ForPath(dest => dest.Barber.FirstName, opt => opt.MapFrom(src => src.BarberFirstName))
                .ForPath(dest => dest.Barber.LastName, opt => opt.MapFrom(src => src.BarberLastName));

            CreateMap<BarberSchedule, BarberScheduleDTO>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartHour))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndHour))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber.FirstName + "" + src.Barber.LastName))
                .ForMember(dest => dest.BarberId, opt => opt.MapFrom(src => src.Barber.Id));

            CreateMap<CreatedAppointmentRequest, Appointment>()
                .ForPath(dest => dest.Customer.FirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
                .ForPath(dest => dest.Customer.LastName, opt => opt.MapFrom(src => src.CustomerLastName))
                .ForPath(dest => dest.Barber.FirstName, opt => opt.MapFrom(src => src.BarberFirstName))
                .ForPath(dest => dest.Barber.LastName, opt => opt.MapFrom(src => src.BarberLastName))
                .ForPath(dest => dest.Service.Name, opt => opt.MapFrom(src => src.ServiceName))
                .ForPath(dest => dest.Service.Price, opt => opt.MapFrom(src => src.ServicePrice));


            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FirstName + " " + src.Customer.LastName))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber.FirstName + " " + src.Barber.LastName))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Customer.Id))
                .ForMember(dest => dest.BarberId, opt => opt.MapFrom(src => src.Barber.Id))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.Service.Id))
                .ForMember(dest => dest.ServicePrice, opt => opt.MapFrom(src => src.Service.Price));

            CreateMap<CreatedServiceRequest, Service>()
                .ForMember(dest => dest.DurationInMinutes, opt => opt.MapFrom(src => (int)src.DurationInMinutes.TotalMinutes));

            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.DurationInMinutes)));

            CreateMap<CreatedReviewRequest, Review>()
                .ForPath(dest => dest.Customer.FirstName, opt => opt.MapFrom(src => src.CustomerFirstName))
                .ForPath(dest => dest.Customer.LastName, opt => opt.MapFrom(src => src.CustomerLastName))
                .ForPath(dest => dest.Barber.FirstName, opt => opt.MapFrom(src => src.BarberFirstName))
                .ForPath(dest => dest.Barber.LastName, opt => opt.MapFrom(src => src.BarberLastName));

            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FirstName + " " + src.Customer.LastName))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber.FirstName + " " + src.Barber.LastName));
        }
    }
}
