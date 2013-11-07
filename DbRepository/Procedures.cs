using System.Collections.Generic;

namespace DbRepository
{
    public sealed class Procedures : IReadOnlyDictionary<string, Parameters>
    {
        private readonly int _capacity;

        private readonly IDictionary<string, Parameters> _dictionary;

        public Procedures(int capacity) 
        { 
            _capacity = capacity;
            _dictionary = new Dictionary<string, Parameters>(capacity);
        }

        public Procedures Set(string key, Parameters value)
        {
            if (_dictionary.Count == _capacity) throw new CapacityExceededException(_capacity);
            _dictionary.Add(key, value);
            return this;
        }

        public static Procedures Create(int capacity)
        {
            return new Procedures(capacity);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public IEnumerable<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public bool TryGetValue(string key, out Parameters value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public IEnumerable<Parameters> Values
        {
            get { return _dictionary.Values; }
        }

        public Parameters this[string key]
        {
            get { return _dictionary[key]; }
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public IEnumerator<KeyValuePair<string, Parameters>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
