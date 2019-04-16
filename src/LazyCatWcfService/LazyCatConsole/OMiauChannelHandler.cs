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
	/// Interface to the message inspector to instruct it
	/// about situation with tokens.
	/// </summary>
	public interface ITokenHandler
	{
		/// <summary>
		/// Instructs to have token ready.
		/// </summary>
		/// <returns>Token</returns>
		Task<string> MakeTokenReadyAsync(bool refreshNeeded = false);

		string CurrentToken { get; }
	}

	/// <summary>
	/// Ugly class to implement message inspector that will set Bearer header on call to the service.
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	public class OMiauChannelHandler<TChannel> : ITokenHandler
	{
		readonly ChannelFactory<TChannel> m_Factory;
		readonly ITokenService m_TokenService;

		public OMiauChannelHandler(string endpointUrl, ITokenService tokenService) :
			this(Helpers.MakeSoap11BindingWithAnonymousAuth(new Uri(endpointUrl)),
				new EndpointAddress(endpointUrl),
				tokenService)
		{
		}

		public OMiauChannelHandler(Binding binding, EndpointAddress endpointAddress, ITokenService tokenService)
		{
			m_TokenService = tokenService;

			m_Factory = new ChannelFactory<TChannel>(binding, endpointAddress);

			IEndpointBehavior behavior = new EndpointBehavior(this);
			m_Factory.Endpoint.Behaviors.Add(behavior);
		}

		public TChannel CreateChannel()
		{
			return m_Factory.CreateChannel();
		}

		public string CurrentToken { get; set; }

		public async Task<string> MakeTokenReadyAsync(bool refreshNeeded = false)
		{
			CurrentToken = await m_TokenService.GetTokenAsync(refreshNeeded);
			return CurrentToken;
		}

		public class MessageInspector : IClientMessageInspector
		{
			readonly ITokenHandler m_TokenHandler;

			public MessageInspector(ITokenHandler accessToToken)
			{
				m_TokenHandler = accessToToken ?? throw new ArgumentNullException(nameof(accessToToken));
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
					$"Bearer {m_TokenHandler.CurrentToken}";

				request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestProperty);

				return null;
			}
		}

		public class EndpointBehavior : IEndpointBehavior
		{
			readonly ITokenHandler m_TokenHandler;

			public EndpointBehavior(ITokenHandler accessToToken)
			{
				m_TokenHandler = accessToToken ?? throw new ArgumentNullException(nameof(accessToToken));
			}

			public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
			{
			}

			public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
			{
				clientRuntime.MessageInspectors.Add(new MessageInspector(m_TokenHandler));
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
