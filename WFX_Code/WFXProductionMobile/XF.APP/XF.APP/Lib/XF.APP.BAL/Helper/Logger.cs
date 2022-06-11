using XF.APP.ABSTRACTION;
using XF.APP.DTO;
using System;
using System.Threading.Tasks;

namespace XF.APP.BAL
{
    public static class Logger
    {
        public static async Task<ResponseDto> AddException(Exception exception, string title = "")
        {
            var loggerService = ServiceLocator.Resolve<ILoggerService>();
            return await loggerService.AddException(exception, title);
        }

        public static async Task<ResponseDto> AddError(string error, string title = "")
        {
            var loggerService = ServiceLocator.Resolve<ILoggerService>();
            return await loggerService.AddError(error, title);
        }

        public static async Task<ResponseDto> AddInfo(string message, string title = "")
        {
            var loggerService = ServiceLocator.Resolve<ILoggerService>();
            return await loggerService.AddInfo(message, title);
        }
    }
}
