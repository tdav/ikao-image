using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OFIQ.RestApi.Controllers;
using OFIQ.RestApi.Services;
using OFIQ.RestApi.Models;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace OFIQ.RestApi.Tests
{
    public class QualityControllerTests
    {
        private readonly Mock<IOFIQService> _ofiqServiceMock;
        private readonly QualityController _controller;

        public QualityControllerTests()
        {
            _ofiqServiceMock = new Mock<IOFIQService>();
            // This will fail to compile initially because QualityController doesn't exist
            _controller = new QualityController(_ofiqServiceMock.Object);
        }

        [Fact]
        public async Task GetScalarQuality_ReturnsOkWithScore()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var content = "fake image content";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            _ofiqServiceMock.Setup(s => s.GetScalarQualityAsync(It.IsAny<Stream>()))
                .ReturnsAsync(85.5);

            // Act
            var result = await _controller.GetScalarQuality(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ScalarQualityResponse>(okResult.Value);
            Assert.Equal(85.5, response.ScalarQuality);
        }

        [Fact]
        public async Task GetVectorQuality_ReturnsOkWithVectorData()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.Length).Returns(1);

            var mockAssessments = new ikao.NativeInvoke.BridgeAssessment[]
            {
                new() { Measure = (int)ikao.NativeInvoke.QualityMeasure.UnifiedQualityScore, Scalar = 80.0, RawScore = 0.8 }
            };
            var mockBbox = new ikao.NativeInvoke.BridgeBoundingBox { X = 10, Y = 20, Width = 100, Height = 100 };

            _ofiqServiceMock.Setup(s => s.GetVectorQualityAsync(It.IsAny<Stream>()))
                .ReturnsAsync((mockAssessments, mockBbox));

            // Act
            var result = await _controller.GetVectorQuality(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<VectorQualityResponse>(okResult.Value);
            Assert.Equal(10, response.BoundingBox.X);
            Assert.Single(response.Assessments);
            Assert.Equal("UnifiedQualityScore", response.Assessments[0].MeasureName);
        }
    }
}
