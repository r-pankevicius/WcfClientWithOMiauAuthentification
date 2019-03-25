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

			Console.WriteLine("SumTwoNumbers_Anonymous_Sync");
			ClientUsageScenarios.SumTwoNumbers_Anonymous_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine("SumTwoNumbers_Anonymous_Async");
			await ClientUsageScenarios.SumTwoNumbers_Anonymous_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine("SumWithOMiauAuth_Anonymous_Fails_Sync");
			bool exceptionThrown = false;
			try
			{
				ClientUsageScenarios.SumWithOMiauAuth_Anonymous_Fails_Sync();
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

			Console.WriteLine("SumTwoNumbers_OMiau_Sync");
			ClientUsageScenarios.SumTwoNumbers_OMiau_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine("SumTwoNumbers_OMiau_Async");
			await ClientUsageScenarios.SumTwoNumbers_OMiau_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine("@@@@@@ Diving into level 2 @@@@@.\n");

			Console.WriteLine("SumTwoNumbers_Slim_Sync");
			ClientUsageScenarios.SumTwoNumbers_Slim_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine("SumTwoNumbers_Slim_Async");
			await ClientUsageScenarios.SumTwoNumbers_Slim_Async();
			Console.WriteLine("OK.\n");

			Console.WriteLine("SumTwoNumbers_SlimOMiau_Sync");
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine("SumTwoNumbers_SlimOMiau_Async");
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async();
			Console.WriteLine("OK.\n");
		}
	}
}