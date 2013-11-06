using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public static Parameters From<T>(T entity)
        {
            var properties = entity.GetType().GetProperties().AsParallel().Where(p => p.CanRead).ToList();
            var parameters = Create(properties.Count);
            properties.ForEach(p => TrySetParameter(parameters, p, entity));
            return parameters;
        }

        private static bool TrySetParameter<T>(Parameters parameters, PropertyInfo property, T value)
        {
            try
            {
                parameters.Add(property.Name, property.GetValue(value));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
