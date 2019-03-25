using System.ServiceModel;
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
		// Checking an ordinary WCF client

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

		// Diving into level 1

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

		// Diving into level 2

		[Fact]
		public void SumTwoNumbers_Slim_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_Slim_Sync();
		}

		[Fact]
		public async Task SumTwoNumbers_Slim_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_Slim_Async();
		}

		[Fact]
		public void SumTwoNumbers_SlimOMiau_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Sync();
		}

		[Fact]
		public async Task SumTwoNumbers_SlimOMiau_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async();
		}
	}
}
