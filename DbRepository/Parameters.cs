using System.Collections.Generic;

namespace DbRepository
{
    public sealed class Parameters : Dictionary<string, object>
    {
        public Parameters(int capacity) : base(capacity) { }
        
        public Parameters Set(string key, object value)
        {
            Add(key, value);
            return this;
        }

        public static Parameters Create(int capacity)
        {
            return new Parameters(capacity);
        }
    }
}
