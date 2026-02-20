namespace OFIQ.RestApi.Models
{
    public class BoundingBoxDto
    {
        public short X { get; set; }
        public short Y { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }
    }

    public class AssessmentResultDto
    {
        public string MeasureName { get; set; } = string.Empty;
        public double RawScore { get; set; }
        public double ScalarScore { get; set; }
        public int Code { get; set; }
    }

    public class VectorQualityResponse
    {
        public BoundingBoxDto BoundingBox { get; set; } = new();
        public List<AssessmentResultDto> Assessments { get; set; } = new();
    }
}
