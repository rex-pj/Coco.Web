﻿using Camino.Shared.Constants;

namespace Camino.Shared.Utils
{
    public static class DateTimeUtil
    {
        public static string ToDateHourMinusFormat(this DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormats.DateHourMinusFormat);
        }
    }
}
