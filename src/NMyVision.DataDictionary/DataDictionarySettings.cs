using System.Collections.Generic;

namespace NMyVision
{
    public partial class DataDictionary
    {
        public class DataDictionarySettings
        {
            public IEqualityComparer<string> Comparer { get; set; }

            public static DataDictionarySettings Default
            {
                get
                {
                    return new DataDictionarySettings()
                    {
                        Comparer = System.StringComparer.OrdinalIgnoreCase
                    };
                }
            }
        }
    }
}