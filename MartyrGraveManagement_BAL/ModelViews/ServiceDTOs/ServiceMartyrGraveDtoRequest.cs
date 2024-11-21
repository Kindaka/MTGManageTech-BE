using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.ModelViews.ServiceDTOs
{
    public class ServiceMartyrGraveDtoRequest
    {
        public int serviceId {  get; set; }
        public int martyrId { get; set; }
    }
}
