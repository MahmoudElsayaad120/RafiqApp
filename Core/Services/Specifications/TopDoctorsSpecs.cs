using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Services.Specifications
{
    public class TopDoctorsSpecs : BaseSpecifications<Doctor, int>
    {
        public TopDoctorsSpecs(int pageIndex, int pageSize) : base(null)
        {
            AddOrderByDescending(d => d.Rate);

            ApplyPagination(pageIndex, pageSize);
        }

    }
}
