using LazyCatConsole.LazyCatServiceReference;

namespace LazyCatConsole
{
	// Microsoft .NET Remoting: A Technical Overview
	// https://docs.microsoft.com/en-us/previous-versions/dotnet/articles/ms973857(v=msdn.10)
	public class Level3Client : LazyCatServiceClient
	{
		protected override ILazyCatService CreateChannel()
		{
			var baseChannel = base.CreateChannel();
			return Level3Factory<ILazyCatService>.WrapMyChannel(baseChannel);
		}
	}

	public class Level3Factory<TChannel>
	{
		public static TChannel WrapMyChannel(TChannel realChannel)
		{
			return realChannel;
		}
	}
}
