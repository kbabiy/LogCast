# LogCast

## Intro

Library to declaratively compose logs, enrich them with valuable statistics and send to centralize storage

Problems the library is solving:

- Uniform log structure
	- Similar attributes structure and names
	- Same atrribute values when applicable (for example: log level values regardless of the logging framework)
	- Simple extensibility of standard log contents with application specific attributes
- Unit of Work (UoW): composing multiple application log calls into one message by context
- Minimum effort and logging related code on the client side
	- Minimalistic and declarative API
	- As much stats/metrics gathered automatically as possible
- Advanced sending
	- Performance optimized, heavy operations called asynchronously
	- Isolation from application thread
		- Configured parameters of resources consumed by the subsystem
	- Reliable delivery
- Plugging to existing logging frameworks as well as standalone setup
- More: multithreaded scenarios, readymade message handlers, fallback logging, nested contexts, log sending tweaks, extensibility, throttling etc.

------

## Quick start

Example solutions are available to see the basic setup

- [Examples.DirectLog](Examples/DirectLogger)
- [Examples.NLog](Examples/NLog)
- [Examples.Tracing](Examples/Tracing)

The easy way to start is picking one of those and implement your code the same way. Most of the times the trivial setup should do what you need

If you feel need in more details or setup different than default - follow the stages below with the setup and usage exlpained on 
example of standalone configuration (DirectLog)

### Install

Install nuget package to your application project(s)

- All the projects writing log entries will need the main library

```
Install-Package LogCast
```

- In case setup used is external logging framework integration, host project also need logging framework specific library installed.
Example for NLog:

```
Install-Package LogCast.NLog
```

### Configure

'LogConfig.Configure' method should be called on the application startup before first log entry added

This is done to set up:

- which specific implementation will be processing the logging (NLog, Tracing, DirectLog etc.). 
This approach allows avoiding redundant dependencies added to the client applicaiton
- (optionally) peform configuration in the code, such as setting up application name and logging endpoint
- (optionally) inject custom framework behaviors

#### Configure through the code completely

```
LogConfig.Configure(
	new DirectLogManager(
		new DirectLoggerOptions(LogLevel.Info, "DirectLoggerExamples")
		{ Layout = "{date:hh-mm-ss.fff tt} | {level} | {logger} | {message}" },
		new LogCastOptions("http://elk.test.com:9200")));
```

#### Or through the App.config

```
LogConfig.Configure(new DirectLogManager());
```

And the config itself: [App.config](Examples/DirectLogger/App.config)

### Log

Below is the simplest logging scenario:

```
private static readonly ILogger Log = LogManager.GetLogger();

public string Operation(string input, string correlation_id)
{
    using (new LogCastContext(correlation_id))
    {
        Log.Info($"started: {input}");

        // logic here

        Log.Debug("progress");

        // and some more logic here	

        Log.Info($"completed: {result}");
        return result;
    }
}
```

Explanations:

```
using (new LogCastContext(correlation_id))
{ }
```

This 'using' block outlines the code section where all log entries are gathered into one Unit of Work. 
Using UoW is not required, but is recommended in most server side cases. Besides giving well organized
logs overview it also gathers statistics from analyzing log entry set in a single context - such as timings information 
(see below for more stats details)

'correlation_id' is an optional parameter to identify certain call. This is especially useful to join related calls of 
several different systems

LogCastContext supports nested usage. In such setup nested context is treated as standalone UoW, and its content is taken away of 
the parent context

### Results

Query for your entry in the dashboard of the log system you use (such as ELK.Kibana) by the value of SystemType in the 'type' field, or by the value of correlation_id in the fields.correlation_id.
With the simplest setup code from the basic usage scenario will send next message:

