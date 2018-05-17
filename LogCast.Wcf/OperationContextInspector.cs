using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using LogCast.Engine;
using LogCast.Inspectors;
using LogCast.Wcf.Configuration;
using LogCast.Wcf.Data;

namespace LogCast.Wcf
{
    /// <summary>
    /// When registered as LogCast inspector, adds information from the current <see cref="OperationContext"/> 
    /// to every <see cref="LogCastContext"/>, according to the passed <see cref="LoggingOptions"/>
    /// </summary>
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class OperationContextInspector : LoggingInspectorBase, ILogDispatchInspector
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OperationContextInspector"/> class with default logging options
        /// (only request fields are logged)
        /// </summary>
        public OperationContextInspector() : base(LoggingOptions.RequestOnly())
        {
            // By default, we log only request properties because response info won't probably be available at this point
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OperationContextInspector"/> class with the provided logging options
        /// </summary>
        public OperationContextInspector(LoggingOptions options) : base(options)
        {
        }
        
        public void BeforeSend(LogCastDocument document, LogMessage sourceMessage)
        {
            var source = OperationContext.Current;
            if (source != null)
                Apply(document, source);
        }

        public void BeforeSend(LogCastDocument document, LogCastContext sourceContext)
        {
            var source = OperationContext.Current;
            if (source != null)
                Apply(document, source);
        }


        protected virtual void Apply(LogCastDocument target, OperationContext source)
        {
            var root = new Root();

            if (Options.IsRequestLogged)
            {
                var request = new Request();
                if (Options.LogCallerAddress)
                {
                    request.Caller = GetCallerAddress(source.IncomingMessageProperties);
                }
                if (Options.LogRequestProperties)
                {
                    request.Properties = CollectProperties(source.IncomingMessageProperties);
                }
                if (Options.LogRequestHttpData)
                {
                    request.HttpHeaders = CollectHttpRequestHeaders(source.IncomingMessageProperties);
                }
                if (Options.LogRequestBody)
                {
                    request.Body = source.RequestContext?.RequestMessage?.ToString();
                }

                root.Request = request;
            }

            if (Options.IsResponseLogged)
            {
                var response = new Response();
                if (Options.LogResponseProperties)
                {
                    response.Properties = CollectProperties(source.OutgoingMessageProperties);
                }
                if (Options.LogResponseHttpData)
                {
                    response.HttpHeaders = CollectHttpResponseHeaders(source.OutgoingMessageProperties);
                }

                root.Response = response;
            }

            if (root.HasData)
                target.AddProperty(Root.FieldName, root);
        }
    }
}