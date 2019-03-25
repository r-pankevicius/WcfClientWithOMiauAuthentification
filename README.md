# WCF client with OMiau authentification
Simple migration of WFC clients to support OAuth2 or whatever authentification,
including not yet standartized OMiau authentification.

Use for porting legacy WCF clients to support OAuth2 (or whatever).
This example shows how to add support for OMiau authentification, but everything is
the same for OAuth2.

_If you can do OMiau you will better understand OAuth2._

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

__LazyCatServiceOMiauManualClient / LazyCatClientFactory.CreateOMiauAuthClient()__

Manually handcrafted WCF client with OMiau authentification. Supports "source code" 
compatibility level with LazyCatServiceClient: service methods are overriden
with new operator, OMiau/OAuth2 headers are set there, inside new service methods.

This is level 1: use it if you have few WCF services, few methods and the service
methods are quite stable and will not change in future.

__ILazyCatServiceSlimClient / LazyCatClientFactory.CreateOMiauAuthSlimClient()__

"Slim" WCF service client, consists only of [service interface + IDisposable] interface to
the outside world. The implementation of service methods is hooked by interceptor
(with help of dynamic code generation), OMiau/OAuth2 headers are applied inside the hooks.

This is level 2: use it if you have many WCF services, many methods or the service methods
may change in future.

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
