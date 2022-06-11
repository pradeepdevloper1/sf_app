using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WFX.Data;
using WFX.Entities;
using System.Dynamic;

namespace WFX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        DBContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        IWebHostEnvironment _env;
        UserTokenInfo _UserTokenInfo = new UserTokenInfo();
        public UserController(IWebHostEnvironment env, ILogger<UserController> logger, DBContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _env = env;
        }

        [Authorize]
        [HttpGet]
        public ActionResult Get()
        {
            var _user = _context.tbl_Users.ToList();
            return Ok(_user);
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserList")]
        public ActionResult GetUserList()
        {
            var _user = _context.tbl_Users.ToList();
            return Ok(_user);
        }

        [Authorize]
        [HttpGet]
        [Route("GetUser/{userid}")]
        public ActionResult GetUser(int userid)
        {
            if (userid > 0)
            {
                var _user = _context.tbl_Users.Where(x => x.UserID == userid).FirstOrDefault();
                return Ok(_user);
            }
            else
            {
                var _user = _context.tbl_Users.ToList();
                return Ok(_user);
            }
        }

        [Route("Login")]
        [HttpPost]
        public ActionResult Login([FromBody] SearchModel _user)
        {
            UserLoginDetails data = new UserLoginDetails();
            tbl_Users userData = new tbl_Users();
            if (!string.IsNullOrEmpty(_user.TabletID))
            {
                var linedata = _context.tbl_Lines.Where(x => x.TabletID == _user.TabletID).Select(x => new { x.FactoryID, x.LineName, x.DeviceSerialNo,x.ModuleName }).ToList();
                if (linedata.Count() > 0)
                {
                    var linerecord = linedata.Where(x => x.DeviceSerialNo == _user.DeviceSerialNo).FirstOrDefault();
                    if (linerecord == null)
                    {
                        return Ok(new { status = 401, message = "Device ID : " + _user.DeviceSerialNo + " not mapped with Factory Line. Please map." });
                    }
                    else
                    {
                        userData = _context.tbl_Users.Where(x => x.UserName == _user.UserName && x.Password == _user.Password && x.FactoryID == linerecord.FactoryID).FirstOrDefault();
                        int userRoleID = _context.tbl_FactoryUserRoles.Where(x => x.UserID == userData.UserID).Select(x => x.UserRoleID).FirstOrDefault();
                        int userRoleType = _context.tbl_UserRole.Where(x => x.UserRoleID == userRoleID).Select(x => x.UserRoleType).FirstOrDefault();
                        if (userRoleType != 3)
                        {
                            return Ok(new { status = 401, message = "User is not type of QC." });
                        }
                    }
                    if (userData == null)
                    {
                        return Ok(new { status = 401, message = "Invalid User" });
                    }
                    else
                    {
                        var LinkedWithErp = _context.tbl_Factory.Where(x => x.FactoryID == linerecord.FactoryID).Select(x => x.LinkedwithERP).FirstOrDefault();
                        data.FactoryID = linerecord.FactoryID;
                        data.UserFirstName = userData.UserFirstName;
                        data.UserLastName = userData.UserLastName;
                        data.UserName = userData.UserName;
                        data.Password = userData.Password;
                        data.UserRoleID = userData.UserRoleID;
                        data.Members = userData.Members;
                        data.UserType = userData.UserType;
                        data.UserEmail = userData.UserEmail;
                        data.CreatedDate = userData.CreatedDate;
                        data.UpdatedDate = userData.UpdatedDate;
                        data.LineName = linerecord.LineName;
                        data.UserID = userData.UserID;
                        data.LinkedwithERP = LinkedWithErp;
                        data.Module = linerecord.ModuleName;
                    }
                }
                else
                {
                    return Ok(new { status = 401, message = "Invalid User" });
                }
            }
            else
            {
                if (_user.FromPage == "ERPApp")
                {
                    var recordFactory = _context.tbl_Organisations.Where(x => x.OrganisationName == _user.Organisation)
                    .Join(
                        _context.tbl_Clusters,
                        organisation => organisation.OrganisationID,
                        cluster => cluster.OrganisationID,
                        (organisation, cluster) => new
                        {
                            ClusterID = cluster.ClusterID
                        }
                    )
                    .Join(
                        _context.tbl_Factory,
                        cluster => cluster.ClusterID,
                        factory => factory.ClusterID,
                        (cluster, factory) => new
                        {
                            FactoryID = factory.FactoryID,
                            FactoryName = factory.FactoryName
                        }
                    ).Where(x => x.FactoryName == _user.Module)
                    .Select(x => x.FactoryID).FirstOrDefault();

                    if (recordFactory == 0)
                    {
                        return Ok(new { status = 401, message = "Factory not Sync with Smart Factory." });
                    }

                    var recordModule = _context.tbl_Modules.Where(x => x.FactoryID == recordFactory && x.ModuleName == _user.Module).Select(x => x.ModuleName).FirstOrDefault();
                    if (recordModule == null)
                    {
                        return Ok(new { status = 401, message = "Factory not Sync with Smart Factory Module" });
                    }
                    userData = _context.tbl_Users.Where(x => x.FactoryID == recordFactory).FirstOrDefault();
                    if (userData == null)
                    {
                        return Ok(new { status = 401, message = "Invalid User" });
                    }
                    else
                    {
                        data.FactoryID = userData.FactoryID;
                        data.UserID = userData.UserID;
                    }
                }
                else
                {
                    userData = _context.tbl_Users.Where(x => x.UserName == _user.UserName && x.Password == _user.Password).FirstOrDefault();

                    if (userData == null)
                    {
                        return Ok(new { status = 401, message = "Invalid User" });
                    }
                    else
                    {
                        var LinkedWithErp = _context.tbl_Factory.Where(x => x.FactoryID == userData.FactoryID).Select(x => x.LinkedwithERP).FirstOrDefault();
                        data.FactoryID = userData.FactoryID;
                        data.UserFirstName = userData.UserFirstName;
                        data.UserLastName = userData.UserLastName;
                        data.UserName = userData.UserName;
                        data.Password = userData.Password;
                        data.UserRoleID = userData.UserRoleID;
                        data.Members = userData.Members;
                        data.UserType = userData.UserType;
                        data.UserEmail = userData.UserEmail;
                        data.CreatedDate = userData.CreatedDate;
                        data.UpdatedDate = userData.UpdatedDate;
                        data.UserID = userData.UserID;
                        data.LineName = "";
                        data.LinkedwithERP = LinkedWithErp;
                    }
                }
            }
            if (data == null)
            {
                return Ok(new { status = 401, message = "Invalid User" });
            }
            else
            {

                string auth = CreateToken(userData);
                data.Password = "";
                return Ok(new { status = 200, message = "Success", auth = auth, data });
            }
        }

        private string CreateToken(tbl_Users user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FactoryID", user.FactoryID.ToString()),
                new Claim("UserID", user.UserID.ToString()),
                new Claim("UserFirstName", user.UserFirstName)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpPost]
        [Route("PostMultiUser")]
        public ActionResult PostMultiUser([FromBody] List<tbl_Users> list)
        {
            try
            {
                int id = 0;
                var lastrecord = _context.tbl_Users.OrderBy(x => x.UserID).LastOrDefault();
                id = (lastrecord == null ? 0 : lastrecord.UserID) + 1;
                if (list.Count > 0)
                {
                    foreach (tbl_Users onerow in list)
                    {
                        var record = _context.tbl_Users.Where(x => x.UserName == onerow.UserName && x.FactoryID == onerow.FactoryID).FirstOrDefault<tbl_Users>();

                        if (record == null)
                        {
                            onerow.UserID = id;
                            onerow.FactoryID = onerow.FactoryID;
                            onerow.UserFirstName = onerow.UserFirstName;
                            onerow.UserLastName = onerow.UserLastName;
                            onerow.UserName = onerow.UserName;
                            onerow.Password = onerow.Password;
                            onerow.UserRoleID = 0;
                            onerow.Members = 0;
                            onerow.UserType = onerow.UserType;
                            onerow.UserEmail = onerow.UserEmail;
                            onerow.CreatedDate = DateTime.Now.Date;
                            onerow.UpdatedDate = DateTime.Now.Date;
                            if (onerow.tbl_UserModules != null)
                            {
                                foreach (var module in onerow.tbl_UserModules)
                                {
                                    module.ModuleID = _context.tbl_Modules.Where(x => x.FactoryID == onerow.FactoryID && x.ModuleName == module.ModuleName).Select(x => x.ModuleID).FirstOrDefault();
                                    module.UserID = onerow.UserID;
                                    module.CreatedOn = DateTime.Now.Date;
                                }
                            }

                            if (onerow.tbl_FactoryUserRoles != null)
                            {
                                //string[] UserRolesList = onerow.tbl_FactoryUserRoles.First().UserRole.Split(',');

                                foreach (var userRole in onerow.tbl_FactoryUserRoles)
                                {
                                    userRole.UserRoleID = _context.tbl_UserRole.Where(x => x.UserRole == userRole.UserRole).Select(x => x.UserRoleID).FirstOrDefault();
                                    userRole.UserID = onerow.UserID;
                                    userRole.CreatedOn = DateTime.Now.Date;

                                }
                            }
                            _context.tbl_Users.Add(onerow);
                            _context.SaveChanges();

                            id++;
                        }
                    }
                }

                return Ok(new { status = 200, message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("PutMultiUser")]
        public ActionResult PutMultiUser([FromBody] List<tbl_Users> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    foreach (tbl_Users onerow in list)
                    {
                        var record = _context.tbl_Users.Where(x => x.UserID == onerow.UserID).SingleOrDefault();
                        if (record != null)
                        {
                            record.UserFirstName = onerow.UserFirstName;
                            record.UserLastName = onerow.UserLastName;
                            record.UserName = onerow.UserName;
                            record.Password = onerow.Password;
                            record.UserRoleID = onerow.UserRoleID;
                            record.Members = onerow.Members;
                            record.UserType = onerow.UserType;
                            record.UserEmail = onerow.UserEmail;
                            record.UpdatedDate = DateTime.Now.Date;

                            if (onerow.tbl_FactoryUserRoles != null)
                            {
                                var t = _context.tbl_FactoryUserRoles.Where(x => x.UserID == onerow.UserID).ToList();
                                _context.tbl_FactoryUserRoles.RemoveRange(t);
                                foreach (var rec in onerow.tbl_FactoryUserRoles)
                                {
                                    rec.UserRole = _context.tbl_UserRole.Where(x => x.UserRoleID == rec.UserRoleID).Select(x => x.UserRole).FirstOrDefault();
                                    rec.UserID = onerow.UserID;
                                    rec.UserRoleID = rec.UserRoleID;
                                    rec.CreatedOn = DateTime.Now.Date;
                                    _context.tbl_FactoryUserRoles.Add(rec);
                                }
                                _context.SaveChanges();


                            }
                            if (onerow.tbl_UserModules != null)
                            {
                                var userModules = _context.tbl_UserModules.Where(x => x.UserID == onerow.UserID).ToList();
                                _context.tbl_UserModules.RemoveRange(userModules);
                                foreach (var rec in onerow.tbl_UserModules)
                                {
                                   rec.ModuleName = _context.tbl_Modules.Where(x => x.FactoryID == onerow.FactoryID && x.ModuleID== rec.ModuleID).Select(x => x.ModuleName).FirstOrDefault();
                                    rec.UserID = onerow.UserID;
                                    rec.CreatedOn = DateTime.Now.Date;
                                    _context.tbl_UserModules.Add(rec);

                                }
                                _context.SaveChanges();
                            }
                        }
                        _context.tbl_Users.Update(record);
                        _context.SaveChanges();
                    }

                    return Ok(new { status = 200, message = "Success" });
                }
                else
                {
                    return Ok(new { status = 400, message = "No Record Found." });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message + ex.InnerException });
                //return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetUserList/{factoryid}")]
        public ActionResult GetUserList(int factoryid)
        {
            try
            {
                List<dynamic> UserList = new List<dynamic>();

                var data = _context.tbl_Users.Where(x => x.FactoryID == factoryid).OrderBy(x => x.UserName).ToList();
                foreach (var obj in data)
                {
                    dynamic users = new ExpandoObject();
                    users.UserID = obj.UserID;
                    users.factoryID = obj.FactoryID;
                    users.userFirstName = obj.UserFirstName;
                    users.userLastName = obj.UserLastName;
                    users.userName = obj.UserName;
                    users.password = obj.Password;
                    users.userRoleID = obj.UserRoleID;
                    users.members = obj.Members;
                    users.userType = obj.UserType;
                    users.userEmail = obj.UserEmail;
                    users.createdDate = obj.CreatedDate;
                    users.updatedDate = obj.UpdatedDate;

                    IQueryable<tbl_UserModules> query = _context.tbl_UserModules.Where(x => x.FactoryID == factoryid && x.UserID == obj.UserID);
                    IQueryable<tbl_Modules> Modulesquery = _context.tbl_Modules.Where(x => x.FactoryID == factoryid);
                    var moduleList = from q in query
                                     join m in Modulesquery
                                      on new { q.ModuleID } equals new { m.ModuleID }

                                     select new
                                     {
                                         m.ModuleName, m.ModuleID

                                     };
                    List<int> ModuleIDs = new List<int>();
                    List<string> ModuleNames = new List<string>();
                    foreach (var t in moduleList)
                    {
                        ModuleIDs.Add(t.ModuleID);
                        ModuleNames.Add(t.ModuleName);
                    }
                    if (moduleList != null)
                    {
                        users.ModuleID = ModuleIDs;
                        users.ModuleName = ModuleNames;
                    }
                    IQueryable<tbl_FactoryUserRoles> FactoryUserRolesquery = _context.tbl_FactoryUserRoles.Where(x => x.FactoryID == factoryid && x.UserID == obj.UserID);
                    IQueryable<tbl_UserRole> UserRolequery = _context.tbl_UserRole;
                    var UserRoleList = from q in FactoryUserRolesquery
                                       join m in UserRolequery
                                      on new { q.UserRoleID } equals new { m.UserRoleID }

                                       select new
                                       {
                                           m.UserRole, m.UserRoleID

                                       };
                    List<int> UserRoleIDs = new List<int>();
                    List<string>UserRoles= new List<string>();

                    foreach (var t in UserRoleList)
                    {
                        UserRoleIDs.Add(t.UserRoleID);
                        UserRoles.Add(t.UserRole);
                        
                    }
                    if (UserRoleList.Count() != 0)
                    {
                        users.userRole = UserRoles;
                        users.userRoleID = UserRoleIDs;
                    }
                    UserList.Add(users);
                }
                return Ok(UserList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("FillUserRole")]
        public ActionResult FillUserRole()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_UserRole> query = _context.tbl_UserRole.Where(x => x.UserRoleType != 1);
                var data = query.Select(x => new { Id = x.UserRoleID, Text = x.UserRole }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("FillUserListModule/{factoryid}")]
        public ActionResult FillUserListModule(int factoryid)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_Modules> query = _context.tbl_Modules.Where(x => x.FactoryID == factoryid);
                var data = query.Select(x => new { Id = x.ModuleID, Text = x.ModuleName }).Distinct().ToList();
                if (data.Count <= 0)
                    return Ok(new { status = 400, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("UserRoleList")]
        public ActionResult UserRoleList()
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                IQueryable<tbl_FactoryUserRoles> tblFactoryUserRoles = _context.tbl_FactoryUserRoles.Where(x => x.FactoryID == _UserTokenInfo.FactoryID && x.UserID == _UserTokenInfo.UserID);

                var query = from fur in tblFactoryUserRoles
                            from ur in _context.tbl_UserRole
                            where (ur.UserRoleID == fur.UserRoleID)
                            select new
                            {
                                UserRoleID = fur.UserRoleID,
                                RoleName = ur.UserRole,
                                UserRoleType = ur.UserRoleType
                            };

                var data = query.ToList();

                if (data == null)
                    return Ok(new { status = 401, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("UserLineList")]
        public ActionResult UserLineList(SearchModel _obj)
        {
            try
            {
                _UserTokenInfo = APIHelper.GetUserTokenInfo(HttpContext);
                var data = _context.tbl_Lines.Where(x => x.FactoryID == _UserTokenInfo.FactoryID  && x.ModuleName == _obj.Module).Select(x=> new{LineID = x.LineID,LineName = x.LineName  }).ToList();

                if (data == null)
                    return Ok(new { status = 401, message = "No record found." });

                return Ok(new { status = 200, message = "Success", data });
            }
            catch (Exception ex)
            {
                return Ok(new { status = 400, message = ex.Message });
            }
        }
    }
}
