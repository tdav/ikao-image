using Microsoft.AspNetCore.Mvc;
using OFIQ.RestApi.Services;
using OFIQ.RestApi.Models;
using OFIQ.RestApi.Attributes;
using ikao;

namespace OFIQ.RestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityController : ControllerBase
    {
        private readonly IOFIQService _ofiqService;

        public QualityController(IOFIQService ofiqService)
        {
            _ofiqService = ofiqService;
        }

        [HttpPost("scalar")]
        [ValidateImageFile]
        public async Task<IActionResult> GetScalarQuality(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            double score = await _ofiqService.GetScalarQualityAsync(stream);
            return Ok(new ScalarQualityResponse { ScalarQuality = score });
        }

        [HttpPost("vector")]
        [ValidateImageFile]
        public async Task<IActionResult> GetVectorQuality(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var (assessments, bbox) = await _ofiqService.GetVectorQualityAsync(stream);

            var response = new VectorQualityResponse
            {
                BoundingBox = new BoundingBoxDto { X = bbox.X, Y = bbox.Y, Width = bbox.Width, Height = bbox.Height },
                Assessments = assessments.Select(a => new AssessmentResultDto
                {
                    MeasureName = Enum.IsDefined(typeof(NativeInvoke.QualityMeasure), a.Measure)
                        ? ((NativeInvoke.QualityMeasure)a.Measure).ToString()
                        : $"Unknown(0x{a.Measure:X})",
                    RawScore = a.RawScore,
                    ScalarScore = a.Scalar,
                    Code = a.Code
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost("preprocessing")]
        [ValidateImageFile]
        public async Task<IActionResult> GetPreprocessingResults(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var (assessments, bbox, landmarks) = await _ofiqService.GetPreprocessingResultsAsync(stream);

            var response = new PreprocessingQualityResponse
            {
                BoundingBox = new BoundingBoxDto { X = bbox.X, Y = bbox.Y, Width = bbox.Width, Height = bbox.Height },
                Assessments = assessments.Select(a => new AssessmentResultDto
                {
                    MeasureName = Enum.IsDefined(typeof(NativeInvoke.QualityMeasure), a.Measure)
                        ? ((NativeInvoke.QualityMeasure)a.Measure).ToString()
                        : $"Unknown(0x{a.Measure:X})",
                    RawScore = a.RawScore,
                    ScalarScore = a.Scalar,
                    Code = a.Code
                }).ToList(),
                Landmarks = landmarks.Select(l => new LandmarkDto { X = l.X, Y = l.Y }).ToList()
            };

            return Ok(response);
        }
    }
}
