using AutoMapper;
using BarberShopManagementSystem.API.DTO.CreatedRequest;
using BarberShopManagementSystem.Data.Entities;
using System;

namespace BarberShopManagementSystem.API.DTO.BarberShopAutoMapper
{
    public class BarberShopAutoMapper : Profile
    {
        public BarberShopAutoMapper()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.JWT, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<CreatedBarberScheduleRequest, BarberSchedule>()
                .ForMember(dest => dest.StartHour, opt => opt.MapFrom(src => src.StartHour))
                .ForMember(dest => dest.EndHour, opt => opt.MapFrom(src => src.EndHour));

            CreateMap<BarberSchedule, BarberScheduleDTO>()
                .ForMember(dest => dest.StartHour, opt => opt.MapFrom(src => src.StartHour))
                .ForMember(dest => dest.EndHour, opt => opt.MapFrom(src => src.EndHour))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber.FirstName + " " + src.Barber.LastName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Barber.UserName));

            CreateMap<CreatedAppointmentRequest, Appointment>();

            CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FirstName + " " + src.Customer.LastName))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber.FirstName + " " + src.Barber.LastName))
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
                .ForMember(dest => dest.ServicePrice, opt => opt.MapFrom(src => src.Service.Price))
                .ForMember(dest => dest.AppointmentDuration, opt => opt.MapFrom(src => src.Service.DurationInMinutes))
                .ForMember(dest => dest.AppointmentStart, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.AppointmentEndTime, opt => opt.MapFrom(src => src.EndTime));

            CreateMap<CreatedServiceRequest, Service>()
                .ForMember(dest => dest.DurationInMinutes, opt => opt.MapFrom(src => TimeSpan.FromMinutes(src.DurationInMinutes)));

            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.DurationInMinutes));


            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FirstName + " " + src.Customer.LastName))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src => src.Barber.FirstName + " " + src.Barber.LastName));

            CreateMap<CreatedReviewRequest, Review>()
    .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
    .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
    .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
    .ForMember(dest => dest.CustomerId, opt => opt.Ignore()) // Will be set from logged-in user
    .ForMember(dest => dest.Customer, opt => opt.Ignore())
    .ForMember(dest => dest.BarberId, opt => opt.Ignore()) // Will be set from Appointment data
    .ForMember(dest => dest.Barber, opt => opt.Ignore())
    .ForMember(dest => dest.Appointment, opt => opt.Ignore())
    .ForMember(dest => dest.Id, opt => opt.Ignore()); // Auto-gene
        }
    }
}