```
{
	"timestamp": "2016-10-28T14:41:54.6379703+03:00",
	"message": "error",
	"type": "Examples",
	"source": "Direct",
	"fields": {
		"correlation_id": "direct log example call",
		"operation": "Operation",
		"log_level": "Error",
		"log_level_code": 21,
		"details": 
			"(24) ----------
			02-41-54.662 PM | Info | Program | info
			(18) ----------
			02-41-54.681 PM | Warn | Program | warn
			(20) ----------
			02-41-54.701 PM | Error | Program | error
			(58) ----------",
		"durations": {
			"value": [24,18,20,58],
			"total": 129
		},
		"logger": ["Program"],
		"host": {
			"name": "LT-214809",
			"ip": ["10.0.48.57"]
		},
		"application_version": "1.0.0.0",
		"logs": {
			"length": 607
		}
	}
}
```

------

## Application specific attributes

No matter how much stats can the library collect, there are still application or domain specific properties that have to be added from the client code.
For such case there is an API provided to inject such properties into the UoW collected

### Simple cases

Simply adding a value to a named attribute can be done in several ways, such as 

- Through the logger (recommended)

```
logger.AddContextProperty("int_property", 3);
```

- Through local context instance

```
context.Properties.Add("Int_Property", 5)
```

- Or even static context instance

```
LogCastContext.Current.Properties.Add("INT_PROPERTY", 7);
```

Application specific attributes are case insensitive and values are grouped by certain rules that can be redefined.
Default behavior is composing an array in case of multiple values set by the same key

Calling three code examples above in a context of the single UoW leads to the next attribute added to the @fields object 
of your log message:

```
"int_property": [3, 5, 7]
```

### Advanced

#### Complex data

Property value added can be not only primitive values as in the examples above, but also complex structures. 
This is a good approach that can be used to group logical parts of the log entries

Options available to approach this are:

- Data contract, with json serialization attributes applied when needed

```
private class MyLoggingData
{
	[JsonProperty(PropertyName = "name")]
	public string Name;

	[JsonProperty(PropertyName = "numbers")]
	public int[] Numbers;
} 
```

- Complex object in form of 'Dictionary<string, object>'

While this is an option, it is not comfortable to use. In order to make usage declarative, it is possible to
add properties in form of {containerName, propertyName, value}

In the resulting log properties with the same containerName are grouped under the same nested object

For example code that enriches existing durations attribute looks like:

```
Log.AddContextProperty("durations", "storage.all", 12);
Log.AddContextProperty("durations", "storage.all", 30);
```

#### Aggregations

Default approach for the values added to the same field is aggregating them into an array. 
This is a subject to override though. 

Example:

```
var total = new Func<IEnumerable<int>, int>(x => x.Sum());
Log.AddContextProperty("durations", "storage.total", 12, total);
...
Log.AddContextProperty("durations", "storage.total", 30, total);

```

#### Results

```
"durations": {
	"value": [23,17,20,78],
	"storage": [12, 30],
	"storage.total": 42,
	"total": 148
}
```

## Handling context with message handlers

Basic approach proposed above suggests LogCastContext creation in the application code. Even though it is simple and straightforward, 
it is not the only approach available for services

In service code basic approach has certain properties that can be seen as disadvantages:

- Every service method has to be decorated with LogCastContext scope
- Some infrastructural information can be unavailable on the service implementation level (e.g. response body)

In order to target possible concerns an option to identify context scope can be on the message handler is available:

- WebApi: [LogCastContextHandler (in LogCast.WebApi package)](LogCast.WebApi/LogCastContextHandler.cs)
	- [TIP] If you want LogCastContextHandler to include log messages from your other handlers into the corresponding UoW, add it to the 
HttpConfiguration.MessageHandlers collection before them
- WCF: [LogCastContextInspector (in LogCast.Wcf package)](LogCast.Wcf/LogCastContextInspector.cs)
- MVC: [LogCastModule (in LogCast.Mvc package)](LogCast.Mvc/LogCastModule.cs)
- OWIN: [LoggingMiddleware (in LogCast.Owin)](LogCast.Owin/LoggingMiddleware.cs)
- Asp.Net Core: [AspNetCoreContextStrategy (in LogCast.AspNetCore package)](LogCast.AspNetCore/AspNetCoreContextStrategy.cs)

These message handlers allow to target concerns listed above. They also contain logic to extact service specific information 
(such as request/response message headers and bodies, claims etc.) automatically

Example for the WebApi service configuration

