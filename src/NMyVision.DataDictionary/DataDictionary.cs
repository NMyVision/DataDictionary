using Newtonsoft.Json;
using NMyVision.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace NMyVision
{
    [Newtonsoft.Json.JsonConverter(typeof(DataDictionary.Converter))]
    public partial class DataDictionary : Dictionary<string, object>
    {
        public static Func<DataDictionarySettings> DefaultSettings = () => DataDictionarySettings.Default;


        public DataDictionary() : this(DefaultSettings().Comparer)
        {

        }

        public DataDictionary(IEqualityComparer<string> comparer) : base(comparer)
        {

        }

        public DataDictionary(IDictionary<string, object> source) : this(source, DefaultSettings().Comparer)
        {

        }

        public DataDictionary(IDictionary<string, object> source, IEqualityComparer<string> comparer) : base(source, comparer)
        {

        }

        public DataDictionary(object source) : this(source, DefaultSettings().Comparer)
        {

        }

        public DataDictionary(object source, IEqualityComparer<string> comparer) : base(DataDictionary.From(source), StringComparer.OrdinalIgnoreCase)
        {

        }

        #region Static Methods

        public static DataDictionary Load(string filename)
        {

            DataDictionary data = new DataDictionary();

            var json = System.IO.File.ReadAllText(filename);

            return ParseJson(json);

        }

        public static DataDictionary ParseJson(string content)
        {
            // if json is an array wrap it in a root key... might change this later to deserialize an DataDictionary[]
            if (content.StartsWith("["))
                content = $"{{ \"root\" : {content} }}";
            return JsonConvert.DeserializeObject<DataDictionary>(content, new Converter());
        }

        /// <summary>
        /// Create a DataDictionary from anonymous object, classes or dictionaries.
        /// </summary>
        /// <param name="source">item to be converted</param>
        /// <returns>DataDictionary</returns>
        public static DataDictionary From(object source)
        {
            return DataDictionary.ParseJson(JsonConvert.SerializeObject(source));
        }

        #endregion

        /// <summary>
        ///  Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<string, object> item) => this.Add(item.Key, item.Value);

        /// <summary>
        /// Convert DataDictionary to an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T To<T>()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Convert DataDictionary to an ExpandoObject of type .
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ExpandoObject ToExpandoObject() => this.ToExpando();

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type object will be returned as.</typeparam>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            try
            {
                return (T)this[key].To<T>();
            }
            catch { }

            return default(T);
        }

        public DataDictionary GetItem(string key)
        {
            var dd = new DataDictionary();

            var o = this.Get<object>(key);
            if (o is DataDictionary od)
                return od;

            if (o is String)
                throw new InvalidCastException($"Object with key: [{key}] is a string, use Get<>.");

            if (o is System.Collections.IEnumerable oe)
            {
                throw new NotSupportedException($"Object with key: [{key}] is enumerable, use GetItems.");
            }

            return dd;
        }

        public IEnumerable<DataDictionary> GetItems(string key)
        {

            var o = this.Get<object>(key);
            if (o is IEnumerable oe)
            {
                return oe
                .Cast<object>()
                .Select(x =>
                   DataDictionary.ParseJson(
                       JsonConvert.SerializeObject(x)))
                .AsEnumerable();
            }

            throw new NotSupportedException($"Object with key: [{key}] is not enumerable, use GetItem");
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type object will be returned as.</typeparam>
        /// <param name="key">The key of the value to get or set.</param>
        /// <param name="defaultValue">If key is not present or value is not able to be converted to T this value is used.</param>
        public T Get<T>(string key, T defaultValue)
        {
            if (!ContainsKey(key))
                return defaultValue;

            return Get<T>(key);
        }


        public void AddOrUpdate(string key, object value)
        {
            if (ContainsKey(key))
                this[key] = value;
            else
                Add(key, value);
        }


        internal void AddRange(Dictionary<string, object> properties)
        {
            InternalAddRange(properties);
        }

        public void AddRange(object source)
        {
            if (source == null) return;

            if (source is IDictionary d)
            {
                InternalAddRange((Dictionary<string, object>)source);
            }
            else
            {
                AddRange(DataDictionary.From(source));
            }
        }

        private void InternalAddRange(Dictionary<string, object> properties)
        {
            //dangerous cause we could override information from one result over another
            properties.ToList().ForEach(x => this.AddOrUpdate(x.Key, x.Value));
        }

        public DataDictionary Flatten()
        {
            return DataDictionary.Flatten(this);
        }

        public static DataDictionary Flatten(DataDictionary dd, DataDictionary result = null, string parent = null)
        {

            if (result == null) result = new DataDictionary();

            foreach (var kv in dd)
            {
                if (kv.Value is DataDictionary kvd)
                {
                    var prefix = string.IsNullOrEmpty(parent) ? "" : parent;
                    Flatten(kvd, result, $"{ prefix }{ kv.Key }.");
                }
                else if (kv.Value is IEnumerable<DataDictionary> kve)
                {
                    for (int i = 0; i < kve.Count(); i++)
                    {
                        var prefix = string.IsNullOrEmpty(parent) ? "" : parent;
                        Flatten(kve.ElementAt(i), result, $"{ prefix }{ kv.Key }[{i}].");
                    }
                }
                else if (parent != null)
                    result.Add($"{parent}{kv.Key}", kv.Value);
                else
                    result.Add(kv);
            }

            return result;
        }
        
    }
}
