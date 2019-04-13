# WCF client with OMiau authentification
Simple migration of WFC clients to support OAuth2 or whatever authentification+authorization,
including not yet standartized OMiau. It's all about setting
HTTP header _Authorization: Bearer token_ on the client side.

Use this _softare_ for fast porting of legacy WCF clients to support legacy WCF services that just
added OAuth2 (or whatever auth).
This example shows how to add support for OMiau authorization, but everything is
the same for OAuth2.

![In Lazy Cats Studio office](./AssEtc-s/WcfOMiau.jpg)

_If you can do OMiau you will better understand OAuth2._

### LazyCatWcfService.csproj
Start LazyCatWcfService project, the WCF service will be served on IIS Express at
http://localhost:41193/LazyCatService.svc

Unit (integration) tests, Console app and WinForms app will use that endpoint.

### LazyCatConsole.csproj
Contains all the guts.
The project contains xUnit tests (integration tests!), and simple console test-run
for common client usage scenarios.
2 levels of applying OMiau/OAuth authorization are shown and tested here.

__Level 0: LazyCatServiceClient / LazyCatClientFactory.CreateAnonymousAuthClient()__

Generated standard WCF client (via Add Service Reference in Visual Studio,
using Task-based async operations option). Anonymous authorization.
Then I moved all generated stuff to Service_Reference.cs and removed all
Debugger-Pass-Through-s so you and me can debug what's going on.

![](./AssEtc-s/green-box.png) Shall work because it worked for 10 years in production right?
It's more likely that you were using basic authentification not anonymous or other,
what WCF gives you out of the box.

__Level 1: LazyCatServiceOMiauManualClient / LazyCatClientFactory.CreateOMiauAuthClient()__

Manually handcrafted WCF client with OMiau authentification. Supports "source code" 
compatibility level with LazyCatServiceClient: service methods are overriden
with new operator, OMiau/OAuth2 headers are set there, inside service methods.

They are set via 
[See how much work is it to add these headers.](src/LazyCatWcfService/LazyCatConsole/LazyCatServiceOMiauManualClient.cs)
But you can write 2 helpers like WrapServiceCall and WrapServiceCallAsync to simplify work
needed to fine tune a service method call. (TODO: add wraps)

This is level 1: use it if you have few WCF services, few methods and the service
methods are quite stable and will not change in future.

![](./AssEtc-s/green-box.png) Level 1 works as it was tested at Lazy Cats Studio.

__Notes__

I have tried to set headers via message inspector according to
[code review by Matt Connew](https://github.com/dotnet/wcf/issues/3472#issuecomment-478127943)
as "more standard way" in WCF. However that approach led to ugly code to overcome issues caused
by asynchronous token usage vs syncronous method
[IClientMessageInspector.BeforeSendRequest](https://docs.microsoft.com/en-us/dotnet/api/system.servicemodel.dispatcher.iclientmessageinspector.beforesendrequest?view=netframework-4.7.2)
I created a [separate branch](https://github.com/r-pankevicius/WcfClientWithOMiauAuthentification/tree/Pass-token-via-message-inspector)
where to put this attempt.

__Readings__

__SailingRock__ (Microsoft) blog post "Using OAuth2 with SOAP". It shows that OAuth2 token can be put
in SOAP header as well as in HTTP header.
https://blogs.msdn.microsoft.com/mrochon/2015/11/19/using-oauth2-with-soap/

__Andrew Nosenko__ showed how to get away with nasty System.InvalidOperationException when calling WCF client's
Dispose in async service method - This OperationContextScope is being disposed on a different thread than it was created.
https://stackoverflow.com/a/22753055

__Level 2: ILazyCatServiceSlimClient / LazyCatClientFactory.CreateOMiauAuthSlimClient()__

"Slim" WCF service client, consists only of [service interface + IDisposable] interface to
the outside world. The implementation of service methods is hooked by interceptor
(with help of dynamic code generation), OMiau/OAuth2 headers are applied inside the hooks.

This is level 2: use it if you have many WCF services, many methods or the service methods
may change in future.

References:
https://github.com/JSkimming/Castle.Core.AsyncInterceptor

![](./AssEtc-s/green-box.png) Level 2 works as it was tested at Lazy Cats Studio.

__Readings__

__Krzysztof Koźmic__ explained how to use Castle Windsor for codegen
http://kozmic.net/category/castle/

__Level 3: It's only enough to override CreateChannel() on the class generated derived from original client__
...and everything else is made automagically. Requires more knowledge of what "transparent proxy" is
because WCF channel is transparent proxy.

![](./AssEtc-s/blue-box.png) The question 1 is if I can generate derived class?

### LazyCatWinForm.csproj
Uses service clients to replay common client usage scenarios to check that
we don't block WinForms UI with async operations.



