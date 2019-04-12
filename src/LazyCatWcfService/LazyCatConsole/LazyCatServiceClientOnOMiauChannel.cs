using LazyCatConsole.LazyCatServiceReference;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace LazyCatConsole
{
	public interface IServiceClientOnOMiauChannel<TChannel>
	{
		OMiauChannelHandler<TChannel> ChannelHandler { get; }
	}

	public class LazyCatServiceClientOnOMiauChannel :
		LazyCatServiceClient, ILazyCatServiceSlimClient, IServiceClientOnOMiauChannel<ILazyCatService>
	{
		public OMiauChannelHandler<ILazyCatService> ChannelHandler { get; private set; }

		public LazyCatServiceClientOnOMiauChannel(
			Binding binding, EndpointAddress address, ITokenService tokenService) :
			base(binding, address)
		{
			ChannelHandler = new OMiauChannelHandler<ILazyCatService>(binding, address, tokenService);
		}

		protected override ILazyCatService CreateChannel()
		{
			// Method is overriden to return channel with message inspector
			return ChannelHandler.CreateChannel();
		}
	}
}
