using XF.APP.DTO;
using System;
using System.Threading.Tasks;

namespace XF.APP.ABSTRACTION
{
    public interface ILoggerService:IDisposable
    {
        Task<ResponseDto> AddException(Exception exception, string title = "");
         
        Task<ResponseDto> AddError(string error, string title = "");

        Task<ResponseDto> AddInfo(string message, string title = "");
    }
}
