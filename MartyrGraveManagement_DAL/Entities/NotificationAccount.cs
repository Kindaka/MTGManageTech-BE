using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class NotificationAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int NotificationId { get; set; }
        public bool isRead { get; set; }
        public bool Status { get; set; }

        // Navigation properties
        public Notification? Notification { get; set; }
        public Account? Account { get; set; }
    }
}
