using LazyCatConsole.LazyCatServiceReference;
using Shouldly;
using System.ServiceModel;
using System.Threading.Tasks;
using static LazyCatConsole.LazyCatClientFactory;

namespace LazyCatConsole
{
	public static class ClientUsageScenarios
	{
		public readonly static string EndpointUrl = "http://localhost:41193/LazyCatService.svc";

		//
		// Level 0 - standard generated client
		//

		public static void SumTwoNumbers_StandardClient_Anonymous_Sync()
		{
			using (var client = CreateAnonymousAuthClient(EndpointUrl))
			{
				var result = client.SumWithAnonymousAuth(2, 5);
				result.ShouldBe(7);

				result = client.SumWithAnonymousAuth(5, 3);
				result.ShouldBe(8);
			}
		}

		public static async Task SumTwoNumbers_StandardClient_Anonymous_Async()
		{
			using (var client = CreateAnonymousAuthClient(EndpointUrl))
			{
				var result = await client.SumWithAnonymousAuthAsync(2, 5);
				result.ShouldBe(7);

				result = await client.SumWithAnonymousAuthAsync(5, 3);
				result.ShouldBe(8);
			}
		}

		/// <summary>
		/// Tries to cheat anonymously accessing method protected with OMiau.
		/// </summary>
		/// <exception cref="FaultException&lt;ExceptionDetail&gt;" />
		public static void SumWithOMiauAuth_StandardClient_Anonymous_Fails_Sync()
		{
			using (var client = CreateAnonymousAuthClient(EndpointUrl))
			{
				client.SumWithOMiauAuth(1, 1); ;
			}
		}

		/// <summary>
		/// Tries to cheat anonymously accessing method protected with OMiau.
		/// </summary>
		/// <exception cref="FaultException&lt;ExceptionDetail&gt;" />
		public static async Task SumWithOMiauAuth_StandardClient_Anonymous_Fails_Async()
		{
			using (var client = CreateAnonymousAuthClient(EndpointUrl))
			{
				await client.SumWithOMiauAuthAsync(1, 1); ;
			}
		}

		//
		// Level 1 - manual client
		//

		public static void SumTwoNumbers_ManualClient_Anonymous_Sync()
		{
			using (var client = CreateOMiauAuthClient(EndpointUrl))
			{
				TestManualSumTwoNumbersWithAnonymous(client);
			}
		}

		public static async Task SumTwoNumbers_ManualClient_Anonymous_Async()
		{
			using (var client = CreateOMiauAuthClient(EndpointUrl))
			{
				await TestManualSumTwoNumbersWithAnonymousAsync(client);
			}
		}

		public static void SumTwoNumbers_ManualClient_OMiau_Sync()
		{
			using (var client = CreateOMiauAuthClient(EndpointUrl))
			{
				TestManualSumTwoNumbersWithOMiau(client);
			}
		}

		public static void SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Sync()
		{
			using (var client = CreateOMiauAuthClient(
				EndpointUrl, new ExpiredTokenService(EndpointUrl)))
			{
				TestManualSumTwoNumbersWithOMiau(client);
			}
		}

		public static async Task SumTwoNumbers_ManualClient_OMiau_Async()
		{
			using (var client = CreateOMiauAuthClient(EndpointUrl))
			{
				await TestManualSumTwoNumbersWithOMiauAsync(client);
			}
		}

		public static async Task SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Async()
		{
			using (var client = CreateOMiauAuthClient(
				EndpointUrl, new ExpiredTokenService(EndpointUrl)))
			{
				await TestManualSumTwoNumbersWithOMiauAsync(client);
			}
		}

		static void TestManualSumTwoNumbersWithAnonymous(LazyCatServiceOMiauManualClient client)
		{
			var result = client.SumWithAnonymousAuth(2, 5);
			result.ShouldBe(7);

			result = client.SumWithAnonymousAuth(5, 3);
			result.ShouldBe(8);
		}

		static async Task TestManualSumTwoNumbersWithAnonymousAsync(LazyCatServiceOMiauManualClient client)
		{
			var result = await client.SumWithAnonymousAuthAsync(2, 5);
			result.ShouldBe(7);

			result = await client.SumWithAnonymousAuthAsync(5, 3);
			result.ShouldBe(8);
		}

