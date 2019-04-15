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
2 ways of applying OMiau/OAuth authorization are shown and tested here.

__As is: LazyCatServiceClient / LazyCatClientFactory.CreateAnonymousAuthClient()__

Generated standard WCF client (via Add Service Reference in Visual Studio,
using Task-based async operations option). Anonymous authorization.
Then I moved all generated stuff to Service_Reference.cs and removed all
Debugger-Pass-Through-s so you and me can debug what's going on.

Just to test it works, as expected.

![](./AssEtc-s/green-box.png) Shall work because it worked for 10 years in production right?
It's more likely that you were using basic authentification not anonymous or other,
what WCF gives you out of the box.

__Manual way: LazyCatServiceOMiauManualClient / LazyCatClientFactory.CreateOMiauAuthClient()__

Manually handcrafted WCF client with OMiau authentification. Supports "source code" 
compatibility level with LazyCatServiceClient: service methods are overriden
with new operator, OMiau/OAuth2 headers are set there, inside new service methods.

Use it if you have few WCF services, few methods and the service
methods are quite stable and will not change in future.

Redesigned it according to code review by Matt Connew at
https://github.com/dotnet/wcf/issues/3472#issuecomment-478127943.
So wanted Bearer HTTP header with access token is set in message inspector that
is inside ugly OMiauChannelHandler class. The ugliness here comes from the fact
that IClientMessageInspector.BeforeSendRequest() method is sync while our token
service is async. This forced me to make client know about "service internals".
The client has to call OMiauChannelHandler.RefreshTokenAsync() exactly when needed
to make stuff work.

![](./AssEtc-s/green-box.png) Works as it was tested at Lazy Cats Studio.

References:
https://docs.microsoft.com/en-us/dotnet/framework/wcf/extending/how-to-inspect-or-modify-messages-on-the-client

__Codegen way: ILazyCatServiceSlimClient / LazyCatClientFactory.CreateOMiauAuthSlimClient()__

"Slim" WCF service client, consists only of [service interface + IDisposable] interface to
the outside world. The implementation of service methods is hooked by interceptors (two!)
(with help of dynamic code generation), OMiau/OAuth2 headers are applied inside the hooks.

This is more complex way, it adds dependency on code generation libraries (and their bugs),
use with care. If you have many WCF services, many methods therefore or the service methods
may change in future.

References:

__Krzysztof Koźmic__ explained how to use Castle Windsor for codegen
http://kozmic.net/category/castle/

Also:
https://github.com/JSkimming/Castle.Core.AsyncInterceptor

![](./AssEtc-s/green-box.png) Works as it was tested at Lazy Cats Studio.

### LazyCatWinForm.csproj
Uses service clients to replay common client usage scenarios to check that
we don't block WinForms UI with async operations.

