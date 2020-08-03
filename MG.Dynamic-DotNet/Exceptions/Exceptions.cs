using System;
using MG.Dynamic.Parameter;

namespace MG.Dynamic
{
    public class LibraryContainsNoIDynsException : InvalidOperationException
    {
        private const string DEF_MSG = "This library contains no dynamic parameters to retrieve values from.";

        public LibraryContainsNoIDynsException()
            : base(DEF_MSG) { }
    }

    public class InvalidKeyException : InvalidOperationException
    {
        private const string DEF_MSG = "Converting into a RuntimeDefinedParameterDictionary failed because a parameter's Key was not specified and we've been told not to use the Name.";

        public IRuntimeParameter Parameter { get; }

        public InvalidKeyException(IRuntimeParameter parameter)
            : base(DEF_MSG)
        {
            this.Parameter = parameter;
        }
    }
}