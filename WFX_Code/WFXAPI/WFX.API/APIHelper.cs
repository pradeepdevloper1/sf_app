using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace WFX.API
{
    public  static class APIHelper
    {
        public static UserTokenInfo GetUserTokenInfo(HttpContext _httpcontext)
        {
            var str = _httpcontext.GetTokenAsync("Bearer", "access_token");
            var stream = str.Result;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            UserTokenInfo _UserTokenInfo = new UserTokenInfo();
            _UserTokenInfo.UserID = Convert.ToInt32( tokenS.Claims.First(claim => claim.Type == "UserID").Value);
            _UserTokenInfo.FactoryID = Convert.ToInt32(tokenS.Claims.First(claim => claim.Type == "FactoryID").Value);
            return _UserTokenInfo;
        }
    }

    public class UserTokenInfo
    {
        public int FactoryID { get; set; }
        public int UserID { get; set; }
    }
}
