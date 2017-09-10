using System;
using RestSharp;

namespace ib.mbank
{
    public class MbankResponse : IMbankResponse
    {
        public IRestResponse Response { get; }
        public bool IsSuccess { get; }

        public MbankResponse(IRestResponse response, bool isSuccess)
        {
            Response = response;
            IsSuccess = isSuccess;
        }

        public static MbankResponse Failed(IRestResponse response) => new MbankResponse(response, false);
    }

    public class MbankResponse<T> : MbankResponse, IMbankResponse<T>
        where T : class
    {
        public T Result { get; }
        public MbankResponse(IRestResponse response, bool isSuccess, T result)
            : base(response, isSuccess)
        {
            Result = result;
        }

        public static new MbankResponse<T> Failed(IRestResponse response) => new MbankResponse<T>(response, false, null);

    }
}
