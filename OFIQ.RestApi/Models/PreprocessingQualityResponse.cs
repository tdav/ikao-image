namespace OFIQ.RestApi.Models
{
    public class LandmarkDto
    {
        public short X { get; set; }
        public short Y { get; set; }
    }

    public class PreprocessingQualityResponse : VectorQualityResponse
    {
        public List<LandmarkDto> Landmarks { get; set; } = new();
    }
}
