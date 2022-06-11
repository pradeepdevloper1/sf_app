using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WFX.Web.Controllers
{
    [ApiController]
    [Route("Upload/[controller]")]
    public class ImageController : ControllerBase
    {
         private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        public ImageController(IWebHostEnvironment env, ILogger<ImageController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _env = env;
        }
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        [HttpPost]
        [Route("UploadImage/{IMGNo}")]
        public async Task<IActionResult> UploadImage(int IMGNo, string SONo, IFormFile file)
        {
            try
            {
                var path = "";
                string filename = "";
                string pathfordb = "";
                string webServerURL = _configuration.GetSection("AppSettings:webServerURL").Value;
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\poimages\\" + SONo))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\soimages\\" + SONo);
                }
                if (file != null)
                {
                    filename = file.FileName;
                    String timeStamp = GetTimestamp(DateTime.Now);
                    String filenamewithoutext = filename.Split('.')[0];
                    String ext = filename.Split('.')[1];
                    filenamewithoutext = filenamewithoutext + "_" + timeStamp;
                    filename = filenamewithoutext + "." + ext;
                    var  uploads = Path.Combine("uploads/soimages/" + SONo + "/" + filename);
                    path = Path.Combine(_env.WebRootPath, uploads);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                      await file.CopyToAsync(fileStream);
                    }
                    pathfordb = Path.Combine(webServerURL, uploads);
                    return Ok(new { status = 200, filename = filename, path = pathfordb });
                }
                else
                {
                    return Ok(new { status = 400, message = filename });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("ChangeImage/{POImageID}")]
        public async Task<IActionResult> ChangeImage(long POImageID,string SONo, IFormFile newfile,string PreviousImageName)
        {
            try
            {
                var path = "";
                string filename = "";
                string pathfordb = "";
                string webServerURL = _configuration.GetSection("AppSettings:webServerURL").Value;
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\soimages\\" + SONo))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\soimages\\" + SONo);
                }
                var previousimagepath = _env.WebRootPath + "\\uploads\\soimages\\" + SONo + "/" + PreviousImageName;
                FileInfo previousFile = new FileInfo(previousimagepath);
                if (previousFile.Exists)
                {
                    previousFile.Delete();
                }
                if (newfile != null)
                {
                    filename = newfile.FileName;
                    String timeStamp = GetTimestamp(DateTime.Now);
                    String filenamewithoutext = filename.Split('.')[0];
                    String ext = filename.Split('.')[1];
                    filenamewithoutext = filenamewithoutext +"_"+ timeStamp;
                    filename = filenamewithoutext + "." + ext;
                    var uploads = Path.Combine("uploads/soimages/" + SONo + "/" + filename);
                    path = Path.Combine(_env.WebRootPath, uploads);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                       await newfile.CopyToAsync(fileStream);
                    }
                    pathfordb = Path.Combine(webServerURL, uploads);
                    return Ok(new { status = 200, filename = filename, path = pathfordb });
                }
                else
                {
                    return Ok(new { status = 400, message = filename });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("DeleteImage")]
        public IActionResult DeleteImage(string SONo, string FileName)
        {
            string res = "";
            try
            {
                string webServerURL = _configuration.GetSection("AppSettings:webServerURL").Value;
                var path = _env.WebRootPath + "\\uploads\\soimages\\" + SONo + "/" + FileName;
                FileInfo File = new FileInfo(path);
                if (File.Exists)
                {
                    File.Delete();
                    res="File deleted.";
                }
                else
                {
                    res="File does not Exist.";
                }
                return Ok(new { status = 200, message = res }); ;
            }
            catch (Exception ex)
            {
                res = ex.Message.ToString();
                return Ok(new { status = 400, message = res }); ;
            }         
        }

        [HttpPost]
        [Route("UploadOrganisationLogo")]
        public async Task<IActionResult> UploadOrganisationLogo(IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\organisationlogo"))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\organisationlogo");
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    //var uploads = Path.Combine("uploads/poimages/" + pono + "/1.jpg");
                    var uploads = Path.Combine("uploads/organisationlogo/" + file.FileName);
                    path = Path.Combine(_env.WebRootPath, uploads);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return Ok(new { status = 200, message = filename, path = uploads });
                }
                else
                {
                    return Ok(new { status = 400, message = filename });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
    }
}
