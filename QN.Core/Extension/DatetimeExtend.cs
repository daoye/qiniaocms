using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QN
{
    public static class DatetimeExtend
    {
        /// <summary>
        /// 把日期格式化为近期时间，例如：几个月前,几天前,几小时前,几分钟前,或几秒前
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format">格式化字符串（当日期简间距超过60天时会使用）</param>
        /// <returns></returns>
        public static string ToRecentTime(this DateTime dt, string format = null)
        {
            QLang lang = QLang.Instance();

            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60)
            {
                if (string.IsNullOrWhiteSpace(format))
                {
                    return dt.ToShortDateString();
                }

                return dt.ToString(format);
            }
            else
            {
                if (span.TotalDays > 30)
                {
                    return lang.Lang("1个月前");
                }
                else
                {
                    if (span.TotalDays > 14)
                    {
                        return lang.Lang("2周前");
                    }
                    else
                    {
                        if (span.TotalDays > 7)
                        {
                            return lang.Lang("1周前");
                        }
                        else
                        {
                            if (span.TotalDays > 1)
                            {
                                return
                                string.Format(lang.Lang("{0}天前"), (int)Math.Floor(span.TotalDays));
                            }
                            else
                            {
                                if (span.TotalHours > 1)
                                {
                                    return
                                    string.Format(lang.Lang("{0}小时前"), (int)Math.Floor(span.TotalHours));
                                }
                                else
                                {
                                    if (span.TotalMinutes > 1)
                                    {
                                        return
                                        string.Format(lang.Lang("{0}分钟前"), (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return
                                            string.Format(lang.Lang("{0}秒前"), (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return lang.Lang("1秒前");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
