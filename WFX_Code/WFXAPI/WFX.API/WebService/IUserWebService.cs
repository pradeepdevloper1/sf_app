using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WFX.API.Model;
using WFX.Entities;

namespace WFX.API.WebService
{
    public interface IUserWebService : IDisposable
    {
        Task<T> PostEndShiftGRNAsync<T>(List<tbl_QCMaster> tbl_QCMaster, string ERPAPIURL);
        Task<T> PostIssuetoNextProcessAsync<T>(List<tbl_OrderIssue> tbl_OrderIssue, string ERPAPIURL);
    }
}
