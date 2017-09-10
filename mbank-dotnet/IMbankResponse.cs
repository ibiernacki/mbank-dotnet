using RestSharp;
using System;
using System.Net.Http;

namespace ib.mbank
{
    public interface IMbankResponse
    {
        IRestResponse Response { get; }
        bool IsSuccess { get; }
    }

    public interface IMbankResponse<T> : IMbankResponse
        where T : class
    {
        T Result { get; }
    }
}
