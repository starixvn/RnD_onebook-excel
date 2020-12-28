namespace ONEbook.SpreadSheets
{
    using System;
    using System.Diagnostics;

    public static class Converter
    {
        [DebuggerStepThrough]
        public static T To<T>(object item)
        {
            if (item == null || item == DBNull.Value)
            {
                return default(T);
            }
            return (T)Convert.ChangeType(item, typeof(T));
        }

        [DebuggerStepThrough]
        public static string ToString2(object item)
        {
            if (item == null || item == DBNull.Value)
            {
                return string.Empty;
            }
            return item.ToString();
        }
    }
}
