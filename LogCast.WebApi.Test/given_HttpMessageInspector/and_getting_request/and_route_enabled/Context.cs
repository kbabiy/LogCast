using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using LogCast.Http;

namespace LogCast.WebApi.Test.given_HttpMessageInspector.and_getting_request.and_route_enabled
{
    public abstract class Context : and_getting_request.Context
    {
        protected override LoggingOptions Options => new LoggingOptions
        {
            LogRequestRouteInfo = true
        };
        
        protected virtual IDictionary<string, object> RouteValues => new Dictionary<string, object>
        {
            {"key", "value"}
        };

        public override void Arrange()
        {
            base.Arrange();
            var context = RequestMessage.GetRequestContext();
            context.RouteData = new HttpRouteData(new HttpRoute(), new HttpRouteValueDictionary(RouteValues));
        }
    }
}