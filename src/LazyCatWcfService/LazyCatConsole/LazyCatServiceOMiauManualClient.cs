using LazyCatConsole.LazyCatServiceReference;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	/// <summary>
	/// Lazy Cat Service client with manually added support for OAMiau.
	/// Service methods are overriden with new operator, OMiau headers are set by ChannelKeeper
	/// message inspector.
	/// Source code level compatibility, overriden methods.
	/// It's better to fine tune source code of generated WCF client to replace origina methods with something
	/// like this.
	/// </summary>
	public class LazyCatServiceOMiauManualClient : LazyCatServiceClientOnOMiauChannel
	{
		public LazyCatServiceOMiauManualClient(Binding binding, EndpointAddress address, ITokenService tokenService) :
			base(binding, address, tokenService)
		{
		}

		public new int SumWithOMiauAuth(int a, int b)
		{
			// This async syntax doesn't block WinForms UI
			string accessToken = Task.Run(async () => await ChannelHandler.RefreshTokenAsync()).Result;

			try
			{
				return base.SumWithOMiauAuth(a, b);
			}
			catch (FaultException ex)
			{
				// Very primitive exception handling, but good enough for Lazy Cats Studio
				if (ex.Message == "🔒 Unrecognized Bearer.")
				{
					// Try to refresh token
					string newToken = Task.Run(async () => await ChannelHandler.RefreshTokenAsync()).Result;
					if (newToken != accessToken)
					{
						return base.SumWithOMiauAuth(a, b);
					}
				}

				throw;
			}
		}

		public new async Task<int> SumWithOMiauAuthAsync(int a, int b)
		{
			// This async syntax doesn't block WinForms UI
			string accessToken = await ChannelHandler.RefreshTokenAsync();

			try
			{
				return base.SumWithOMiauAuth(a, b);
			}
			catch (FaultException ex)
			{
				// Very primitive exception handling, but good enough for Lazy Cats Studio
				if (ex.Message == "🔒 Unrecognized Bearer.")
				{
					// Try to refresh token
					string newToken = await ChannelHandler.RefreshTokenAsync();
					if (newToken != accessToken)
					{
						return base.SumWithOMiauAuth(a, b);
					}
				}

				throw;
			}
		}
	}
}