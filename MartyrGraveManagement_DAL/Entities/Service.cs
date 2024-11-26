using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServiceId { get; set; }
        public int CategoryId { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string ServiceName { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
        public bool isScheduleService { get; set; }
        public int RecurringType { get; set; }
        public bool Status { get; set; }

        public ServiceCategory? ServiceCategory { get; set; }
        public IEnumerable<Material_Service>? Material_Services { get; set; }
        public IEnumerable<OrderDetail>? OrderDetails { get; set; }
        public IEnumerable<CartItemCustomer>? CartItems { get; set; }
        public IEnumerable<GraveService>? GraveServices { get; set; }
        public IEnumerable<Service_Schedule>? ServiceSchedules { get; set; }
    }
}
