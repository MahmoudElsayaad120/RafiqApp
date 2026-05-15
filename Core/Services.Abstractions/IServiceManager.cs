using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rafiq.Api.Services.Abstractions;

namespace Services.Abstractions
{
    public interface IServiceManager
    {
         IDoctorService DoctorService { get; }
        IAvailabilityService AvailabilityService { get; }
        IAppointmentService AppointmentService { get; }
    }
}
