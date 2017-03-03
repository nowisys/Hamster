using System;
using System.Collections.Generic;
using System.Text;
using Hamster.Plugin;

namespace Hamster
{
	public class LoggerProxy
	{
		private ILogger logger = NullLogger.Instance;

		public LoggerProxy()
		{

		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value ?? NullLogger.Instance; }
		}
	}
}
