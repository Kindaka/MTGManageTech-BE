using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MLModels
{
    public class PerformancePrediction
    {
        public float QualityScore { get; set; }
        public float TimeManagementScore { get; set; }
        public float InteractionScore { get; set; }
    }
}
