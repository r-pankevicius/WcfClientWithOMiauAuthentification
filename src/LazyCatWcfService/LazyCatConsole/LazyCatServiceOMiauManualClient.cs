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
			// This async syntax doesn't block WinForms UI
			string accessToken = Task.Run(async () => await m_TokenService.GetTokenAsync()).Result;

			// For more information how to apply access token see
			// https://blogs.msdn.microsoft.com/mrochon/2015/11/19/using-oauth2-with-soap/
			using (var scope = new OperationContextScope(InnerChannel))
			{
				var httpRequestProperty = new HttpRequestMessageProperty();
				httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
					$"Bearer {accessToken}";
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
					httpRequestProperty;

				try
				{
					return base.SumWithOMiauAuth(a, b);
				}
				catch (FaultException ex)
				{
					if (Helpers.CouldBeExpiredTokenException(ex))
					{
						// Try to refresh token
						string newToken = Task.Run(async () =>
							await m_TokenService.GetTokenAsync()).Result;
						if (newToken != accessToken)
						{
							httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
								$"Bearer {newToken}";
							return base.SumWithOMiauAuth(a, b);
						}
					}

					throw;
				}
			}
		}

		public new async Task<int> SumWithOMiauAuthAsync(int a, int b)
		{
			string accessToken = await m_TokenService.GetTokenAsync();

			// Using OperationContextScope in "async Dispose" scenario
			// will cause System.InvalidOperationException :
			// This OperationContextScope is being disposed on a different thread than it was created.
			// See https://stackoverflow.com/a/22753055 how to get away with it.
			using (var scope = new FlowingOperationContextScope(InnerChannel))
			{
				var httpRequestProperty = new HttpRequestMessageProperty();
				httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
					$"Bearer {accessToken}";
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
					httpRequestProperty;

				try
				{
					return await base.SumWithOMiauAuthAsync(a, b).ContinueOnScope(scope);
				}
				catch (AggregateException ex)
				{
					if (Helpers.CouldBeExpiredTokenException(ex.InnerException))
					{
						// Try to refresh token
						string newToken = await m_TokenService.GetTokenAsync().ContinueOnScope(scope);
						if (newToken != accessToken)
						{
							httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
								$"Bearer {newToken}";
							return await base.SumWithOMiauAuthAsync(a, b).ContinueOnScope(scope);
						}
					}

					throw;
				}
			}
		}
	}
}