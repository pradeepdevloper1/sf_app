using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using System;
using System.Threading.Tasks;

namespace XF.APP.LogService
{
    public class LogService : BaseWebService, ILoggerService
    {
        public async Task<ResponseDto> AddError(string error)
        {
            //replace this with your actual keep log logoc if required
            return null;

        }

        public async Task<ResponseDto> AddError(string error, string title = "")
        {
            //replace this with your actual keep log logoc if required
            return null;
        }

        public async Task<ResponseDto> AddException(Exception exception)
        {
            //replace this with your actual keep log logoc if required
            return null;

        }

        public async Task<ResponseDto> AddException(Exception exception, string title = "")
        {
            //replace this with your actual keep log logoc if required
            return null;     
        }

        public async Task<ResponseDto> AddInfo(string message)
        {
            //replace this with your actual keep log logoc if required
            return null;
        }

        public async Task<ResponseDto> AddInfo(string message, string title = "")
        {
            //replace this with your actual keep log logoc if required
            return null;
        }
    }
}