- Install additional package for WebApi
- Configure as explained above and include the message handler (in Global.asax for example):

```
LogConfig.Configure(new DirectLogManager());
httpConfiguration.MessageHandlers.Add(new LogCastContextHandler());
```

This configuration means: wrap every service call into the LogCastContext; use default correlation_id, 
operation name and HttpMessageInspector options 

You have option to apply a more custom configuration, like:

```
new LogCastContextHandler(
    correlationIdProvider: r => r.GetCorrelationId(), 
    operationNameProvider: r => r.GetOperationName, 
    messageInspector: new HttpMessageInspector(LoggingOptions.All()))
```

This means: wrap every service call into the LogCastContext; redefine the way correlation_id and 
operation name are obtained; use custom HttpMessageInspector options 

- In the service code instantiate logger and log as usual - but don't create LogCast context. It exists on the level 
above your code now, will be created before any call and disposed after it. Example:

```
private readonly ILogger _logger = LogManager.GetLogger();

public IHttpActionResult GetItem(int id)
{
    _logger.Info("Starting with id " + id);
    var item = _stock.FirstOrDefault(p => p.Id == id);
    if (item == null)
        return NotFound();

    _logger.Info("Finished");
    return Ok(item);
}
```

- Result for the default configuration can look like

```
{
	"timestamp": "2017-04-14T13:14:25.1575926+03:00",
	"message": "Finished",
	"type": "Examples.WebApi",
	"source": "Trace",
	"fields": {
	  "correlation_id": "webapi_call_correlationId",
	  "operation": "{controller}/{id}",
	  "log_level": "Info",
	  "log_level_code": 3,
		"details": 
			"(97) ----------
			01-53-15.135 PM | Info | StoreController | Starting with id1
			(3) ----------
			01-53-15.138 PM | Info | StoreController | Finished
			( 36) ----------",
	  "durations": {
			"value": [97, 3, 36],
			"total": 143
	  },
	  "logger": ["StoreController"],
	  "http": {
		"request": {
		  "uri": "http://localhost:61523/store/1",
		  "http_method": "GET",
		  "headers": {
			"connection": ["keep-alive"],
			"accept": ["text/html","application/xhtml+xml","application/xml; q=0.9","image/webp","*/*; q=0.8"],
			"accept-encoding": ["gzip","deflate","sdch","br"],
			"accept-language": ["ru-RU","ru; q=0.8","en-US; q=0.6","en; q=0.4","uk; q=0.2"],
			"host": ["localhost:61523"],
			"user-agent": ["Mozilla/5.0","(Windows NT 10.0; WOW64)","AppleWebKit/537.36","(KHTML, like Gecko)","Chrome/57.0.2987.133","Safari/537.36"],
			"upgrade-insecure-requests": ["1"]
		  },
		  "route": {
			"template": "{controller}/{id}",
			"controller": "store",
			"action": null,
			"values": {
			  "controller": "store",
			  "id": "1"
			}
		  }
		},
		"response": {
		  "status": 200,
		  "headers": {
			"correlation-id": ["Examples.WebApi-d7f77795-d25a-46a1-8a8b-7dbe57f7ed9f"],
			"content-type": ["application/xml; charset=utf-8"]
		  }
		}
	  },
	  "host": {
		"name": "LT-214809",
		"ip": ["10.0.48.58"]
	  },
	  "application_version": "1.0.0.0",
	  "logs": {
		"version": "1.0.0.0",
		"drop_count": 0,
		"retry_count": 0,
		"length": 1501
	  }
	}
}
```

Note next things about this response:

- UoW is composed of a few calls
- Regular metrics are gathered
- Http related info is captured (request/response headers, route)
- correlation_id has a custom value, and sent with response headers


## Configuration settings

### DirectLog 

Can be divided into

- Log sending specific options, represented by [LogCastOptions](LogCast/Delivery/LogCastOptions.cs)

The only required parameter in this settings is the 'Endpoint' to send log messages to, but you also have an option to tweak components 
sending log messages over the network into centralized logging system

You can optionally vary sending thread count, timeouts, define when retry will give up and current message dropped by 
changing retry timeout and throttling, enable self diagnostics etc.

