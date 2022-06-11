using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WFX.API.Model;

namespace WFX.API.WebService
{
    public interface IBaseWebService : IDisposable
    {
        HttpClient httpClient { get; set; }
        RequestDto requestDto { get; set; }
        ResponseDto responseDto { get; set; }
        Task<ResponseDto> SendAsync(ApiRequest apiRequest);
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
