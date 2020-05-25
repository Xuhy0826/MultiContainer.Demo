using System;

namespace Mark.Common
{
	public static class DateTimeExtensionMethod
	{
		/// <summary>
		/// 将日期转换成当天的0时0分0秒
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		public static DateTime ToDayBeginTime(this DateTime dateTime)
		{
			return new DateTime(dateTime.Year,dateTime.Month,dateTime.Day);
		}

		/// <summary>
		/// 去除指定时间的毫秒
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static DateTime ToSecondBegin(this DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
		}

		/// <summary>
		/// 时间间隔
		/// </summary>
		/// <param name="dateTime1"></param>
		/// <param name="dateTime2"></param>
		/// <returns></returns>
		public static string DateDiff(DateTime dateTime1, DateTime dateTime2)
		{
			string dateDiff = null;
			try
			{
				//TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
				//TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
				//TimeSpan ts = ts1.Subtract(ts2).Duration();
				TimeSpan ts = dateTime2 - dateTime1;
				if (ts.Days >= 1)
				{
					dateDiff = dateTime1.Month.ToString() + "月" + dateTime1.Day.ToString() + "日";
				}
				else
				{
					if (ts.Hours > 1)
					{
						dateDiff = ts.Hours.ToString() + "小时前";
					}
					else
					{
						dateDiff = ts.Minutes.ToString() + "分钟前";
					}
				}
			}
			catch
			{ }
			return dateDiff;
		}

        public static int ToAge(this DateTime dt)
        {
            int age = DateTime.Now.Year - dt.Year;
            if (DateTime.Now.Month < dt.Month || (DateTime.Now.Month == dt.Month && DateTime.Now.Day < dt.Day)) age--;
            return age;
        }

        public static int ToAge(this DateTime? dt)
        {
            if (dt == null)
            {
                return -1;
            }
            int age = DateTime.Now.Year - dt.Value.Year;
            if (DateTime.Now.Month < dt.Value.Month || (DateTime.Now.Month == dt.Value.Month && DateTime.Now.Day < dt.Value.Day)) age--;
            return age;
        }
    }
}
