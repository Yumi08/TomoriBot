using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomoriBot
{
	internal static class Global
	{
		public static Random R = new Random();

		/// <summary>
		/// Gets time since 1/1/2000.
		/// </summary>
		/// <returns></returns>
		public static TimeSpan GetElapsedTime()
		{
			var centuryBegin = new DateTime(2000, 1, 1);
			var currentDate = DateTime.Now;
			long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
			return new TimeSpan(elapsedTicks);
		}
	}
}