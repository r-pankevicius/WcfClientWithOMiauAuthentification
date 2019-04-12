# WCF client with OMiau authentification
Simple migration of WFC clients to support OAuth2 or whatever authentification,
including not yet standartized OMiau authentification.

Use for fast porting of legacy WCF clients to support legacy WCF services that just
added OAuth2 (or whatever authorization).
This example shows how to add support for OMiau authentification, but everything is
the same for OAuth2.

![In Lazy Cats Studio office](./AssEtc-s/WcfOMiau.jpg)

_If you can do OMiau you will better understand OAuth2._

### LazyCatWcfService.csproj
Start LazyCatWcfService project, the WCF service will be served on IIS Express at
http://localhost:41193/LazyCatService.svc

Unit (integration) tests, Console app and WinForms app will use that endpoint.

### LazyCatConsole.csproj
Contains all the guts.
The project contains xUnit tests (integration test!), and simple console test-run
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
with new operator, OMiau/OAuth2 headers are set there, inside new service methods.

This is level 1: use it if you have few WCF services, few methods and the service
methods are quite stable and will not change in future.

So wanted Bearer HTTP header with access token is set in message inspector that
is inside ugly OMiauChannelHandler class. The ugliness here comes from the fact
that IClientMessageInspector.BeforeSendRequest() method is sync while our token
service is async. This forced me to make client know about "service internals".
The client has to call OMiauChannelHandler.RefreshTokenAsync() exactly when needed
to make stuff work.

![](./AssEtc-s/green-box.png) Level 1 works as it was tested at Lazy Cats Studio.

References:
https://docs.microsoft.com/en-us/dotnet/framework/wcf/extending/how-to-inspect-or-modify-messages-on-the-client

__Level 2: ILazyCatServiceSlimClient / LazyCatClientFactory.CreateOMiauAuthSlimClient()__

"Slim" WCF service client, consists only of [service interface + IDisposable] interface to
the outside world. The implementation of service methods is hooked by interceptor
(with help of dynamic code generation), OMiau/OAuth2 headers are applied inside the hooks.

This is level 2: use it if you have many WCF services, many methods or the service methods
may change in future.

Redesigned it according to code review by Matt Connew at
https://github.com/dotnet/wcf/issues/3472#issuecomment-478127943 ,
and that "Slim" client is not so much slim as I wanted it to be: entirely code generated
given a service + IDispose interface. You need to create a base class like a 
LazyCatServiceClientOnOMiauChannel for each service client.

References:
https://github.com/JSkimming/Castle.Core.AsyncInterceptor

![](./AssEtc-s/green-box.png) Level 2 works as it was tested at Lazy Cats Studio.

__Level 3: It's only enough to override CreateChannel() on the class derived from original client__
...and everything else is made automagically. Requires more knowledge of what "transparent proxy" is
because WCF channel is transparent proxy.

![](./AssEtc-s/blue-box.png) No value, because you will need to have at least one constructor in derived
class and it will be of the same value as Level 2.

### LazyCatWinForm.csproj
Uses service clients to replay common client usage scenarios to check that
we don't block WinForms UI with async operations.

## Other links

__Krzysztof Koźmic__ explained how to use Castle Windsor for codegen
http://kozmic.net/category/castle/
