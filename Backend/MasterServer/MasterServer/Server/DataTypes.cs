using System;

namespace SNetwork
{
    /// <summary>
    ///     Serializable KeyValuePairs
    /// </summary>
    [Serializable]
    public class KeyValuePairs
    {
        public string Key;
        public object Value;

        public KeyValuePairs(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public KeyValuePairs()
        {
        }
    }
}