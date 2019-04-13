using Castle.DynamicProxy;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// <para>
	/// Async interceptor for slim service client on top of "slim dispatcher".
	/// </para>
	/// <para>
	/// Applies access token via OperationContext.Current over client channel
	/// when service methods are invoked.
	/// </para>
	/// </summary>
	class SlimServiceClientAsyncInterceptor<TChannel> : AsyncInterceptorBaseHack where TChannel : class
	{
		readonly TChannel m_Dispatcher;
		readonly Func<IClientChannel> m_GetClientChannel;
		readonly ITokenService m_TokenService;

		public SlimServiceClientAsyncInterceptor(
			TChannel dispatcher, Func<IClientChannel> getClientChannel, ITokenService tokenService)
		{
			m_Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
			m_GetClientChannel = getClientChannel ?? throw new ArgumentNullException(nameof(m_GetClientChannel));
			m_TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
		}

		public override void InterceptSynchronous(IInvocation invocation)
		{
			if (!IsServiceMethodInvocation(invocation))
			{
				invocation.Proceed(); // Dispose()
				return;
			}

			// This async syntax doesn't block WinForms UI
			string accessToken = Task.Run(async () =>
				await m_TokenService.GetTokenAsync()).Result;

			var channel = m_GetClientChannel();
			using (var scope = new OperationContextScope(channel))
			{
				var httpRequestProperty = new HttpRequestMessageProperty();
				httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
					$"Bearer {accessToken}";
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
					httpRequestProperty;

				try
				{
					invocation.Proceed();
				}
				catch (FaultException ex)
				{
					// Very primitive exception handling, but good enough for Lazy Cats Studio
					if (ex.Message != "🔒 Unrecognized Bearer.")
						throw;

					// Try to refresh token and replay the invocation
					string newToken = Task.Run(async () =>
						await m_TokenService.GetTokenAsync()).Result;
					if (newToken != accessToken)
					{
						httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
							$"Bearer {newToken}";

						invocation.Proceed();
					}
				}
			}
		}

		protected override async Task<TResult> InterceptAsyncCore<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
		{
			if (!IsServiceMethodInvocation(invocation))
			{
				return await proceed(invocation);
			}

			// Tell message inspector to have token ready
			string accessToken = await m_TokenService.GetTokenAsync();

			var channel = m_GetClientChannel();
			using (var scope = new FlowingOperationContextScope(channel))
			{
				var httpRequestProperty = new HttpRequestMessageProperty();
				httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
					$"Bearer {accessToken}";
				OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
					httpRequestProperty;

				try
				{
					// Try invoke service method
					return await proceed(invocation).ContinueOnScope(scope);
				}
				catch (AggregateException ex)
				{
					// Very primitive exception handling, but good enough for Lazy Cats Studio
					if (ex.InnerException is FaultException && ex.InnerException.Message == "🔒 Unrecognized Bearer.")
					{
						// Try to refresh token
						string newToken = await m_TokenService.GetTokenAsync().ContinueOnScope(scope);
						if (newToken == accessToken)
						{
							// Unauthorized not because of token being expired but for other reasons
							throw;
						}

						httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
							$"Bearer {newToken}";

						return await proceed(invocation).ContinueOnScope(scope);
					}

					throw;
				}
			}
		}

		protected override Task InterceptAsyncCore(IInvocation invocation, Func<IInvocation, Task> proceed)
		{
			if (!IsServiceMethodInvocation(invocation))
			{
				return proceed(invocation);
			}

			// void service methods (Task in async)? I don't need them for a moment.
			throw new NotImplementedException(
				"Lazy Cat Studio doesn't implement anything more what's needed for them. Feel free to fill the gap.");
		}


		private static bool IsServiceMethodInvocation(IInvocation invocation)
		{
			var targetType = invocation.Method.DeclaringType;
			var targetInterface = typeof(TChannel).GetInterface(targetType.FullName);
			return targetInterface != null;
		}
	}
}
