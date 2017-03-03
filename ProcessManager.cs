using System.Collections.Generic;
using Hamster.Plugin.Signal;

namespace Hamster
{
	public class ProcessManager
	{
		private List<SignalProcessor> processors;

		public ProcessManager( SignalProcessor[] processors )
		{
			this.processors = new List<SignalProcessor>( processors );
		}

		public void Start()
		{
			foreach( SignalProcessor p in processors )
			{
				p.Start();
			}
		}

		public void Stop()
		{
			foreach( SignalProcessor p in processors )
			{
				p.Stop();
			}
		}
	}
}
