using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

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

		public static string GetNickname(SocketGuildUser user)
		{
			return user.Nickname ?? user.Username;
		}

		public static class WeightedRandomization
		{
			public static T Choose<T>(List<T> list) where T : IWeighted
			{
				if (list.Count == 0)
				{
					return default(T);
				}

				int totalweight = list.Sum(c => c.Weight);
				Random rand = new Random();
				int choice = rand.Next(totalweight);
				int sum = 0;

				foreach (var obj in list)
				{
					for (int i = sum; i < obj.Weight + sum; i++)
					{
						if (i >= choice)
						{
							return obj;
						}
					}
					sum += obj.Weight;
				}

				return list.First();
			}
		}

		public interface IWeighted
		{
			int Weight { get; set; }
		}
	}
}