using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using OFIQ.RestApi.Attributes;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace OFIQ.RestApi.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void ValidateImageFile_ReturnsBadRequest_WhenNoFile()
        {
            // Arrange
            var attribute = new ValidateImageFileAttribute();
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()
            );
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?>(),
                new Mock<Controller>().Object
            );

            // Act
            attribute.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
        }

        [Fact]
        public void ValidateImageFile_ReturnsBadRequest_WhenInvalidExtension()
        {
            // Arrange
            var attribute = new ValidateImageFileAttribute();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns("test.txt");
            fileMock.Setup(_ => _.Length).Returns(100);

            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()
            );
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object?> { { "file", fileMock.Object } },
                new Mock<Controller>().Object
            );

            // Act
            attribute.OnActionExecuting(actionExecutingContext);

            // Assert
            var result = Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
            Assert.Contains("Invalid file extension", result.Value?.ToString());
        }
    }
}
