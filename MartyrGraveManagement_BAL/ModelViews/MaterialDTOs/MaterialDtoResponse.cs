using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.MaterialDTOs
{
    public class MaterialDtoResponse
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }
    }
}
