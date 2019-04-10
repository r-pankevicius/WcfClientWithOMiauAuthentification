using Castle.DynamicProxy;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	internal class SlimServiceClientAsyncInterceptor<TChannel> : IAsyncInterceptor where TChannel : class
	{
		TChannel m_Dispatcher;
		Func<IClientChannel> m_GetClientChannel;
		ITokenService m_TokenService;

		public SlimServiceClientAsyncInterceptor(
			TChannel dispatcher, Func<IClientChannel> getClientChannel, ITokenService tokenService)
		{
			m_Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
			m_GetClientChannel = getClientChannel ?? throw new ArgumentNullException(nameof(m_GetClientChannel));
			m_TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
		}

		public void InterceptSynchronous(IInvocation invocation)
		{
			if (IsServiceMethodInvocation(invocation))
			{
				string accessToken = Task.Run(async () => await m_TokenService.GetTokenAsync()).Result;

				var channel = m_GetClientChannel();
				using (var scope = new OperationContextScope(channel))
				{
					try
					{
						var httpRequestProperty = new HttpRequestMessageProperty();
						httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
							$"Bearer {accessToken}";
						OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
							httpRequestProperty;

						invocation.Proceed();
					}
					catch (FaultException ex)
					{
						// Very primitive exception handling, but good enough for Lazy Cats Studio
						if (ex.Message == "🔒 Unrecognized Bearer.")
						{
							// Try to refresh token and replay the invocation
							string newToken = Task.Run(async () => await m_TokenService.GetTokenAsync()).Result;
							if (newToken != accessToken)
							{
								var httpRequestProperty = new HttpRequestMessageProperty();
								httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
									$"Bearer {newToken}";
								OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
									httpRequestProperty;

								invocation.Proceed();
							}
						}

						throw;
					}
				}
			}
			else
			{
				invocation.Proceed(); // Dispose()
			}
		}

		public void InterceptAsynchronous(IInvocation invocation)
		{
			throw new NotImplementedException(
				"Lazy Cat Studio doesn't implement anything more what's needed for them. Feel free to fill the gap.");
		}

		public async void InterceptAsynchronous<TResult>(IInvocation invocation)
		{
			invocation.Proceed();
			/*
			if (IsServiceMethodInvocation(invocation))
			{
				string accessToken = await m_TokenService.GetTokenAsync();

				var channel = m_GetClientChannel();
				using (var scope = new OperationContextScope(channel))
				{
					invocation.Proceed();
				}
			}
			else
			{
				throw new InvalidProgramException(
					"Something went wrong because only service interface has async methods");
			}
			*/
		}


		#region Implementation

		private static bool IsServiceMethodInvocation(IInvocation invocation)
		{
			var targetType = invocation.Method.DeclaringType;
			var targetInterface = typeof(TChannel).GetInterface(targetType.FullName);
			return targetInterface != null;
		}

		#endregion
	}

}
