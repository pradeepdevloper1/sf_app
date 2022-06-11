using XF.APP.DTO;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace XF.APP.ABSTRACTION
{
    public interface IUserWebService : IDisposable
    {
        Task<T> LoginUserAsync<T>(UsersLoginInputDto usersDto);
        Task<T> GetSoListAsync<T>(SoViewInputDto soDto, string auth);
        Task<T> GetPoListAsync<T>(SoPoInputDto soInputDto, string auth);
        Task<T> GetPoListAsync<T>(PoInputDto poInputDto, string auth);
        Task<T> PostQCAsync<T>(PostQCInputDto postQCInputDto);
        Task<T> GetDefectListAsync<T>();
        Task<T> PostOperationListAsync<T>(PurchaseOrderInputDto poInputDto);
        Task<T> PostDailyActivityAsync<T>(DailyActivityInputDto dailyActivityInputDto);
        Task<T> PostUndoAsync<T>(UndoInputDto undoInputDto);
        Task<T> PostShiftTargetAsync<T>(ShiftTargetInputDto shiftTargetInput);
        Task<T> GetShiftAsync<T>();
        Task<T> PostEndShiftAsync<T>(PostQCInputDto PostQCInputDto);
        Task<T> GetTotalQCQuantity<T>(PostQCInputDto PostQCInputDto);
        Task<T> GetUserRoleListAsync<T>(string auth);
        Task<T> GetUserLineListAsync<T>(UserModule module, string auth);
        Task<T> GetIssueToWOData<T>(PostQCInputDto PostQCInputDto);
        Task<T> GetIssueToNextProcessData<T>(PostQCInputDto PostQCInputDto);
        Task<T> PostOrderIssue<T>(List<OrderIssue> orderIssueData);
    }
}
