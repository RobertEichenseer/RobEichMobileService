using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace RobEichMobileService
{
class RequestInterceptor  : DelegatingHandler 
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
    {
        var withVersion = request.RequestUri.AbsoluteUri;

        withVersion += (string.IsNullOrEmpty(request.RequestUri.Query)) ? "?" : "&";
        withVersion += "version=2.0";

        request.RequestUri = new Uri(withVersion); 

        return base.SendAsync(request, cancellationToken);
    }
    
}
}
