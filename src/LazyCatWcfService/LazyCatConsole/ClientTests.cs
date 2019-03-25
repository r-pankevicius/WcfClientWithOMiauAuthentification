using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
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

		[Fact]
		public void SumWithOMiauAuth_Anonymous_Fails_Sync()
		{
			Assert.Throws<FaultException<ExceptionDetail>>(
				() => ClientUsageScenarios.SumWithOMiauAuth_Anonymous_Fails_Sync());
		}

		[Fact]
		public void SumTwoNumbers_OMiau_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_OMiau_Sync();
		}

		[Fact]
		public async void SumTwoNumbers_OMiau_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_OMiau_Async();
		}
	}
}
