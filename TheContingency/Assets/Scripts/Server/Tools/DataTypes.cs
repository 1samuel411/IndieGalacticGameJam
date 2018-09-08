using System;
using System.Collections;
using System.Collections.Generic;

namespace SNetwork
{
    /// <summary>
    /// Serializable KeyValuePairs
    /// </summary>
    [Serializable]
    public class KeyValuePairs
    {
        public string Key;
        public object Value;

        public KeyValuePairs(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        public KeyValuePairs()
        {

        }
    }
}