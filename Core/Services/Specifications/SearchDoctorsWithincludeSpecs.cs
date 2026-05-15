using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class SearchDoctorsWithincludeSpecs : BaseSpecifications<Doctor, int>
    {
        public SearchDoctorsWithincludeSpecs(string? search)
            : base(d => string.IsNullOrEmpty(search) || d.FullName.Contains(search))
        {
            AddInclude(d => d.Specialization);
            AddInclude(d => d.Availabilities);
            AddInclude(d => d.Reviews);
        }
    }
}
