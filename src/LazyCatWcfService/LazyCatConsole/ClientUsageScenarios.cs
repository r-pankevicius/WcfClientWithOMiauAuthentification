using Shouldly;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	public static class ClientUsageScenarios
	{
		public readonly static string EndpointUrl = "http://localhost:41193/LazyCatService.svc";

		public static void SumTwoNumbers_Anonymous_Sync()
		{
			using (var client = LazyCatAnonymousClientFactory.Create(EndpointUrl))
			{
				var result = client.SumWithAnonymousAuth(2, 5);
				result.ShouldBe(7);

				result = client.SumWithAnonymousAuth(5, 3);
				result.ShouldBe(8);
			}
		}

		public static async Task SumTwoNumbers_Anonymous_Async()
		{
			using (var client = LazyCatAnonymousClientFactory.Create(EndpointUrl))
			{
				var result = await client.SumWithAnonymousAuthAsync(2, 5);
				result.ShouldBe(7);

				result = await client.SumWithAnonymousAuthAsync(5, 3);
				result.ShouldBe(8);
			}
		}
	}
}
