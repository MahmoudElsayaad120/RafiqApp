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


            CreateMap<DoctorAvailability, AvailabilityDto>();


            CreateMap<Appointment, AppointmentDto>();


            CreateMap<Doctor, DoctorProfileDto>()
                 .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));


       




        }
    }
}
