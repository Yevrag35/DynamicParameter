using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace Dynamic
{
    public abstract class Parameter : RuntimeDefinedParameter, IDynParam, IEquatable<Parameter>
    {
        #region Private Properties
        private IDictionary _atts;
        private string[] _aliases;
        private IList<string> _valItems = new List<string>();
        private Type _type;

        #endregion

        #region Fields

        public IList<string> ValidatedItems => _valItems;

        public string[] Aliases => _aliases;

        public abstract bool AllowNull { get; }

        #endregion

        #region Constructors
        public Parameter()
            : base()
        {
        }
        public Parameter(string name, Type type)
            : base()
        {
            base.Name = name;
            base.ParameterType = type;
        }
        public Parameter(string name, Type type, Collection<Attribute> attributes)
            : base(name, type, attributes)
        {
        }

        #endregion

        #region Methods

        public void AddAliases(IList<string> aliases)
        {
            _aliases = aliases.ToArray();
        }

        public void AddAttributes(IDictionary attributes)
        {
            _atts = attributes;
        }

        public void AddValidatedItem(IList<string> valItems)
        {
            for (int i = 0; i < valItems.Count; i++)
            {
                string s = valItems[i];
                _valItems.Add(s);
            }
        }

        public void RemoveValidatedItem(string[] items)
        {
            for (int i = _valItems.Count - 1; i >= 0; i--)
            {
                string possible = _valItems[i];
                for (int r = 0; r < items.Length; r++)
                {
                    string s = items[r];
                    if (s.Equals(possible))
                    {
                        _valItems.Remove(possible);
                    }
                }
            }
        }

        #endregion

        #region IEquatable Overrides
        public bool Equals(Parameter param)
        {
            DynParamEquality peq = new DynParamEquality();
            if (peq.Equals(this, param))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public abstract override string ToString();

        #endregion
    }
}
