using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NMyVision.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMyVision
{
    public partial class DataDictionary
    {
        internal class Converter : JsonConverter
        {
            //Dictionary<string, object> _dict = new Dictionary<string, object>();

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
            }

            public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
            {
                var _dict = new Dictionary<string, object>();

                JToken token = JToken.ReadFrom(reader);

                if (token is JObject o)
                {
                    Parse(o, _dict);
                }

                return new DataDictionary(_dict, null);
            }

            void Parse(object o, IDictionary<string, object> dict)
            {
                if (o is JObject)
                {
                    ((JObject)o).Properties().Each(x =>
                    {
                        object v = x.Value;
                        if (v is JObject jo)
                        {
                            Parse(jo, dict, x.Name);
                        }
                        else if (v is JValue jv)
                        {
                            Parse(jv, dict, x.Name);
                        }
                        else if (v is JArray ja)
                        {
                            Parse(ja, dict, x.Name);
                        }
                    });
                }
                else if (o is JValue)
                {
                    Parse((JValue)o, dict, "");
                }
            }


            void Parse(JObject v, IDictionary<string, object> dict, string name)
            {
                var d = new DataDictionary();

                Parse(v, d);

                dict.Add(name, d);
            }

            void Parse(JValue v, IDictionary<string, object> dict, string name)
            {
                try
                {
                    dict.Add(name, ((JValue)v).Value);
                }
                catch (Exception ex)
                {
                    throw new DataDictionaryException(ex, name, ((JValue)v).Value, dict);
                    //new { Error = ex.Message, name, ((JValue)v).Value, dict };
                }
            }

            void Parse(JArray array, IDictionary<string, object> dict, string name)
            {
                if (array.AsJEnumerable().All(y => y is JValue))
                {
                    if (array.GroupBy(x => x.Type).Count() == 1)
                    {
                        var jv = array.First() as JValue;
                        //if (jv.HasValues)
                        //{
                        //    var t = jv.Value.GetType();
                        //    Array a = Array.CreateInstance(t, array.Count());
                        //    array.Cast<JValue>().Each((y, index) =>
                        //    {
                        //        a.SetValue(y.Value.To(t), index);
                        //    });
                        //    dict.Add(name, a);
                        //}
                        //else
                        //{
                            var t = jv.Value.GetType();
                            Array a = Array.CreateInstance(t, array.Count());
                            array.Cast<JValue>().Each((y, index) =>
                            {
                                a.SetValue(y.Value.To(t), index);
                            });
                            dict.Add(name, a);

                            //dict.Add(name, new object[0]);
                        //}
                    }
                    else
                    {
                        var a = new List<object>();

                        array.Each(y =>
                        {
                            a.Add((y as JValue).Value);
                        });

                        dict.Add(name, a);
                    }
                }
                else
                {
                    var list = new List<DataDictionary>();

                    array.Each(y =>
                    {
                        var d = new DataDictionary();

                        Parse(y, d);

                        list.Add(d);
                    });

                    dict.Add(name, list);
                }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override bool CanConvert(Type type)
            {
                return (typeof(System.Collections.IDictionary)).IsAssignableFrom(type);
            }
        }
    }
}
