using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace Dynamic
{
    public abstract class Parameter : RuntimeDefinedParameter, IDynamic, IEquatable<Parameter>
    {
        #region Private Properties
        private IList<string> _aliases;
        private IList<string> _valItems = new List<string>();

        #endregion

        #region Fields

        public string[] ValidatedItems
        {
            get
            {
                if (_valItems == null)
                {
                    return null;
                }
                else
                {
                    return _valItems.ToArray();
                }
            }
            set => _valItems = value;

        }

        public string[] Aliases
        {
            get
            {
                if (_aliases == null)
                {
                    return null;
                }
                else
                {
                    return _aliases.ToArray();
                }
            }
            set
            {
                _aliases = value;
            }
        }

        public abstract bool AllowNull { get; set; }
        public abstract bool AllowEmptyCollection { get; set; }
        public abstract bool AllowEmptyString { get; set; }
        public abstract bool ValidateNotNull { get; set; }
        public abstract bool ValidateNotNullOrEmpty { get; set; }

        #endregion

        #region Constructors
        public Parameter()
            : base()
        {
        }
        public Parameter(string name)
            : base()
        {
            Name = name;
            ParameterType = typeof(string);
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

        public void Clear()
        {
            Attributes.Clear();
            Aliases = null;
            ValidatedItems = null;
            Value = null;
        }

        public void CommitAttributes()
        {
            if (ValidatedItems == null)
            {
                throw new NullReferenceException("ValidatedItems cannot be null");
            }
            else if (Attributes.Where(x => x.GetType() == typeof(ValidateSetAttribute)).ToArray().Length == 0)
            {
                ValidateSetAttribute valSet = new ValidateSetAttribute(ValidatedItems.ToArray());
                Attributes.Add(valSet);
            }

            if (Aliases != null && Attributes.Where(x => x.GetType() == typeof(AliasAttribute)).ToArray().Length == 0)
            {
                AliasAttribute aliasAtt = new AliasAttribute(Aliases.ToArray());
                Attributes.Add(aliasAtt);
            }
            PropertyInfo[] propInfo = GetType().GetProperties().Where(x => 
                x.PropertyType == typeof(bool) && x.Name != "IsSet").ToArray();

            for (int i = 0; i < propInfo.Length; i++)
            {
                PropertyInfo p = propInfo[i];
                if (p.GetValue(this).Equals(true))
                {
                    Type t = typeof(ParameterAttribute).Assembly.DefinedTypes.Single(x => x.Name == p.Name + "Attribute");
                    if (Attributes.Where(x => x.GetType() == t).ToArray().Length == 0)
                    {
                        CmdletMetadataAttribute att = (CmdletMetadataAttribute)Activator.CreateInstance(t, new object[] { });
                        Attributes.Add(att);
                    }
                }
            }
        }

        public void SetValidateCount(int minLength, int maxLength)
        {
            ValidateCountAttribute valCount = new ValidateCountAttribute(minLength, maxLength);
            Attributes.Add(valCount);
        }

        public T Cast<T>(object o)
        {
            return (T)o;
        }

        public void SetParameterAttributes(IDictionary attributes)
        {
            string[] keys = attributes.Keys.Cast<string>().ToArray();
            ParameterAttribute pAtt = new ParameterAttribute();
            PropertyInfo[] info = typeof(ParameterAttribute).GetProperties();
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo pi = info[i];
                for (int t = 0; t < attributes.Keys.Count; t++)
                {
                    string key = keys[t];
                    if (pi.Name.Equals(key))
                    {
                        MethodInfo castMethod = GetType().GetMethod("Cast").MakeGenericMethod(pi.PropertyType);
                        object castedObject = castMethod.Invoke(this, new object[] { attributes[key] });
                        pi.SetValue(pAtt, castedObject, null);
                    }
                }
            }
            Attributes.Add(pAtt);
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
        public override string ToString() => GetType().FullName;

        public bool Equals(IDynamic other)
        {
            return base.Equals(other);
        }

        #endregion
    }
}
