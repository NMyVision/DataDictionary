using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NMyVision
{
    public partial class DataDictionary
    {
        [Serializable]
        public class DataDictionaryException : Exception
        {            
            public readonly string Name;
            public readonly object Value;
            public readonly new IDictionary<string, object> Source;

            public DataDictionaryException()
            {
            }

            public DataDictionaryException(string message) : base(message)
            {
            }

            public DataDictionaryException(string message, Exception innerException) : base(message, innerException)
            {
            }

            public DataDictionaryException(Exception ex, string name, object value, IDictionary<string, object> dict) : base($"Error {{ name= {name}, value= {value}, type= { value.GetType().Name } }}.", ex)
            {
                
                this.Data.Add("Name", name);

                if (value.GetType().IsSerializable)
                    this.Data.Add("Value", value);

                if (dict.GetType().IsSerializable)
                    this.Data.Add("Source", dict);

                this.Name = name;
                this.Value = value;
                this.Source = dict;
            }

            protected DataDictionaryException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
