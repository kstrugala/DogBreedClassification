namespace DogBreedClassification.Api.DTO
{
    public class PredictionResultDetailsDTO
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string PredictedLabel { get; set; }
        public string Image { get; set; }
        public float Probability { get; set; }
        public long PredictionExecutionTime { get; set; }
    }
}
