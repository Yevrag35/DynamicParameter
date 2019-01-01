using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace MG.Dynamic
{
    public abstract class Parameter : RuntimeDefinedParameter, IDynamic
    {
        #region Private Properties
        private IList<string> _aliases;
        private IList<string> _valItems = new List<string>();

        #endregion

        #region Fields

        public string[] ValidatedItems
        {
            get => _valItems == null ? null : _valItems.ToArray();
            set => _valItems = value;
        }

        public string[] Aliases
        {
            get => _aliases == null ? null : _aliases.ToArray();
            set => _aliases = value;
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
                var valSet = new ValidateSetAttribute(ValidatedItems.ToArray());
                Attributes.Add(valSet);
            }

            if (Aliases != null && Attributes.Where(x => x.GetType() == typeof(AliasAttribute)).ToArray().Length == 0)
            {
                var aliasAtt = new AliasAttribute(Aliases.ToArray());
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
                        var att = (CmdletMetadataAttribute)Activator.CreateInstance(t, new object[] { });
                        Attributes.Add(att);
                    }
                }
            }
        }

        public void SetValidateCount(int minLength, int maxLength)
        {
            var valCount = new ValidateCountAttribute(minLength, maxLength);
            Attributes.Add(valCount);
        }

        internal protected T Cast<T>(dynamic o) => (T)o;

        public void SetParameterAttributes(IDictionary attributes)
        {
            string[] keys = attributes.Keys.Cast<string>().ToArray();
            var pAtt = new ParameterAttribute();
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
    }
}
