using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DbRepository
{
    public sealed class Parameters : IReadOnlyDictionary<string, object>
    {
        private readonly int _capacity;

        private readonly ConcurrentDictionary<string, object> _dictionary;

        public Parameters(int capacity)
        { 
            _capacity = capacity;
            _dictionary = new ConcurrentDictionary<string, object>();
        }
        
        public Parameters Set(string key, object value)
        {
            if (_dictionary.Count == _capacity) throw new CapacityExceededException(_capacity);
            if (!_dictionary.TryAdd(key, value)) throw new ArgumentException("An element with the the same key already exists in the parameter list", key);
            return this;
        }

        public static Parameters Create(int capacity)
        {
            return new Parameters(capacity);
        }

        public static Parameters From<T>(T entity)
        {
            var properties = entity.GetType().GetProperties().AsParallel().Where(p => p.CanRead);
            var parameters = Create(properties.Count());
            Parallel.ForEach(properties, property => TrySetParameter(parameters, property, entity));
            return parameters;
        }

        private static bool TrySetParameter<T>(Parameters parameters, PropertyInfo property, T value)
        {
            try
            {
                parameters.Set(property.Name, property.GetValue(value));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public IEnumerable<string> Keys
        {
            get { return _dictionary.Keys; }
        }

        public bool TryGetValue(string key, out object value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public IEnumerable<object> Values
        {
            get { return _dictionary.Values; }
        }

        public object this[string key]
        {
            get { return _dictionary[key]; }
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
