using System.Collections.Generic;

namespace DbRepository
{
    public sealed class Procedures : Dictionary<string, Parameters>
    {
        private readonly int _capacity;

        public Procedures(int capacity) : base(capacity) { _capacity = capacity; }

        public Procedures Set(string key, Parameters value)
        {
            if (Count == _capacity) throw new CapacityExceededException(_capacity);
            Add(key, value);
            return this;
        }

        public static Procedures Create(int capacity)
        {
            return new Procedures(capacity);
        }
    }
}
