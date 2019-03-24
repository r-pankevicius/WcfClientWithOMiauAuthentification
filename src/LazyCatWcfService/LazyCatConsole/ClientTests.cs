using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LazyCatConsole
{
	/// <summary>
	/// These are INTEGRATION tests for clients. Not true unit tests, they require
	/// actually running service at <see cref="ClientUsageScenarios.EndpointUrl"/>
	/// </summary>
	public class ClientTests
	{
		[Fact]
		public void SumTwoNumbers_Anonymous_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_Anonymous_Sync();
		}

		[Fact]
		public async void SumTwoNumbers_Anonymous_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_Anonymous_Async();
		}
	}
}
