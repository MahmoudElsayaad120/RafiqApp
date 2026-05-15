using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Models;
using Rafiq.Api.DTOs;
using Shared;


namespace Services.MappingProfiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, DoctorDto>();
            //.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name))
            //.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));


            CreateMap<DoctorAvailability, AvailabilityDto>();
                //.ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek.ToString()));


            CreateMap<Appointment, AppointmentDto>();
                //.ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.Name))
                //.ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.User.Name));
            CreateMap<Doctor, DoctorProfileDto>()
                 .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));


       




        }
    }
}
