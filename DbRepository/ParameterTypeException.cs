using System;

namespace DbRepository
{
    public class ParameterTypeException : Exception
    {
        private readonly string _expected;

        private readonly string _obtained;

        private readonly string _parameter;

        private readonly string _command;

        public ParameterTypeException()
        {

        }

        public ParameterTypeException(string command, string parameter, string expected, string obtained)
        {
            _expected = expected;
            _obtained = obtained;
            _parameter = parameter;
            _command = command;
        }
        public override string Message
        {
            get
            {
                return string.Format("Type of the parameter '{0}' assigned to command '{1}' is '{2}'. Expecting: '{3}'", _parameter, _command, _obtained, _expected);
            }
        }
    }
}
