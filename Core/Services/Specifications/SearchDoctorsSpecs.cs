using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    class SearchDoctorsSpecs : BaseSpecifications<Doctor, int>
    {
        public SearchDoctorsSpecs(string searchWord) : base(d => d.FullName.Contains(searchWord) || d.Specialization.Name.Contains(searchWord))
        {
            AddInclude(d => d.Specialization);
        }
    }
}
