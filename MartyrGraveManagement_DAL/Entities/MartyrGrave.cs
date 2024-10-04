using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class MartyrGrave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MartyrId { get; set; }
        public int AreaId { get; set; }
        public string MartyrCode { get; set; }
        public string? CustomerCode { get; set; }
        public int RowNumber { get; set; }
        public int MartyrNumber { get; set; }
        public int AreaNumber { get; set; }
        public bool Status { get; set; }

        public Area? Area { get; set; }
        public IEnumerable<GraveImage>? GraveImages { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
        public IEnumerable<MartyrGraveInformation>? MartyrGraveInformations { get; set; }
        public IEnumerable<WeeklyReportGrave>? WeeklyReportGraves { get; set; }
        public IEnumerable<CartItem>? CartItems { get; set; }

    }
}