- Log processing specific options, represented by [DirectLoggerOptions](LogCast/Loggers/Direct/DirectLoggerOptions.cs)

In case of App.config based configuration is used config structure can be checked in [DirectLog config section](LogCast/Loggers/Direct/ConfigurationSection.cs)

### Logging framework specific setup

Spelling of the settings similar to the ones explained in the previous section can be revisied in:

- NLog: [LogCastTarget](LogCast.NLog/LogCastTarget.cs) - in the 'Target options' and 'LogCastClient options' regions
- Tracing: [LogCastTraceListener](LogCast.Tracing/LogCastTraceListenerWorker.cs) - in the 'Options' region

## Extending logging with aspects

LogConfig.Configure simple call can be replaced with the more advanced fluent version:

```
LogConfig.BeginConfiguration(new DirectLogManager())
    .With...
    .With...
    .End();
```

This approach allows changing multiple aspects of the framework behavior. Most interesting is next though:

```
WithGlobalInspector(ILogDispatchInspector inspector)
```

This extension allows adding custom logging aspects to the resulting message. [ILogDispatchInspector](LogCast/Inspectors/ILogDispatchInspector.cs) passed in 
handles both standalone and UoW-composed messages and lets apply application specific changes

Readymade inspectors available are: 

- [ConfigurationInspector.cs](LogCast/Inspectors/ConfigurationInspector.cs) - used in the library to apply logging framework specific values
- [EnvironmentInspector.cs](LogCast/Inspectors/EnvironmentInspector.cs) - used in the library to apply application environment values
- [HttpContextInspector](LogCast.Http/HttpContextInspector.cs) - available to apply values from the 'HttpContext.Current'. Can be used to add http-specific 
attributes with the basic setup (without handlers/interceptors)
- [OperationContextInspector](LogCast.Wcf/OperationContextInspector.cs) - available to apply values from the 'OperationContext.Current'. Can be used to add WCF-specific 
attributes with the basic setup (without handlers/interceptors)
NOTE: there is a known .Net framework issue when within the async WCF service OperationContext.Current becomes null after the first async call. This has been fixed with .Net framework 4.6.2,
but operation context async flow is now disabled by default. To enable it (and use functionality fixed in 4.6.2) you need to set a flag in your app.config like:

```
<appSettings>
    <add key="wcf:disableOperationContextAsyncFlow" value="false" />
</appSettings>
```

## Fallback file logger

For case of standalone configuration or when selected logging framework doesn't support the fallback logging mechanism, file fallback logger 
is provided

In order to enable this functionality use 'FallbackLogDirectory' option when configuring your logging setup. This can be both absolute or relative director path.
In case relative path is used it is being rooted to '{Path.GetTempPath()}\LogCastTemp'

Base directory of an applicaion is not used in order to avoid unexpected side effects on the client application

## Multithreaded scenarios

If writing logs in parallel threads the automatically gathered logging statistics will be corrupted since logs will arrive in undefined order, and 
UoW summary will also have changing structure

In order to avoid problems in such scenarios we recommend to consider writing logs in the sequential parts of code in order to have 
a simple setup along with correct and meaningful statistics. It is important to notice here that sequential does not mean single-threaded
(thank you, Captain Obvious) which is an especially important notice in applications using async/await

In some cases it is required to gather statistics from the code executed in parallel though. For such cases we have introduced
LogCastContextBranch which serves to mark the boundaries of the logical sequential piece of code

Metadata generated with such markup allows meaningfully arrange items from code executed in parralel and calculate the statistics (such as durations)

Example (can be found in the [Examples.DirectLog](Examples.DirectLogger/Program.cs)):

```
private void OperationWithParallelThreads()
{
    using (new LogCastContext())
    {
        Log.Info("main started");

        var tasks = new List<Task>();
        for (int i = 0; i < 3; i++)
        {
            tasks.Add(Task.Factory.StartNew(id =>
            {
                using (new LogCastContextBranch())
                {
                    Log.Info($"task {id} started");
                            
                    //Some work done here
                    Thread.Sleep(10);

                    Log.Info($"task {id} progresses");

                    //Some more work done here
                    Thread.Sleep(15);

                    Log.Info($"task {id} finished");
                }
            }, i));
        }

        Task.WaitAll(tasks.ToArray());
        Log.Info("main finished");
    }
}
```

