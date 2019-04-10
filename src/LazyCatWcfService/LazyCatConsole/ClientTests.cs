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
		public void SumTwoNumbers_StandardClient_Anonymous_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Sync();
		}

		[Fact]
		public async void SumTwoNumbers_StandardClient_Anonymous_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Async();
		}

		[Fact]
		public void SumWithOMiauAuth_StandardClient_Anonymous_Fails_Sync()
		{
			Assert.Throws<FaultException<ExceptionDetail>>(
				() => ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Sync());
		}

		[Fact]
		public async void SumWithOMiauAuth_Manual_Anonymous_Fails_Async()
		{
			await Assert.ThrowsAsync<FaultException<ExceptionDetail>>(
				async () =>
				await ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Async()
				);
		}

		// Diving into level 1 - Manual client

		[Fact]
		public void SumTwoNumbers_ManualClient_Anonymous_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Sync();
		}

		[Fact]
		public async void SumTwoNumbers_ManualClient_Anonymous_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Async();
		}

		[Fact]
		public void SumTwoNumbers_ManualClient_OMiau_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Sync();
		}

		[Fact]
		public void SumTwoNumbers_ManualClient_OMiau_Handles_ExpiredToken_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Sync();
		}

		[Fact]
		public async void SumTwoNumbers_ManualClient_OMiau_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Async();
		}

		[Fact]
		public async void SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Async();
		}

		// Diving into level 2 - slim client

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
		public void SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Sync()
		{
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Sync();
		}

		[Fact]
		public async Task SumTwoNumbers_SlimOMiau_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async();
		}

		[Fact]
		public async Task SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Async()
		{
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Async();
		}
	}
}
