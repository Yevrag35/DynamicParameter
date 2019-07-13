using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MG.Dynamic
{
    public class LibraryContainsNoIDynsException : InvalidOperationException
    {
        private const string DEF_MSG = "This library contains no dynamic parameters to retrieve values from.";

        public LibraryContainsNoIDynsException()
            : base(DEF_MSG) { }
    }
}