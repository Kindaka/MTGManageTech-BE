using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ServiceDTOs
{
    public class ServiceDtoRequest
    {
        public int CategoryId { get; set; }
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
    }
}
