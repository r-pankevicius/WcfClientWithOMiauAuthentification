using LazyCatConsole.LazyCatServiceReference;
using Shouldly;
using System.ServiceModel;
using System.Threading.Tasks;
using static LazyCatConsole.LazyCatClientFactory;

namespace LazyCatConsole
{
	public static class ClientUsageScenarios
	{
		#region Not interesting

		//
		// Standard generated client
		//

		public static void SumTwoNumbers_StandardClient_Anonymous_Sync()
		{
			using (var client = CreateAnonymousAuthClient())
			{
				var result = client.SumWithAnonymousAuth(2, 5);
				result.ShouldBe(7);

				result = client.SumWithAnonymousAuth(5, 3);
				result.ShouldBe(8);
			}
		}

		public static async Task SumTwoNumbers_StandardClient_Anonymous_Async()
		{
			using (var client = CreateAnonymousAuthClient())
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
			using (var client = CreateAnonymousAuthClient())
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
			using (var client = CreateAnonymousAuthClient())
			{
				await client.SumWithOMiauAuthAsync(1, 1); ;
			}
		}

		#endregion

		static ChannelFactory<ILazyCatService> Factory;

		static ClientUsageScenarios()
		{
		}
	}
}
