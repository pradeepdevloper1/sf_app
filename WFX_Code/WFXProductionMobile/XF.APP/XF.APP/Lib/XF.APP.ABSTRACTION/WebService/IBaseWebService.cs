using XF.APP.DTO;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace XF.APP.ABSTRACTION
{
    public interface IBaseWebService:IDisposable
    {
        HttpClient httpClient { get; set; }
        RequestDto requestDto { get; set; }
        ResponseDto responseDto { get; set; }
        Task<ResponseDto> SendAsync(ApiRequest apiRequest);
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
