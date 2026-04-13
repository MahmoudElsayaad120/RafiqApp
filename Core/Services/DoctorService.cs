using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Rafiq.Api.DTOs;
using Rafiq.Api.Services;
using Microsoft.EntityFrameworkCore;
using Services.Specifications;

namespace Rafiq.Api.Services
{
    public class DoctorService(IUnitOfWork unitOfWork , IMapper mapper) : IDoctorService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        
        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string? specialization = null, int pageIndex = 1, int pageSize = 5)
        {
            var specification = new BaseSpecifications<Doctor, int>(null);

            var doctors = await  unitOfWork.GetRepository<Doctor, int>().GetAllAsyns(specification);
           var result = mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return result;
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
            var specification = new DoctorSpecifications(id);
           var doctor = await unitOfWork.GetRepository<Doctor, int>().GetAsyns(specification);
            if (doctor is null) return null;

           var result = mapper.Map<DoctorDto>(doctor);
            return result;
        }
    }
}
