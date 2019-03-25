using LazyCatConsole.LazyCatServiceReference;
using System;

namespace LazyCatConsole
{
	/// <summary>
	/// Slim WCF service client consists only of [service interface + IDisposable]
	/// </summary>
	public interface ILazyCatServiceSlimClient : ILazyCatService, IDisposable
	{
	}
}
