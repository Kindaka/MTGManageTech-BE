using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MLModels
{
    public class ServiceData
    {
        public int ServiceId { get; set; }
        public float Frequency { get; set; }
        public float AveragePrice { get; set; }
        public float Quantity { get; set; }
        public float RecentOrdersCount { get; set; }

        //test
        public float TrendScore { get; set; }

    }
}
