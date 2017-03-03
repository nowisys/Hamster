using System.ServiceProcess;

namespace Hamster
{
	public class WindowsService : ServiceBase
	{
		public WindowsService()
			: base()
		{
			this.ServiceName = "hServer";
		}

		protected override void OnStart( string[] args )
		{
			Program.Start();
		}

		protected override void OnStop()
		{
			Program.Stop();
		}
	}
}
