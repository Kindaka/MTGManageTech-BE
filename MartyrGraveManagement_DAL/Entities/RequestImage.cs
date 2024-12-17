using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class RequestImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestImageId { get; set; }
        public int RequestId { get; set; }
        public string? ImageRequestCustomer { get; set; }
        public DateTime CreateAt { get; set; }

        public RequestCustomer? RequestCustomer { get; set; }
    }
}
