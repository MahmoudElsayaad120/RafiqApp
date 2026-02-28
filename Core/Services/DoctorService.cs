using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Contracts;
using Domain.Models;
using Rafeeq.Api.DTOs;
using Rafeeq.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class DoctorService(IUnitOfWork unitOfWork , IMapper mapper) : IDoctorService
    {
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync(string? specialization = null)
        {
          var doctors = await  unitOfWork.GetRepository<Doctor, int>().GetAllAsyns();
           var result = mapper.Map<IEnumerable<DoctorDto>>(doctors);
            return result;
        }

        public async Task<DoctorDto?> GetDoctorByIdAsync(int id)
        {
           var doctor = await unitOfWork.GetRepository<Doctor, int>().GetAsyns(id);
            if (doctor is null) return null;

           var result = mapper.Map<DoctorDto>(doctor);
            return result;
        }
    }
}