		static void TestManualSumTwoNumbersWithOMiau(LazyCatServiceOMiauManualClient client)
		{
			var result = client.SumWithOMiauAuth(2, 5);
			result.ShouldBe(7);

			result = client.SumWithOMiauAuth(5, 3);
			result.ShouldBe(8);
		}

		static async Task TestManualSumTwoNumbersWithOMiauAsync(LazyCatServiceOMiauManualClient client)
		{
			var result = await client.SumWithOMiauAuthAsync(2, 5);
			result.ShouldBe(7);

			result = await client.SumWithOMiauAuthAsync(5, 3);
			result.ShouldBe(8);
		}

		//
		// Level 2 - slim client
		//

		public static void SumTwoNumbers_Slim_Sync()
		{
			using (var client = CreateOMiauAuthSlimClient(EndpointUrl))
			{
				TestSumTwoNumbersWithAnonymous(client);
			}
		}

		public static async Task SumTwoNumbers_Slim_Async()
		{
			using (var client = CreateOMiauAuthSlimClient(EndpointUrl))
			{
				await TestSumTwoNumbersWithAnonymousAsync(client);
			}
		}

		public static void SumTwoNumbers_SlimOMiau_Sync()
		{
			using (var client = CreateOMiauAuthSlimClient(EndpointUrl))
			{
				TestSumTwoNumbersWithOMiau(client);
			}
		}

		public static void SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Sync()
		{
			using (var client = CreateOMiauAuthSlimClient(
				EndpointUrl, new ExpiredTokenService(EndpointUrl)))
			{
				TestSumTwoNumbersWithOMiau(client);
			}
		}

		public static async Task SumTwoNumbers_SlimOMiau_Async()
		{
			using (var client = CreateOMiauAuthSlimClient(EndpointUrl))
			{
				await TestSumTwoNumbersWithOMiauAsync(client);
			}
		}

		public static async Task SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Async()
		{
			using (var client = CreateOMiauAuthSlimClient(
				EndpointUrl, new ExpiredTokenService(EndpointUrl)))
			{
				await TestSumTwoNumbersWithOMiauAsync(client);
			}
		}

		////
		//// Level 3 - at stage of LOLCat Bible Project
		////
		//public static void SumTwoNumbers_Level3_Sync()
		//{
		//	using (var client = CreateLevel3Client(EndpointUrl))
		//	{
		//		TestSumTwoNumbersWithAnonymous(client);
		//	}
		//}

		static void TestSumTwoNumbersWithAnonymous(ILazyCatService client)
		{
			var result = client.SumWithAnonymousAuth(2, 5);
			result.ShouldBe(7);

			result = client.SumWithAnonymousAuth(5, 3);
			result.ShouldBe(8);
		}

		static async Task TestSumTwoNumbersWithAnonymousAsync(ILazyCatService client)
		{
			var result = await client.SumWithAnonymousAuthAsync(2, 5);
			result.ShouldBe(7);

			result = await client.SumWithAnonymousAuthAsync(5, 3);
			result.ShouldBe(8);
		}

		static void TestSumTwoNumbersWithOMiau(ILazyCatService client)
		{
			var result = client.SumWithOMiauAuth(2, 5);
			result.ShouldBe(7);

			result = client.SumWithOMiauAuth(5, 3);
			result.ShouldBe(8);
		}

		static async Task TestSumTwoNumbersWithOMiauAsync(ILazyCatService client)
		{
			var result = await client.SumWithOMiauAuthAsync(2, 5);
			result.ShouldBe(7);

			result = await client.SumWithOMiauAuthAsync(5, 3);
			result.ShouldBe(8);
		}

		#region ExpiredTokenService

		/// <summary>
		/// Token service that returns expired token at first call
		/// but refreshes token on subsequent calls.
		/// </summary>
		class ExpiredTokenService : ITokenService
		{
			readonly ITokenService m_RealTokenService;
			string m_CachedToken = "EXPIRED-TOKEN";

			public ExpiredTokenService(string endpointUrl)
			{
				m_RealTokenService = new OMiauTokenService(endpointUrl);
			}

			public async Task<string> GetTokenAsync(bool refreshNeeded = false)
			{
				if (refreshNeeded)
				{
					m_CachedToken = await m_RealTokenService.GetTokenAsync();
				}

				return m_CachedToken;
			}
		}

		#endregion
	}
}
