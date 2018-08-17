using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Dynamic.Testing
{
    public class TestParameter : Parameter
    {
        private protected const string pName = "Files";
        private static protected Type pType = typeof(string[]);

        private bool _test;
        public override bool AllowNull
        {
            get => _test;
            set => _test = value;
        }
        public override bool AllowEmptyCollection { get; set; }
        public override bool AllowEmptyString { get; set; }
        public override bool ValidateNotNull { get; set; }
        public override bool ValidateNotNullOrEmpty { get; set; }

        public TestParameter()
            : base(pName, pType)
        {
            _test = true;
            ValidatedItems = new string[5]
            {
                "Hi",
                "Hey",
                "Yo",
                "Whatup?",
                "Wazzzzzzup"
            };
            Aliases = new string[1] { "f" };
            CommitAttributes();
        }
    }
}
