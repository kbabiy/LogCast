using System.Net.Http;
using System.Threading.Tasks;
using LogCast.Http;
using LogCast.Http.Contract;
using JetBrains.Annotations;

namespace LogCast.WebApi
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public interface IHttpMessageInspector
    {
        Request GetRequest(LogCastContext target, HttpRequestMessage requestMessage);
        Response GetResponse(LogCastContext target, HttpResponseMessage responseMessage);

        Task<Request> GetRequestAsync(LogCastContext target, HttpRequestMessage requestMessage);
        Task<Response> GetResponseAsync(LogCastContext target, HttpResponseMessage responseMessage);

        /// <summary>
        /// If 'True' then <see cref="GetRequestAsync"/> method will be used, if 'False' - <see cref="GetRequest"/>
        /// </summary>
        bool UseGetRequestAsync { get; }

        /// <summary>
        /// If 'True' then <see cref="GetResponseAsync"/> method will be used, if 'False' - <see cref="GetResponse"/>
        /// </summary>
        bool UseGetResponseAsync { get; }

        LoggingOptions Options { get; }
    }
}