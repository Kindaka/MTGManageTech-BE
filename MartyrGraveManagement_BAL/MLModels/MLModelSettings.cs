using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MLModels
{
    public class MLModelSettings
    {
        public string ModelDirectory { get; set; }
        public string QualityModelPath { get; set; }
        public string TimeModelPath { get; set; }
        public string InteractionModelPath { get; set; }
    }
}
