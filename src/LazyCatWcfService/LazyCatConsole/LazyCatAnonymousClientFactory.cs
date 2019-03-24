using LazyCatConsole.LazyCatServiceReference;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace LazyCatConsole
{
	/// <summary>
	/// Creates anonymous clients, e.g. clients as they were generated.
	/// </summary>
	static class LazyCatAnonymousClientFactory
	{
		public static LazyCatServiceClient Create(string endpointUrl)
		{
			var uri = new Uri(endpointUrl);
			return new LazyCatServiceClient(
				MakeSoap11BindingWithAnonymousAuth(uri), new EndpointAddress(uri));
		}

		//
		// About MessageVersion.Soap11 vs MessageVersion.Soap12
		//
		// The remote server returned an error: (415) Cannot process the message because the content type
		// 'application/soap+xml; charset=utf-8; action="http://tempuri.org/ISleepingCatService/SmartMathSum"'
		// was not the expected type 'text/xml; charset=utf-8'
		// https://stackoverflow.com/a/8250304
		// ... this is usually a mismatch in the client/server bindings,
		// where the message version in the service uses SOAP 1.2 (which expects application/soap+xml)
		// and the version in the client uses SOAP 1.1 (which sends text/xml).
		// WSHttpBinding uses SOAP 1.2, BasicHttpBinding uses SOAP 1.1.
		//
		internal static Binding MakeSoap11BindingWithAnonymousAuth(Uri uri)
		{
			BindingElement bindingElement;

			if (uri.Scheme == "http")
			{
				bindingElement = new HttpTransportBindingElement
				{
					AuthenticationScheme = AuthenticationSchemes.Anonymous,
					MaxReceivedMessageSize = 2147483647,
					MaxBufferPoolSize = 2147483647,
					MaxBufferSize = 2147483647
				};
			}
			else if (uri.Scheme == "https")
			{
				bindingElement = new HttpsTransportBindingElement
				{
					AuthenticationScheme = AuthenticationSchemes.Anonymous,
					MaxReceivedMessageSize = 2147483647,
					MaxBufferPoolSize = 2147483647,
					MaxBufferSize = 2147483647
				};
			}
			else
			{
				throw new ArgumentException($"😮 I don't know what to do with URI scheme {uri.Scheme}");
			}

			var binding = new CustomBinding(
				new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8),
				bindingElement);

			return binding;
		}
	}
}
