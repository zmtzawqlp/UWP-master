using System;

namespace MyUWPToolkit.Util
{
    /// <summary>
    /// 参数验证帮助类
    /// </summary>
    public static class ValidationHelper
    {
        public static void ArgumentNotNull(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        public static void ArgumentNotNullOrEmpty(string value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Length == 0)
                throw new ArgumentException($"'{parameterName}' cannot be empty.", parameterName);
        }
    }
}
