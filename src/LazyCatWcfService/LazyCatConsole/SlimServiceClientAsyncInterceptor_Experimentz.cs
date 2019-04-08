
/*using Castle.DynamicProxy;
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
		IClientChannel m_ClientChannel;
		ITokenService m_TokenService;

		public SlimServiceClientAsyncInterceptor(
			TChannel dispatcher, IClientChannel clientChannel, ITokenService tokenService)
		{
			m_Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
			m_ClientChannel = clientChannel ?? throw new ArgumentNullException(nameof(clientChannel));
			m_TokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
		}

		public void InterceptSynchronous(IInvocation invocation)
		{
			if (IsServiceMethodInvocation(invocation))
			{
				string accessToken = Task.Run(async () => await m_TokenService.GetTokenAsync()).Result;

				using (var scope = new OperationContextScope(m_ClientChannel))
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
			throw new NotImplementedException();
		}

		public async void InterceptAsynchronous<TResult>(IInvocation invocation)
		{
			if (IsServiceMethodInvocation(invocation))
			{
				using (var scope = new FlowingOperationContextScope(m_ClientChannel))
				{
					string accessToken = await m_TokenService.GetTokenAsync().ContinueOnScope(scope);

					try
					{
						var httpRequestProperty = new HttpRequestMessageProperty();
						httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
							$"Bearer {accessToken}";
						OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
							httpRequestProperty;

						// base class will decrement currentInterceptorIndex right after the first await returns control, ensure that is immediate with Task.Yield
						//await Task.Yield().C;

						//invocation.Proceed();
						//var returnTask = (Task<TResult>)invocation.ReturnValue;
						//await returnTask.ContinueOnScope(scope);
						object[] parameters = invocation.Arguments;
						var method = invocation.Method;
						object result = method.Invoke(m_Dispatcher, parameters);
						var returnTask = (Task<TResult>)result;
						invocation.ReturnValue = returnTask;
						await returnTask.ContinueOnScope(scope);
					}
					catch (AggregateException ex)
					{
						// Very primitive exception handling, but good enough for Lazy Cats Studio
						var faultException = ex.GetBaseException() as FaultException;
						if (faultException?.Message == "🔒 Unrecognized Bearer.")
						{
							// Try to refresh token
							string newToken = await m_TokenService.GetTokenAsync().ContinueOnScope(scope);
							if (newToken != accessToken)
							{
								var httpRequestProperty = new HttpRequestMessageProperty();
								httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
									$"Bearer {newToken}";
								OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
									httpRequestProperty;

								//// Because awaits will make it decrease it's nice little counter before it's done, we wrap that little counter up <3
								//// Little counters need love too
								//var currentInterceptorIndexField = typeof(AbstractInvocation).GetField(
								//	"currentInterceptorIndex",
								//	BindingFlags.NonPublic | BindingFlags.Instance);
								//currentInterceptorIndexField.SetValue(this, -1);
								//var nnn = currentInterceptorIndexField.GetValue(this);


								// 2nd time it will fail = invocation.Proceed();
								//var returnTask = (Task<TResult>)invocation.ReturnValue;

								object[] parameters = invocation.Arguments;
								var method = invocation.Method;
								object result = method.Invoke(m_Dispatcher, parameters);
								var returnTask = (Task<TResult>)result;
								invocation.ReturnValue = returnTask;
								await returnTask.ContinueOnScope(scope);
							}
						}
					}
				}
			}
			else
			{
				throw new InvalidProgramException(
					"Something went wrong because only service interface has async methods");
			}
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
*/