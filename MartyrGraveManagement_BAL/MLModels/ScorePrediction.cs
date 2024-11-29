using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_BAL.MLModels
{
    public class ScorePrediction
    {
        [ColumnName("Features")]
        public float[] Features { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }

        [ColumnName("PredictedLabel")]
        public float PredictedScore { get; set; }
    }
}

