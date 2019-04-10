using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Has token ready, when needed for message inspector.
	/// </summary>
	interface IHaveToken
	{
		string Token { get; }
	}

	/// <summary>
	/// Ugly class to implement message inspector that will set Bearer header on call to the service.
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	class ChannelKeeper<TChannel> : IDisposable, IHaveToken
	{
		readonly ChannelFactory<TChannel> m_Factory;
		readonly ITokenService m_TokenService;

		public ChannelKeeper(string endpointUrl, ITokenService tokenService) :
			this(Helpers.MakeSoap11BindingWithAnonymousAuth(new Uri(endpointUrl)),
				new EndpointAddress(endpointUrl),
				tokenService)
		{
		}

		public ChannelKeeper(Binding binding, EndpointAddress endpointAddress, ITokenService tokenService)
		{
			m_TokenService = tokenService;

			m_Factory = new ChannelFactory<TChannel>(binding, endpointAddress);

			IEndpointBehavior behavior = new EndpointBehavior(this);
			m_Factory.Endpoint.Behaviors.Add(behavior);
		}

		public string Token { get; private set; }

		public TChannel CreateChannel()
		{
			return m_Factory.CreateChannel();
		}

		public async Task<string> RefreshTokenAsync()
		{
			Token = await m_TokenService.GetTokenAsync();
			return Token;
		}

		public void Dispose()
		{
		}

		public class MessageInspector : IClientMessageInspector
		{
			IHaveToken m_MyToken;

			public MessageInspector(IHaveToken accessToToken)
			{
				m_MyToken = accessToToken ?? throw new ArgumentNullException(nameof(accessToToken));
			}

			public void AfterReceiveReply(ref Message reply, object correlationState)
			{
			}

			public object BeforeSendRequest(ref Message request, IClientChannel channel)
			{
				// Token shall be prepared by container at this moment because
				// this method is not async.
				var httpRequestProperty = new HttpRequestMessageProperty();
				httpRequestProperty.Headers[HttpRequestHeader.Authorization] =
					$"Bearer {m_MyToken.Token}";

				request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestProperty);

				return null;
			}
		}

		public class EndpointBehavior : IEndpointBehavior
		{
			IHaveToken m_MyToken;

			public EndpointBehavior(IHaveToken accessToToken)
			{
				m_MyToken = accessToToken ?? throw new ArgumentNullException(nameof(accessToToken));
			}

			public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
			{
			}

			public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
			{
				clientRuntime.MessageInspectors.Add(new MessageInspector(m_MyToken));
			}

			public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
			{
			}

			public void Validate(ServiceEndpoint endpoint)
			{
			}
		}
	}
}
