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
    public class ExcelController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        public ExcelController(IWebHostEnvironment env, ILogger<ExcelController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public ActionResult Get()
        {
            string username = HttpContext.Session.GetString("userFirstName");
            return Ok("Login User " + username);
            //return Ok("Order API Call");
        }

        [HttpPost]
        [Route("UploadOrder/{userid}")]
        public async Task<IActionResult> UploadOrder(int userid ,IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\order\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\order\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;                   
                    var uploads = Path.Combine("uploads/order/" + userid.ToString ()  +"/orderexcel.xlsx");
                    path = Path.Combine(_env.WebRootPath, uploads);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return Ok(new { status = 200, message = filename ,path= uploads });
                }
                else
                {
                    return Ok(new { status = 400, message = filename });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("UploadLineTarget/{userid}")]
        public async Task<IActionResult> UploadLineTarget(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\linetarget\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\linetarget\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/linetarget/" + userid.ToString() + "/linetargetexcel.xlsx");
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
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("UploadLineBooking/{userid}")]
        public async Task<IActionResult> UploadLineBooking(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\linebooking\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\linebooking\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/linebooking/" + userid.ToString() + "/linebookingexcel.xlsx");
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
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("UploadOB/{userid}")]
        public async Task<IActionResult> UploadOB(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\ob\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\ob\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/ob/" + userid.ToString() + "/ob.xlsx");
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
                return BadRequest(ex.Message);
            }
        }

    }
}
