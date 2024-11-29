using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.NotificationDTOs
{
    public class NotificationAccountDto
    {
        public int AccountId { get; set; }
        public string? FullName { get; set; }

        public string? AvatarPath { get; set; }

        public bool Status { get; set; }
    }
}
