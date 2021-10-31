using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace backend.Formatter
{
    public class PlainTextFormatter : InputFormatter
    {
        public PlainTextFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
        }

		public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
		{
			var request = context.HttpContext.Request;
            using(var reader = new StreamReader(request.Body))
            {
                var content = await reader.ReadToEndAsync();
                return await InputFormatterResult.SuccessAsync(content);
            }
		}

        public override bool CanRead(InputFormatterContext context)
        {
            var ContentType = context.HttpContext.Request.ContentType;
            return ContentType.StartsWith(ContentType);
        }
	}
}