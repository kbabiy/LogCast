using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using LogCast.Wcf.Configuration;
using LogCast.Wcf.Data;
using JetBrains.Annotations;

namespace LogCast.Wcf
{
    /// <summary>
    /// When registered as WCF message inspector, opens <see cref="LogCastContext"/> for each service call.
    /// Additionaly, adds request/response message information to the current <see cref="LogCastContext"/> 
    /// according to the provided <see cref="LoggingOptions"/>
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class LogCastContextInspector : LoggingInspectorBase, IDispatchMessageInspector
    {
        private static readonly ILogger Logger = LogManager.GetLogger();
        private readonly Func<Message, string> _correlationIdProvider;

        /// <summary>
        /// Initializes a new instance of <see cref="LogCastContextInspector"/> class with default logging options
        /// (no information is added to the current <see cref="LogCastContext"/>)
        /// </summary>
        public LogCastContextInspector([CanBeNull] Func<Message, string> correlationIdProvider)
            : this(correlationIdProvider, new LoggingOptions())
        {
            // By default, we log nothing because main purpose of the inspector is opening of LogCastContext
        }

        /// <summary>
        /// Initializes a new instance of <see cref="LogCastContextInspector"/> class with the provided logging options
        /// </summary>
        public LogCastContextInspector([CanBeNull] Func<Message, string> correlationIdProvider, [NotNull] LoggingOptions options)
            : base(options)
        {
            _correlationIdProvider = correlationIdProvider;
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var operation = request.Headers.Action;
            if (operation == null)
                return null;

            var context = new LogCastContext
            {
                OperationName = operation.Substring(operation.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)
            };

            var correlationId = _correlationIdProvider?.Invoke(request);
            if (correlationId != null)
                context.CorrelationId = correlationId;

            var root = ApplyRequestMessage(context, request);
            return new CorrelationState<Root>(context, root);
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var state = correlationState as CorrelationState<Root>;
            var context = state?.Context;
            if (context == null)
                return;

            // For one-way operations reply message will be null (except when an error is thrown)
            if (reply != null)
                ApplyResponseMessage(context, state.LogEntry, reply);

            context.Dispose();
        }

        protected virtual Root ApplyRequestMessage(LogCastContext target, Message requestMessage)
        {
            // Check is inside the method to allow overriding
            if (!Options.IsRequestLogged)
                return null;

            var request = new Request();
            var root = new Root { Request = request };

            try
            {
                if (Options.LogCallerAddress)
                {
                    request.Caller = GetCallerAddress(requestMessage.Properties);
                }
                if (Options.LogRequestProperties)
                {
                    request.Properties = CollectProperties(requestMessage.Properties);
                }
                if (Options.LogRequestHttpData)
                {
                    request.HttpHeaders = CollectHttpRequestHeaders(requestMessage.Properties);
                }
                if (Options.LogRequestBody)
                {
                    request.Body = requestMessage.ToString();
                }

                target.Properties.Add(Root.FieldName, root);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to log WCF request message data.", ex);
            }
            return root;
        }

        protected virtual void ApplyResponseMessage(LogCastContext target, [CanBeNull]Root logEntry, Message message)
        {
            // Check is inside the method to allow overriding
            if (!Options.IsResponseLogged)
                return;

            try
            {
                var response = new Response();
                if (Options.LogResponseProperties)
                {
                    response.Properties = CollectProperties(message.Properties);
                }
                if (Options.LogResponseHttpData)
                {
                    response.HttpHeaders = CollectHttpResponseHeaders(message.Properties);
                }
                if (Options.LogResponseBody)
                {
                    response.Body = message.ToString();
                }

                if (logEntry == null)
                {
                    logEntry = new Root();
                    target.Properties.Add(Root.FieldName, logEntry);
                }

                logEntry.Response = response;

            }
            catch (Exception ex)
            {
                Logger.Error("Failed to log WCF reply message data.", ex);
            }
        }        
    }
}
