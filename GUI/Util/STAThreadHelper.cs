using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InputLog.Core.Util;

namespace GUI.Util
{
	public abstract class StaHelper
	{
		readonly ManualResetEvent _complete = new ManualResetEvent(false);

		public void Go()
		{
			var thread = new Thread(new ThreadStart(DoWork))
			{
				IsBackground = true,
			};
			thread.SetApartmentState( ApartmentState.STA );
			thread.Start();
		}

		// Thread entry method
		private void DoWork()
		{
			try
			{
				_complete.Reset();
				Work();
			}
			catch (Exception ex)
			{
				if (DontRetryWorkOnFailed)
					throw;
				else
				{
					try
					{
						Thread.Sleep(1000);
						Work();
					}
					catch
					{
						// ex from first exception
						MessageLogger.CatchException(this,ex,Severity.INFO);
					}
				}
			}
			finally
			{
				_complete.Set();
			}
		}

		public bool DontRetryWorkOnFailed { get; set; }

		// Implemented in base class to do actual work.
		protected abstract void Work();
	}
}
