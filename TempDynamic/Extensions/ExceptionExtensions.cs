using System;
using System.Collections.Generic;
using System.Text;

namespace TempDynamic
{
    public static class ExceptionFactory
    {
        public static T Throw<T>(string message, params object[] arguments)
            where T : Exception
        {
            return (T)Activator.CreateInstance(typeof(T), string.Format(message, arguments));
        }

        public static ArgumentException ThrowArgument(string message, params object[] arguments)
        {
            return new ArgumentException(string.Format(message, arguments));
        }
    }
}
