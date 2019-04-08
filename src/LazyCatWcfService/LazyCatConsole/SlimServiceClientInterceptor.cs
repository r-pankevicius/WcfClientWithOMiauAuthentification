using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// The capture to be invoked by codegen when WCF service method
	/// is called.
	/// At invocation of the method we will append HTTP header needed for OMiau (or OAuth2):
	/// Authorization: Bearer super-secret-token
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	internal class SlimServiceClientInterceptor<TChannel> : IInterceptor where TChannel : class
	{
		ClientBase<TChannel> m_RealClient;
		ITokenService m_TokenService;

		public SlimServiceClientInterceptor(ClientBase<TChannel> realClient, ITokenService tokenService)
		{
			m_RealClient = realClient;
			m_TokenService = tokenService;
		}

		public void Intercept(IInvocation invocation)
		{
			Type targetType = invocation.Method.DeclaringType;
			Type realType = m_RealClient.GetType();
			bool assignable = targetType.IsAssignableFrom(realType);
			if (!assignable)
			{
				throw new InvalidProgramException("Something went wrong :(");
			}

			Type realClientInterface = realType.GetInterface(targetType.FullName);
			MethodInfo[] realMethods = realClientInterface.GetMethods();
			var methodToInvoke = realMethods.First(m => m.Name.Equals(invocation.Method.Name));

			if (realClientInterface.Equals(typeof(TChannel)))
			{
				// Here we have captured a call to the service.
				// It's not needed for OMiau to apply token for each method, but
				// for OAuth2 every method in the service is secured the same way, so let's
				// get token and set Authorization header.

				//
				// If there are issues with async, wrap this wrapper again with
				// help of Castle.Core.AsyncInterceptor
				// (Double wrapped codegen with codegen-ed service channel:
				// mind blowing triple wrapper.)

				string accessToken = Task.Run(async () => await m_TokenService.GetTokenAsync()).Result;

				using (var scope = new OperationContextScope(m_RealClient.InnerChannel))
				{
					var httpRequestProperty = new HttpRequestMessageProperty();
					httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] =
						$"Bearer {accessToken}";
					OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] =
						httpRequestProperty;

					var methodResult = methodToInvoke.Invoke(m_RealClient, invocation.Arguments);
					invocation.ReturnValue = methodResult;
				}
			}
			else
			{
				var methodResult = methodToInvoke.Invoke(m_RealClient, invocation.Arguments);
				invocation.ReturnValue = methodResult;
			}
		}
	}
}