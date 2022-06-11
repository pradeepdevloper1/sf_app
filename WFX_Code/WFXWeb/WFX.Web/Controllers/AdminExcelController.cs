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
    public class AdminExcelController : ControllerBase
    {
         private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        public AdminExcelController(IWebHostEnvironment env, ILogger<AdminExcelController> logger, IConfiguration configuration)
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
        [Route("UploadProduct/{userid}")]
        public async Task<IActionResult> UploadProduct(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\product\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\product\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/product/" + userid.ToString() + "/product.xlsx");
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
        [Route("UploadProductFit/{userid}")]
        public async Task<IActionResult> UploadProductFit(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\productfit\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\productfit\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/productfit/" + userid.ToString() + "/productfit.xlsx");
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
        [Route("UploadCustomer/{userid}")]
        public async Task<IActionResult> UploadCustomer(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\customer\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\customer\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/customer/" + userid.ToString() + "/customer.xlsx");
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
        [Route("UploadLine/{userid}")]
        public async Task<IActionResult> UploadLine(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\line\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\line\\" + userid.ToString());
                }
                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/line/" + userid.ToString() + "/line.xlsx");
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
        [Route("UploadShift/{userid}")]
        public async Task<IActionResult> UploadShift(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\shift\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\shift\\" + userid.ToString());
                }
                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/shift/" + userid.ToString() + "/shift.xlsx");
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
        [Route("UploadQCCode/{userid}")]
        public async Task<IActionResult> UploadQCCode(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\defects\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\defects\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/defects/" + userid.ToString() + "/defects.xlsx");
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
        [Route("ModuleUpload/{userid}")]
        public async Task<IActionResult> ModuleUpload(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\module\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\module\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/module/" + userid.ToString() + "/module.xlsx");
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
        [Route("UploadUser/{userid}")]
        public async Task<IActionResult> UploadUser(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\user\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\user\\" + userid.ToString());
                }
                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/user/" + userid.ToString() + "/user.xlsx");
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
        [Route("UploadProcessDefinition/{userid}")]
        public async Task<IActionResult> ProcessDefinition(int userid, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\processdefinition\\" + userid.ToString()))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\processdefinition\\" + userid.ToString());
                }

                var path = "";
                string filename = "";
                if (file != null)
                {
                    filename = file.FileName;
                    var uploads = Path.Combine("uploads/processdefinition/" + userid.ToString() + "/processdefinition.xlsx");
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