Resulting output of this code is arranged like this:

```
(12) ----------
11-59-17.787 AM | Info | Program | main started
    [B3] start
        (5) ----------
        11-59-17.792 AM | Info | Program | task 1 started
        (10) ----------
        11-59-17.803 AM | Info | Program | task 1 progresses
        (16) ----------
        11-59-17.819 AM | Info | Program | task 1 finished
    [B3] end
    [B2] start
        (5) ----------
        11-59-17.792 AM | Info | Program | task 2 started
        (11) ----------
        11-59-17.804 AM | Info | Program | task 2 progresses
        (15) ----------
        11-59-17.820 AM | Info | Program | task 2 finished
    [B2] end
    [B1] start
        (7) ----------
        11-59-17.794 AM | Info | Program | task 0 started
        (10) ----------
        11-59-17.805 AM | Info | Program | task 0 progresses
        (15) ----------
        11-59-17.820 AM | Info | Program | task 0 finished
    [B1] end
(33) ----------
11-59-17.820 AM | Info | Program | main finished
(18) ----------
```

Here all the logs added from the branch are grouped and marked by [B<ID>] tag with timings calculated on the corresponding level

Note that LogCastContextBranch should only be used in scope of the LogCastContext

LogCastContextBranch supports nested usage with the tree-like structure visualization

**NOTE**: Since UoW is built using CallContext, normal context capturing should be present to have your awaited method logs included into the parent UoW. 
If context capturing is disabled (and this is often referred to as a best practice to apply by .ConfigureAwait(false)) then you will have awaited 
logs cut out as a separate context

## Elapsed loggers

In order to instrument your call durations measurements and add them to the output logs conventionally and declaratively set of ElapsedLoggers is available

All of those are inherited from ElapsedLoggerBase and differ by the way measured code duration is aggregated and where in the log it is added.
Currently available are:

- ElapsedLogger - simply measures duration and adds it to the resulting log body
- DurationsElapsedLogger-inherited classes - add measured value as the resulting log named attribute inside the object of existing 
@fields.durations attribute. They differ by how several measurements within the same UoW are aggregated 
	- AllDurationsElapsedLogger
	- DurationsSumElapsedLogger
	- DurationsAverageElapsedLogger
	- DurationsMaxElapsedLogger

Simple example:

```
using (new DurationsSumElapsedLogger("operation"))
{
    // measured action
}
```

Multiple ElapsedLogger example:

```
using (new AllDurationsElapsedLogger(
    new DurationsSumElapsedLogger("operation"))))
{
    Thread.Sleep(delay);
}
```

Log results:

```
"durations": 
{
	"value": [97, 3, 36],
	"total": 143,
	"operation": 42
	"operation.total": [5, 1, 33, 3]
}
```

## Flush

Since all heavy operations are performed asynchronously you may need to synchronize with the logs outcoming 
queue. This is sometimes handy in scenarios like service gracefull stop or console script application when you
need to ensure logging subsystem has enough time to send out all it has

Code sample:

```
LogManager.Flush(TimeSpan.FromMinutes(1));
```

## Logs throttling/skipping

In some scenarios logging system may need to be instructed to skip logging current call

This can be achieved with simply skipping the particular context:

```
LogCastContext.Current.SuppressMessages = true
```

Another available approach is throttling all the logs, which can be set with SkipPercentage setting in the log options

Context that received control flow but didn't experience any log entries or custom attibute will still produce UOW by default. 
This behavior can be changed with next setting:

```
LogCastContext.Current.SuppressEmtpyContextMessages = true
```

## Credits

- **Konstantin Babiy** - main contributor and current  owner
- **Kirill Medvedev** - originally introduced the idea and started the project
- **Vladyslav Novgorodov** - former colleague who applied major redesign contribing into improving solution design and extensibility
- **7Digital backend team** - contributing teammates