using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ServiceDTOs
{
    public class ServiceDtoResponse
    {
        public int ServiceId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public string? ImagePath { get; set; }
        public bool Status { get; set; }
    }
}
