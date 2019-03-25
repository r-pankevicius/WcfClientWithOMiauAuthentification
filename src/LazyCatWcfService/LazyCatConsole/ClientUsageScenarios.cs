using Shouldly;
using System.ServiceModel;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	public static class ClientUsageScenarios
	{
		public readonly static string EndpointUrl = "http://localhost:41193/LazyCatService.svc";

		public static void SumTwoNumbers_Anonymous_Sync()
		{
			using (var client = LazyCatClientFactory.CreateAnonymousAuthClient(EndpointUrl))
			{
				var result = client.SumWithAnonymousAuth(2, 5);
				result.ShouldBe(7);

				result = client.SumWithAnonymousAuth(5, 3);
				result.ShouldBe(8);
			}
		}

		public static async Task SumTwoNumbers_Anonymous_Async()
		{
			using (var client = LazyCatClientFactory.CreateAnonymousAuthClient(EndpointUrl))
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
		public static void SumWithOMiauAuth_Anonymous_Fails_Sync()
		{
			using (var client = LazyCatClientFactory.CreateAnonymousAuthClient(EndpointUrl))
			{
				var result = client.SumWithOMiauAuth(2, 5);
			}
		}

		public static void SumTwoNumbers_OMiau_Sync()
		{
			using (var client = LazyCatClientFactory.CreateOMiauAuthClient(EndpointUrl))
			{
				var result = client.SumWithOMiauAuth(2, 5);
				result.ShouldBe(7);

				result = client.SumWithOMiauAuth(5, 3);
				result.ShouldBe(8);
			}
		}

		public static async Task SumTwoNumbers_OMiau_Async()
		{
			using (var client = LazyCatClientFactory.CreateOMiauAuthClient(EndpointUrl))
			{
				var result = await client.SumWithOMiauAuthAsync(2, 5);
				result.ShouldBe(7);

				result = await client.SumWithOMiauAuthAsync(5, 3);
				result.ShouldBe(8);
			}
		}

		public static void SumTwoNumbers_Slim_Sync()
		{
			using (var client = LazyCatClientFactory.CreateOMiauAuthSlimClient(EndpointUrl))
			{
				var result = client.SumWithAnonymousAuth(2, 5);
				result.ShouldBe(7);

				result = client.SumWithAnonymousAuth(5, 3);
				result.ShouldBe(8);
			}
		}

		public static async Task SumTwoNumbers_Slim_Async()
		{
			using (var client = LazyCatClientFactory.CreateOMiauAuthSlimClient(EndpointUrl))
			{
				var result = await client.SumWithAnonymousAuthAsync(2, 5);
				result.ShouldBe(7);

				result = await client.SumWithAnonymousAuthAsync(5, 3);
				result.ShouldBe(8);
			}
		}

		public static void SumTwoNumbers_SlimOMiau_Sync()
		{
			using (var client = LazyCatClientFactory.CreateOMiauAuthSlimClient(EndpointUrl))
			{
				var result = client.SumWithOMiauAuth(2, 5);
				result.ShouldBe(7);

				result = client.SumWithOMiauAuth(5, 3);
				result.ShouldBe(8);
			}
		}

		public static async Task SumTwoNumbers_SlimOMiau_Async()
		{
			using (var client = LazyCatClientFactory.CreateOMiauAuthSlimClient(EndpointUrl))
			{
				var result = await client.SumWithOMiauAuthAsync(2, 5);
				result.ShouldBe(7);

				result = await client.SumWithOMiauAuthAsync(5, 3);
				result.ShouldBe(8);
			}
		}
	}
}
