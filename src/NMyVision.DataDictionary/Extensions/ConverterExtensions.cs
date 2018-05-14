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

            // Cheat, since we are already referencing Json library use it to do the heavy lifting of serializing the object.
            // We lose some custom logic surrounding bool types but acceptable trade off... I think.

            return Newtonsoft.Json.JsonConvert.DeserializeObject(Newtonsoft.Json.JsonConvert.SerializeObject(item), type);            
        }        
    }
}
