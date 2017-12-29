using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NMyVision.Extensions
{
    internal static class ConverterExtensions
    {
        /// <summary>
        /// Returns an object of the specified type and whose value is equivalent to the specified type.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="item">An object that implements the System.IConvertible interface.</param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="FormatException"></exception>
        public static T To<T>(this object item)
        {
            return (T)To(item, typeof(T));
        }

        /// <summary>
        /// Returns an object of the specified type and whose value is equivalent to the specified type.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="item">An object that implements the System.IConvertible interface.</param>
        /// <param name="defaultValue">If conversion fails this value will be returned.</param>
        /// <returns></returns>
        public static T To<T>(this object item, T defaultValue)
        {
            if (item == null)
                return defaultValue;

            try
            {
                //if a T is Nullable To method will return null.
                return (T)(To(item, typeof(T)) ?? defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Returns an object of the specified type and whose value is equivalent to the specified type.
        /// </summary>
        /// <param name="item">An object that implements the System.IConvertible interface.</param>
        /// <param name="type">The type of object to return.</param>
        /// <returns></returns>
        /// <remarks>This is not intended to use with DataReaders use ToList instead. </remarks>
        public static object To(this object item, Type type)
        {
            if (item == null) return null;
            if (type.GetTypeInfo().IsInstanceOfType(item)) return item;

            object value;

            var temp = item.ToString();

            if (type == typeof(string))
            {
                return temp;
            }
            else if (type == typeof(bool))
            {
                if (string.IsNullOrEmpty(temp))
                {
                    //do nothing we will throw error below
                }
                else if (new[] { "yes", "1", "-1", "checked", "on", "true" }
                    .Contains(temp, StringComparer.OrdinalIgnoreCase))
                    return true;
                else if (new[] { "no", "0", "off", "false" }
                    .Contains(temp, StringComparer.OrdinalIgnoreCase))
                    return false;

                throw new InvalidCastException($"String '{ temp }' cannot be casted to a boolean.") { Source = temp };
            }
            else if (type == typeof(Guid) && !string.IsNullOrEmpty(temp))
            {
                return Guid.Parse(temp);
            }
            else if (type.IsNullableType())
            {
                if (string.IsNullOrEmpty(temp.Trim()))
                    return null;

                try
                {
                    type = Nullable.GetUnderlyingType(type);

                    return temp.To(type);
                }
                catch { return null; }

            }
            else if (type.GetTypeInfo().IsEnum)
            {
                //TODO: consider adding debug check here if temp does not exist should we default or throw error?
                return Enum.Parse(type, temp);
            }
            value = System.Convert.ChangeType(temp, type);

            return value;
        }        
    }
}
