using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace LazyCatConsole
{
	/// <summary>
	/// Angry because of strugling with
	/// https://github.com/JSkimming/Castle.Core.AsyncInterceptor/issues/25
	/// </summary>
	class AngryCatAsyncInterceptor<TChannel> : AsyncInterceptorBaseHack where TChannel : class
	{
		IServiceClientOnOMiauChannel<TChannel> m_Client;

		public AngryCatAsyncInterceptor(IServiceClientOnOMiauChannel<TChannel> client)
		{
			m_Client = client ?? throw new ArgumentNullException(nameof(client));
		}

		public override void InterceptSynchronous(IInvocation invocation)
		{
			if (!IsServiceMethodInvocation(invocation))
			{
				invocation.Proceed();
			}

			// This async syntax doesn't block WinForms UI
			string accessToken = Task.Run(async () =>
				await m_Client.ChannelHandler.RefreshTokenAsync()).Result;

			try
			{
				// Try invoke service method
				invocation.Proceed();
			}
			catch (FaultException ex)
			{
				// Very primitive exception handling, but good enough for Lazy Cats Studio
				if (ex.Message != "🔒 Unrecognized Bearer.")
					throw;

				// Try to refresh token
				string newToken = Task.Run(async () =>
					await m_Client.ChannelHandler.RefreshTokenAsync()).Result;
				if (newToken == accessToken)
				{
					// Unauthorized not because of token being expired but for other reasons
					throw;
				}

				// Invoke service method with fresh token
				invocation.Proceed();
			}
		}

		protected override async Task<TResult> InterceptAsyncCore<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
		{
			if (!IsServiceMethodInvocation(invocation))
			{
				invocation.Proceed();
				return await proceed(invocation);
			}

			// Tell message inspector to have token ready
			string accessToken = await m_Client.ChannelHandler.RefreshTokenAsync();

			try
			{
				// Try invoke service method
				return await proceed(invocation);
			}
			catch (FaultException ex)
			{
				// Very primitive exception handling, but good enough for Lazy Cats Studio
				if (ex.Message != "🔒 Unrecognized Bearer.")
					throw;

				// Try to refresh token
				string newToken = Task.Run(async () =>
					await m_Client.ChannelHandler.RefreshTokenAsync()).Result;
				if (newToken == accessToken)
				{
					// Unauthorized not because of token being expired but for other reasons
					throw;
				}

				// Invoke service method with fresh token
				return await proceed(invocation);
			}
		}


#if false
		protected override Task<TResult> InterceptAsyncCore<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
		{
			if (!IsServiceMethodInvocation(invocation))
			{
				invocation.Proceed();
				return proceed(invocation);
			}

			Task<string> getTokenReadyTask = m_Client.ChannelHandler.RefreshTokenAsync();
			Task<TResult> invoke1 = getTokenReadyTask.ContinueWith((previous) =>
			{
				var serviceCall = proceed(invocation);
				serviceCall.Wait();
				return serviceCall.Result;
			});

			Task<string> refreshTokenTaskIfNeeded = invoke1.ContinueWith((previous) =>
			{
				if (previous.IsFaulted)
				{
					Task<string> ts = m_Client.ChannelHandler.RefreshTokenAsync();
					ts.Wait();
					string newToken = ts.Result;
					if (newToken == getTokenReadyTask.Result)
						throw previous.Exception.Flatten();

					return "REPLAY";
				}

				return "OK";
			});

			Task<TResult> endChain = refreshTokenTaskIfNeeded.ContinueWith((previous) =>
			{
				if (previous.IsFaulted)
				{
					throw previous.Exception.Flatten();
				}

				if (previous.Result == "OK")
				{
					return invoke1.Result;
				}

				// Replay with new token
				var serviceCall = proceed(invocation);
				serviceCall.Wait();
				return serviceCall.Result;
			});

			return endChain;
		}
#endif

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
			return typeof(TChannel).Equals(targetType);
		}
	}
}
