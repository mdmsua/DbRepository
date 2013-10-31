using System;

namespace DbRepository
{
    public class CapacityExceededException : Exception
    {
        private readonly int _capacity;
        
        public CapacityExceededException() { }

        public CapacityExceededException(int capacity) { _capacity = capacity; }

        public override string Message
        {
            get
            {
                return string.Format("Object capacity ({0}) is exceeded");
            }
        }
    }
}
