using System;
using System.Collections.Generic;
using System.Text;

namespace HomeApplianceControl.Common
{
    public static class EnumUtils
    {
        public static TResult Convert<T, TResult>(T value)
            where T : struct, Enum
            where TResult : struct, Enum 
        {
            if (!Enum.TryParse<TResult>(value.ToString(), false, out var result))
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} was not found in type {typeof(TResult)}");

            var intValue = System.Convert.ToInt32(value);
            var intResult = System.Convert.ToInt32(result);
            if (intValue != intResult)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value}={intValue} of type {typeof(T)} is not equal to {result}={intResult} int type {typeof(TResult)}");

            return result;
        }
    }
}
