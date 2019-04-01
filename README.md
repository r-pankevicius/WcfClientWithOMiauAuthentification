# WCF client with OMiau authentification
Simple migration of WFC clients to support OAuth2 or whatever authentification,
including not yet standartized OMiau authentification.

Use for fast porting of legacy WCF clients to support legacy WCF services that just
added OAuth2 (or whatever authorization).
This example shows how to add support for OMiau authentification, but everything is
the same for OAuth2.

![In Lazy Cats Studio office](./AssEtc-s/WcfOMiau.jpg)

_If you can do OMiau you will better understand OAuth2._

_But if you can't OMiau I would not trust you to secure my credit card with your loose knowledge about OAuth2._

### LazyCatWcfService.csproj
Start LazyCatWcfService project, the WCF service will be served on IIS Express at
http://localhost:41193/LazyCatService.svc

### LazyCatConsole.csproj
Contains all the guts.
The project contains xUnit tests, and simple console test-run for common client usage scenarios.
2 levels of applying OMiau/OAuth authorization are shown and tested here.

__LazyCatServiceClient / LazyCatClientFactory.CreateAnonymousAuthClient()__

Generated standard WCF client (via Add Service Reference in Visual Studio,
using Task-based async operations option). Anonymous authorization.
Then I moved all generated stuff to Service_Reference.cs and removed all
Debugger-Pass-Through-s so you and me can debug what's going on._

![](./AssEtc-s/green-box.png) Shall work because it worked for 10 years in production right?
It's more likely that you used not anonymous authentification, but basic or other,
what WCF gives you out of the box.

![](./AssEtc-s/green-box.png) Shall work because it worked for 10 years in production right?
It's more likely that you used not anonymous authentification, but basic or other,
what WCF gives you out of the box.

__LazyCatServiceOMiauManualClient / LazyCatClientFactory.CreateOMiauAuthClient()__

Manually handcrafted WCF client with OMiau authentification. Supports "source code" 
compatibility level with LazyCatServiceClient: service methods are overriden
with new operator, OMiau/OAuth2 headers are set there, inside new service methods.

This is level 1: use it if you have few WCF services, few methods and the service
methods are quite stable and will not change in future.

![](./AssEtc-s/green-box.png) Level 1 works as it was tested at Lazy Cats Studio.

__ILazyCatServiceSlimClient / LazyCatClientFactory.CreateOMiauAuthSlimClient()__

"Slim" WCF service client, consists only of [service interface + IDisposable] interface to
the outside world. The implementation of service methods is hooked by interceptor
(with help of dynamic code generation), OMiau/OAuth2 headers are applied inside the hooks.

This is level 2: use it if you have many WCF services, many methods or the service methods
may change in future.

![](./AssEtc-s/red-box.png) Level 2 pretends to work but doesn't work as it should, actually.

To fix, RTFM: https://github.com/JSkimming/Castle.Core.AsyncInterceptor/tree/hacking-for-async-fix

To fix, Read code review by Matt Connew at https://github.com/dotnet/wcf/issues/3472#issuecomment-478127943

### LazyCatWinForm.csproj
Uses service clients to replay common client usage scenarios to check that
we don't block WinForms UI with async operations.

## It's a soup of ingredients I found on Web

__SailingRock__ (Microsoft) blog post "Using OAuth2 with SOAP". It shows that OAuth2 token can be put
in SOAP header as well as in HTTP header (I used later for OMiau).
https://blogs.msdn.microsoft.com/mrochon/2015/11/19/using-oauth2-with-soap/

__Andrew Nosenko__ showed how to get away with nasty System.InvalidOperationException when calling WCF client's
Dispose in async service method - This OperationContextScope is being disposed on a different thread than it was created.
https://stackoverflow.com/a/22753055

__Krzysztof Koźmic__ explained how to use Castle Windsor for codegen
http://kozmic.net/category/castle/
