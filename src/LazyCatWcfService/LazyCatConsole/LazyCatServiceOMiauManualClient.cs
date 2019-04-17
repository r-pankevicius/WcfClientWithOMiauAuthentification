using LazyCatConsole.LazyCatServiceReference;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Lazy Cat Service client with manually added support for OAMiau authentification
	/// headers in overwritten methods.
	/// Source code level compatibility, overriden methods.
	/// It's better to fine tune source code of generated WCF client to replace origina methods with something
	/// like this.
	/// </summary>
	public class LazyCatServiceOMiauManualClient : LazyCatServiceClient
	{
		readonly ITokenService m_TokenService;

		public LazyCatServiceOMiauManualClient(Binding binding, EndpointAddress address, ITokenService tokenService) :
			base(binding, address)
		{
			m_TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
		}

		public new int SumWithOMiauAuth(int a, int b)
		{
			return SharedStuff.WrapServiceMethodCall(
				() => base.SumWithOMiauAuth(a, b), InnerChannel, m_TokenService);
		}

		public new async Task<int> SumWithOMiauAuthAsync(int a, int b)
		{
			return await SharedStuff.WrapServiceMethodCallAsync(
				() => base.SumWithOMiauAuthAsync(a, b), InnerChannel, m_TokenService);
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
			Func<TResult> invokeServiceMethod, IClientChannel clientChannel, ITokenService tokenService)
		{
			string accessToken = Task.Run(async () => await tokenService.GetTokenAsync()).Result;

			// For more information how to apply access token see
			// https://blogs.msdn.microsoft.com/mrochon/2015/11/19/using-oauth2-with-soap/
			using (var scope = new OperationContextScope(clientChannel))
			{
				var httpRequestProperty = new HttpRequestMessageProperty();
				httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
					$"Bearer {accessToken}";
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
					httpRequestProperty;

				try
				{
					return invokeServiceMethod();
				}
				catch (FaultException ex)
				{
					if (Helpers.CouldBeExpiredTokenException(ex))
					{
						// Try to refresh token
						string newToken = Task.Run(async () =>
							await tokenService.GetTokenAsync()).Result;
						if (newToken != accessToken)
						{
							httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
								$"Bearer {newToken}";
							return invokeServiceMethod();
						}
					}

					throw;
				}
			}
		}

		/// <summary>
		/// Async service method call
		/// </summary>
		public static async Task<TResult> WrapServiceMethodCallAsync<TResult>(
			Func<Task<TResult>> invokeServiceMethod, IClientChannel clientChannel, ITokenService tokenService)
		{
			string accessToken = await tokenService.GetTokenAsync();

			// Using OperationContextScope in "async Dispose" scenario
			// will cause System.InvalidOperationException :
			// This OperationContextScope is being disposed on a different thread than it was created.
			// See https://stackoverflow.com/a/22753055 how to get away with it.
			using (var scope = new FlowingOperationContextScope(clientChannel))
			{
				var httpRequestProperty = new HttpRequestMessageProperty();
				httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
					$"Bearer {accessToken}";
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
					httpRequestProperty;

				try
				{
					return await invokeServiceMethod().ContinueOnScope(scope);
				}
				catch (AggregateException ex)
				{
					if (Helpers.CouldBeExpiredTokenException(ex.InnerException))
					{
						// Try to refresh token
						string newToken = await tokenService.GetTokenAsync().ContinueOnScope(scope);
						if (newToken != accessToken)
						{
							httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
								$"Bearer {newToken}";
							return await invokeServiceMethod().ContinueOnScope(scope);
						}
					}

					throw;
				}
			}
		}

	}
}