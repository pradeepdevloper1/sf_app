using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using WFX.Data;
using WFX.Entities;
using System.Linq;
using System.Collections.Generic;
using WFX.Entities.Table;

namespace WFX.API.Controllers
{
    [ApiController]
    [Route("API/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        DBContext _context;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        public ImageController(IWebHostEnvironment env, ILogger<ImageController> logger, IConfiguration configuration, DBContext context)
        {
            _configuration = configuration;
            _env = env;
            _context = context;
        }

        [HttpPost]
        [Route("UploadPOShiftImages")]
        public async Task<IActionResult> UploadImage(string pono, List<IFormFile> files)
        {
            try
            {
                string apiServerURL = _configuration.GetSection("AppSettings:apiServerURL").Value;
                if (!Directory.Exists(_env.WebRootPath + "\\uploads\\poimages\\" + pono))
                {
                    Directory.CreateDirectory(_env.WebRootPath + "\\uploads\\poimages\\" + pono);
                }

                var path = "";
                string filename = "";
                if (files.Count > 0)
                {


                    long? id = _context.tbl_POShiftImages.Max(x => (long?)x.POShiftImageID);
                    if (id == null) { id = 0; }

                    _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                    string pathfordb = "";

                    foreach (IFormFile file in files)
                    {
                        filename = file.FileName;
                        String timeStamp = GetTimestamp(DateTime.Now);
                        var existing = _context.tbl_POShiftImages.Where(x => x.PONo == pono && x.ImageName == filename).ToList();
                        _context.RemoveRange(existing);
                        _context.SaveChanges();

                        var uploads = Path.Combine("uploads/poimages/" + pono + "/" + timeStamp + "_"+file.FileName);
                        path = Path.Combine(_env.WebRootPath, uploads);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        pathfordb = Path.Combine(apiServerURL, uploads);

                        tbl_POShiftImages _record = new tbl_POShiftImages();
                        id++;
                        _record.POShiftImageID = (long)id;
                        _record.PONo = pono;
                        _record.ImageName = filename.TrimEnd();
                        _record.ImagePath = pathfordb;
                        _record.ShiftName = "S1";
                        _record.FactoryID = _UserTokenInfo.FactoryID;
                        _record.UserID = _UserTokenInfo.UserID;
                        _record.EntryDate = DateTime.Now.Date;
                        _context.Add(_record);

                        tbl_QCMaster qcrecord = _context.tbl_QCMaster.Where(x => x.FactoryID ==_UserTokenInfo.FactoryID 
                                                    && x.PONo == pono).OrderBy(x => x.QCMasterId).LastOrDefault();

                        tbl_QCDefectImages _record1 = new tbl_QCDefectImages();
                        _record1.QCMasterId = qcrecord.QCMasterId;
                        _record1.PONo = pono;
                        _record1.SONo = qcrecord.SONo;
                        _record1.ImageName = filename.TrimEnd();
                        _record1.ImagePath = pathfordb;
                        _record1.FactoryID = _UserTokenInfo.FactoryID;
                        _record1.UserID = _UserTokenInfo.UserID;
                        _record1.EntryDate = DateTime.Now;
                        _context.Add(_record1);
                        _context.SaveChanges();
                    }
                    return Ok(new { status = 200, message = "Sucess", path = pathfordb });
                }
                else
                {
                    return Ok(new { status = 400, message = "No files found." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("GetPOShiftImages")]
        [HttpGet]
        public ActionResult GetPOShiftImages()
        {
            return Ok(_context.tbl_POShiftImages.ToList());
        }
    }
}
