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
                return string.Format("Type {0} of the parameter {1} assigned to command {2} is not expected. Required type: {3}", _obtained, _parameter, _command, _expected);
            }
        }
    }
}
