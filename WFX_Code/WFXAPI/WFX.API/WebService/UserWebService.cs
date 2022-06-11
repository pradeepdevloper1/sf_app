using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WFX.API.Model;
using WFX.Entities;

namespace WFX.API.WebService
{
    public class UserWebService : BaseWebService,IUserWebService
    {
        public async Task<T> PostEndShiftGRNAsync<T>(List<tbl_QCMaster> tbl_QCMaster, string ERPAPIURL)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = tbl_QCMaster,
                Url = ERPAPIURL + "WFXAPISmartFactoryIntegration/CreateGRN",
                ApiType = ApiType.POST
            });
        }

        public async Task<T> PostIssuetoNextProcessAsync<T>(List<tbl_OrderIssue> tbl_OrderIssue, string ERPAPIURL)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = tbl_OrderIssue,
                Url = ERPAPIURL + "WFXAPISmartFactoryIntegration/IssuetoNextProcess",
                ApiType = ApiType.POST
            });
        }
    }
}
