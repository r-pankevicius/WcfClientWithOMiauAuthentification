using System;
using System.ServiceModel.Web;

namespace LazyCatWcfService
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
	// NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
	public class LazyCatService : ILazyCatService
	{
		public int SumWithAnonymousAuth(int a, int b)
		{
			return a + b;
		}

		public int SumWithOMiauAuth(int a, int b)
		{
			var headers = WebOperationContext.Current.IncomingRequest.Headers;
			var authHeader = headers.Get("Authorization");
			if (string.IsNullOrEmpty(authHeader))
			{
				throw new ArgumentException("🔒 You need to pass me Authorization header somehow. Try again.");
			}

			if (!authHeader.StartsWith("Bearer "))
			{
				throw new ArgumentException(
					"🔒 Authorization header must start with 'Bearer ' followed by secret token.");
			}

			if (authHeader != "Bearer Miau")
			{
				throw new ArgumentException(
					"🔒 Unrecognized Bearer.");
			}

			return a + b;
		}

		public string GetOMiauToken_WithClientCredentials(string cat_id, string cat_secret)
		{
			if (cat_id == "KOT VASYA" && cat_secret == "UNDER SOFA")
			{
				return "Miau";
			}

			throw new ArgumentException("🔒 Bad client credentials.");
		}
	}
}
