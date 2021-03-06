using System;
using System.IO;
using System.Threading.Tasks;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
	[ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DocsController : ControllerBase
    {
        [HttpPost]
        [Route("Post")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostData([FromForm]DocsModel model)
        {
            if(model.File != null)
            {
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "File");
                if(!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, model.File.FileName);

                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }
            }

            return Ok();
        }

		[HttpDelete]
		[Route("Delete")]
		[Authorize(Roles = "Admin")]
		public ActionResult DeleteData([FromBody]DocsModel model)
		{
            if(model.FileName != null)
            {
                var filePath = Path.Combine($"{Directory.GetCurrentDirectory()}", "File");
                string[] fileDir = Directory.GetFiles(filePath);
                var reqFilePath = Path.Combine($"{Directory.GetCurrentDirectory()}\\File", model.FileName);
                foreach(string file in fileDir)
                {
                    if (reqFilePath == file)
                    {
                        System.IO.File.Delete(reqFilePath);
						return Ok($"File {file} deleted");
                    }
                }
            }
            return BadRequest("Invalid file name");
		}

        [HttpGet]
        [Route("Get")]
        [Authorize(Roles = "User, Admin")]
		public ActionResult GetData()
		{
            var filePath = Path.Combine($"{Directory.GetCurrentDirectory()}", "File");
            string[] fileDir = Directory.GetFiles(filePath);
			return Ok(new {file = fileDir});
		}
	}
}