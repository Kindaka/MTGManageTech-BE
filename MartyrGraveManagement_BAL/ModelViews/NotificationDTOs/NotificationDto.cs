using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.NotificationDTOs
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LinkTo { get; set; }
        public bool isRead {  get; set; }
        public bool Status { get; set; }
        public List<NotificationAccountDto> NotificationAccounts { get; set; }
    }

}
