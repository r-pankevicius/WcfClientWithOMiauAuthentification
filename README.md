# WCF client with OMiau authentification
Simple migration of WFC clients to support OAuth2 or whatever authentification,
including not yet standartized OMiau authentification.

_If you can do OMiau you will better understand OAuth2._

### LazyCatWcfService.csproj
Start LazyCatWcfService project, the WCF service will be served on IIS Express at
http://localhost:41193/LazyCatService.svc

### LazyCatConsole.csproj
Contains all the guts.

__LazyCatServiceClient / LazyCatClientFactory.CreateAnonymousAuthClient()__

Generated WCF client (via Add Service Reference in Visual Studio,
using Task-based async operations option).

__LazyCatServiceOMiauManualClient / LazyCatClientFactory.CreateOMiauAuthClient()__

Manually handcrafted WCF client with OMiau authentification. Supports "source code" 
compatibility level with LazyCatServiceClient.

The project contains xUnit tests, and simple console test-run for common client usage scenarios.

### LazyCatWinForm.csproj
Uses service clients to replay common client usage scenarios to check that
we don't block WinForms UI with async operations.
