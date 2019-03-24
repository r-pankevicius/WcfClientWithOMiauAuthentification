using System;
using System.Collections.Generic;
using System.Linq;
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
			Console.WriteLine("SumTwoNumbers_Anonymous_Sync");
			ClientUsageScenarios.SumTwoNumbers_Anonymous_Sync();
			Console.WriteLine("OK.\n");

			Console.WriteLine("SumTwoNumbers_Anonymous_Async");
			await ClientUsageScenarios.SumTwoNumbers_Anonymous_Async();
			Console.WriteLine("OK.\n");
		}
	}
}