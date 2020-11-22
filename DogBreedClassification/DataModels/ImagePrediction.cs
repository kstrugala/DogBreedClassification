using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace DogBreedClassification.Shared.DataModels
{
    public class ImagePrediction
    {
        [ColumnName("Score")]
        public float[] Score;

        [ColumnName("PredictedLabel")]
        public string PredictedLabel;
    }
}
