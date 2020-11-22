namespace DogBreedClassification.Api.Models
{
    public class PredictionResult
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public byte[] Image { get; set; }

        public string PredictedLabel { get; set; }
        public float Probability { get; set; }

        public long PredictionExecutionTime { get; set; }
    }
}
