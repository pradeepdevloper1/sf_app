using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using XF.APP.ABSTRACTION;
using XF.APP.DTO;

namespace XF.APP.ApiService
{
    public class UserWebService : BaseWebService, IUserWebService
    {
        public async Task<T> LoginUserAsync<T>(UsersLoginInputDto usersLoginInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = usersLoginInputDto,
                Url = "user/login",
                ApiType = ApiType.POST,
                Auth = null
            });
        }

        public async Task<T> GetSoListAsync<T>(SoViewInputDto soInputDto, string auth)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = soInputDto,
                Url = "order/SOList",
                ApiType = ApiType.POST,
                Auth = auth
            });
        }

        public async Task<T> GetPoListAsync<T>(SoPoInputDto soInputDto, string auth)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = soInputDto,
                Url = "order/POList", // "order/POForMApp",
                ApiType = ApiType.POST,
                Auth = auth
            });
        }

        public async Task<T> GetPoListAsync<T>(PoInputDto poInputDto, string auth)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = poInputDto,
                Url = "order/POList",
                ApiType = ApiType.POST,
                Auth = auth
            });
        }

        public async Task<T> PostQCAsync<T>(PostQCInputDto postQCInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = postQCInputDto,
                Url = "QC/PostQC",
                ApiType = ApiType.POST
            });
        }

        public async Task<T> GetDefectListAsync<T>()
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                //Data = null,
                Url = "Defects",
                ApiType = ApiType.GET,
                Auth = null
            });
        }

        public async Task<T> PostOperationListAsync<T>(PurchaseOrderInputDto poInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = poInputDto,
                Url = "OB/POOB",
                ApiType = ApiType.POST,
                Auth = null
            });
        }

        public async Task<T> PostDailyActivityAsync<T>(DailyActivityInputDto dailyActivityInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = dailyActivityInputDto,
                Url = "QC/PostDailyActivity",
                ApiType = ApiType.POST
            });
        }

        public async Task<T> PostUndoAsync<T>(UndoInputDto undoInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = undoInputDto,
                Url = "QC/UndoQC",
                ApiType = ApiType.POST
            });
        }

        public async Task<T> PostShiftTargetAsync<T>(ShiftTargetInputDto shiftTargetInput)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = shiftTargetInput,
                Url = "LineTarget/POShiftTarget",
                ApiType = ApiType.POST
            });
        }

        public async Task<T> GetShiftAsync<T>()
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                //Data = null,
                Url = "Shift",
                ApiType = ApiType.GET,
                Auth = null
            });
        }

        public async Task<T> PostEndShiftAsync<T>(PostQCInputDto PostQCInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = PostQCInputDto,
                Url = "QC/EndShift",
                ApiType = ApiType.POST
            });
        }

        public async Task<T> GetTotalQCQuantity<T>(PostQCInputDto GetTotalQCQuantity)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = GetTotalQCQuantity,
                Url = "QC/GetTotalQCQuantity",
                ApiType = ApiType.POST
            });
        }
        public async Task<T> GetUserRoleListAsync<T>(string auth)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Url = "user/UserRolelist",
                ApiType = ApiType.GET,
                Auth = auth
            });
        }

        public async Task<T> GetUserLineListAsync<T>(UserModule obj, string auth)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = obj,
                Url = "user/UserLineList",
                ApiType = ApiType.POST,
                Auth = auth
            });
        }

        public async Task<T> GetIssueToWOData<T>(PostQCInputDto PostQCInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = PostQCInputDto,
                Url = "QC/GetIssueToWOData",
                ApiType = ApiType.POST
            });
        }

        public async Task<T> GetIssueToNextProcessData<T>(PostQCInputDto PostQCInputDto)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = PostQCInputDto,
                Url = "QC/GetIssueToNextProcessData",
                ApiType = ApiType.POST
            });
        }


        public async Task<T> PostOrderIssue<T>(List<OrderIssue> orderIssueData)
        {
            return await this.SendAsync<T>(new ApiRequest()
            {
                Data = orderIssueData,
                Url = "QC/PostOrderIssue",
                ApiType = ApiType.POST
            });
        }
    }
}
