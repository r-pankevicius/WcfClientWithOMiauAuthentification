using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LazyCatConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync().GetAwaiter().GetResult();
			Console.WriteLine("All tests passed OK.\n");
		}

		static async Task MainAsync()
		{
			Console.WriteLine("@@@@@@ Checking an ordinary WCF client @@@@@.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Sync));
			ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Async));
			ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Async));
			ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Sync));
			bool exceptionThrown = false;
			try
			{
				ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Sync();
			}
			catch (FaultException<ExceptionDetail> ex)
			{
				exceptionThrown = true;
				Console.WriteLine("Got FaultException AS EXPECTED: {0}", ex.Message);
			}

			if (!exceptionThrown)
			{
				Console.WriteLine("FAIL - OMiau protected method can be accessed with Anonymous auth.");
				return;
			}

			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Async));
			exceptionThrown = false;
			try
			{
				await ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Async();
			}
			catch (FaultException<ExceptionDetail> ex)
			{
				exceptionThrown = true;
				Console.WriteLine("Got FaultException AS EXPECTED: {0}", ex.Message);
			}

			if (!exceptionThrown)
			{
				Console.WriteLine("FAIL - OMiau protected method can be accessed with Anonymous auth.");
				return;
			}

			Console.WriteLine("OK.\n");

			Console.WriteLine("@@@@@@ Diving into level 1 @@@@@.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Sync));
			ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Async));
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Sync));
			ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Sync));
			ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Async));
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Async));
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine("@@@@@@ Diving into level 2 @@@@@.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_Slim_Sync));
			ClientUsageScenarios.SumTwoNumbers_Slim_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_Slim_Async));
			await ClientUsageScenarios.SumTwoNumbers_Slim_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Sync));
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Sync));
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async));
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine(nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Async));
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Async();
			Console.WriteLine("OK.\n");
		}
	}
}