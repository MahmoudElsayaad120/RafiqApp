using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Rafiq.Api.Services.Abstractions;
using Services.Abstractions;
using Shared;

namespace Services
{
    public class ServiceManager(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : IServiceManager
    {
        public IDoctorService DoctorService { get; } = new DoctorService(unitOfWork, mapper, configuration);

        public IAvailabilityService AvailabilityService { get; }

        public IAppointmentService AppointmentService { get; } 
        
    }
}
