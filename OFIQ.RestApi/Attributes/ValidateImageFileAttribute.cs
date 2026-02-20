using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OFIQ.RestApi.Attributes
{
    public class ValidateImageFileAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var file = context.ActionArguments.Values.OfType<IFormFile>().FirstOrDefault();

            if (file == null || file.Length == 0)
            {
                context.Result = new BadRequestObjectResult("No file uploaded or file is empty.");
                return;
            }

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
            {
                context.Result = new BadRequestObjectResult($"Invalid file extension. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
