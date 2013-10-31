using System.Collections.Generic;

namespace DbRepository
{
    public sealed class Parameters : Dictionary<string, object>
    {
        private readonly int _capacity;

        public Parameters(int capacity) : base(capacity) { _capacity = capacity; }
        
        public Parameters Set(string key, object value)
        {
            if (Count == _capacity) throw new CapacityExceededException(_capacity);
            Add(key, value);
            return this;
        }

        public static Parameters Create(int capacity)
        {
            return new Parameters(capacity);
        }
    }
}
