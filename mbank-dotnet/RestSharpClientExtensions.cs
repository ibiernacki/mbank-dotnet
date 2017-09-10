using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ib.mbank
{
    internal static class RestSharpClientExtensions
    {
        internal static Task<IRestResponse> ExecuteTaskAsync(this IRestClient client, IRestRequest request, CancellationToken cancellationToken)
        {
            var taskCompletionSource = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, result => taskCompletionSource.SetResult(result));
            return taskCompletionSource.Task;
        }
    }
}
