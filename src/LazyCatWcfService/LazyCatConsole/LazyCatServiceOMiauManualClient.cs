using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Lazy Cat Service client with manually added support for OAMiau.
	/// Service methods are overriden with new operator, OMiau headers are set by ChannelHandler
	/// message inspector.
	/// Source code level compatibility, overriden methods.
	/// It's better to fine tune source code of generated WCF client to replace origina methods with something
	/// like this.
	/// </summary>
	public class LazyCatServiceOMiauManualClient : LazyCatServiceClientOnOMiauChannel
	{
		public LazyCatServiceOMiauManualClient(Binding binding, EndpointAddress address, ITokenService tokenService) :
			base(binding, address, tokenService)
		{
		}

		public new int SumWithOMiauAuth(int a, int b)
		{
			return SharedStuff.WrapServiceMethodCall(
				() => base.SumWithOMiauAuth(a, b), ChannelHandler);
		}

		public new async Task<int> SumWithOMiauAuthAsync(int a, int b)
		{
			return await SharedStuff.WrapServiceMethodCallAsync(
				() => base.SumWithOMiauAuthAsync(a, b), ChannelHandler);
		}
	}

	// These methods can be reused for other services, move to a separate file.
	// I keep them here to show plumbing needed for service methods in one file.
	static class SharedStuff
	{
		/// <summary>
		/// Sync service method call
		/// </summary>
		public static TResult WrapServiceMethodCall<TResult>(
			Func<TResult> invokeServiceMethod, ITokenHandler tokenHandler)
		{
			string accessToken = Task.Run(async () => await tokenHandler.MakeTokenReadyAsync()).Result;

			try
			{
				return invokeServiceMethod();
			}
			catch (FaultException ex)
			{
				// Very primitive exception handling, but good enough for Lazy Cats Studio
				if (ex.Message == "🔒 Unrecognized Bearer.")
				{
					// Try to refresh token
					string newToken = Task.Run(async () => await tokenHandler.MakeTokenReadyAsync(refreshNeeded: true)).Result;
					if (newToken != accessToken)
					{
						return invokeServiceMethod();
					}
				}

				throw;
			}
		}

		/// <summary>
		/// Async service method call
		/// </summary>
		public static async Task<TResult> WrapServiceMethodCallAsync<TResult>(
			Func<Task<TResult>> invokeServiceMethod, ITokenHandler tokenHandler)
		{
			string accessToken = await tokenHandler.MakeTokenReadyAsync();

			try
			{
				return await invokeServiceMethod();
			}
			catch (FaultException ex)
			{
				// Very primitive exception handling, but good enough for Lazy Cats Studio
				if (ex.Message == "🔒 Unrecognized Bearer.")
				{
					// Try to refresh token
					string newToken = await tokenHandler.MakeTokenReadyAsync(refreshNeeded: true);
					if (newToken != accessToken)
					{
						return await invokeServiceMethod();
					}
				}

				throw;
			}
		}
	}
}