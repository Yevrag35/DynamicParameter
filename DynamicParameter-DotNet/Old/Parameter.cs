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
    public class ParameterDefiner : IDynamicDefiner
    {
        #region Fields/Properties
        private bool _mand = false;
        private bool _an = false;
        private bool _aec = false;
        private bool _aes = false;
        private bool _vc = false;
        private bool _vnn = false;
        private bool _vnnoe = false;
        private List<string> _il;
        private List<string> _als;
        private Collection<Attribute> attCol;

        private static readonly string[] ParameterAttributeNames = new string[5]
        {
            "Mandatory", "Position", "HelpMessage", "ValueFromPipeline", "ValueFromPipelineByPropertyName"
        };
        private static readonly string[] AllowAndValidates = new string[5]
        {
            "AllowNull", "AllowEmptyCollection", "AllowEmptyString", "ValidateNotNull", "ValidateNotNullOrEmpty"
        };

        public string Name { get; }
        public Type ParameterType { get; }
        public int? Position { get; set; }
        public bool Mandatory
        {
            get => _mand;
            set => _mand = value;
        }


        List<string> IDynamicDefiner.ValidatedItems => _il;

        List<string> IDynamicDefiner.Aliases => _als;

        bool IDynamicDefiner.AllowNull
        {
            get => _an;
            set => _an = value;
        }

        bool IDynamicDefiner.AllowEmptyCollection
        {
            get => _aec;
            set => _aec = value;
        }

        bool IDynamicDefiner.AllowEmptyString
        {
            get => _aes;
            set => _aes = value;
        }

        KeyValuePair<int, int>? IDynamicDefiner.ValidateCount { get; set; }

        bool IDynamicDefiner.ValidateNotNull
        {
            get => _vnn;
            set => _vnn = value;
        }

        bool IDynamicDefiner.ValidateNotNullOrEmpty
        {
            get => _vnnoe;
            set => _vnnoe = value;
        }

        #endregion

        #region CONSTRUCTORS

        public ParameterDefiner(string name, Type pType)
        {
            this.Name = name;
            this.ParameterType = pType;
            _il = new List<string>();
            _als = new List<string>();
        }

        #endregion

        #region METHODS

        void IDynamicDefiner.Clear()
        {
            _als.Clear();
            _il.Clear();
            ((IDynamicDefiner)this).AllowEmptyCollection = false;
            ((IDynamicDefiner)this).AllowEmptyString = false;
            ((IDynamicDefiner)this).AllowNull = false;
            ((IDynamicDefiner)this).ValidateCount = null;
            ((IDynamicDefiner)this).ValidateNotNull = false;
            ((IDynamicDefiner)this).ValidateNotNullOrEmpty = false;
            attCol.Clear();
        }

        private T Cast<T>(dynamic o) => (T)o;

        private const string CAST = "Cast";

        ParameterAttribute IDynamicDefiner.SetParameterAttribute()
        {
            PropertyInfo[] theseProps = typeof(IDynamicDefiner).GetProperties().Where(
                x => ParameterAttributeNames.Contains(x.Name)).ToArray();
            
            var pAtt = new ParameterAttribute();
            PropertyInfo[] info = typeof(ParameterAttribute).GetProperties();
            for (int i = 0; i < info.Length; i++)
            {
                PropertyInfo pi = info[i];
                for (int t = 0; t < theseProps.Length; t++)
                {
                    PropertyInfo key = theseProps[t];
                    if (pi.Name.Equals(key.Name))
                    {
                        MethodInfo castMethod = this.GetType().GetMethod(
                            CAST, BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(pi.PropertyType);
                        object keyValue = key.GetValue(this);
                        if (keyValue != null)
                        {
                            object castedObject = castMethod.Invoke(this, new object[] { keyValue });
                            pi.SetValue(pAtt, castedObject, null);
                        }
                    }
                }
            }
            return pAtt;
        }

        private void AddTheRest()
        {
            PropertyInfo[] theseProps = typeof(IDynamicDefiner).GetProperties().Where(
                x => AllowAndValidates.Contains(x.Name)).ToArray();
            for (int i = 0; i < theseProps.Length; i++)
            {
                var prop = theseProps[i];
                if ((bool)prop.GetValue(this))
                {
                    var objHandle = Activator.CreateInstance(
                        "System.Management.Automation", "System.Management.Automation." + prop.Name + "Attribute");
                    var newAtt = (Attribute)objHandle.Unwrap();
                    attCol.Add(newAtt);
                }
            }
        }

        public static explicit operator RuntimeDefinedParameter(ParameterDefiner pd) =>
            ((IDynamicDefiner)pd).NewParameter();

        public static explicit operator RuntimeDefinedParameterDictionary(ParameterDefiner pd) =>
            ((IDynamicDefiner)pd).NewDictionary();

        RuntimeDefinedParameter IDynamicDefiner.NewParameter()
        {
            attCol = new Collection<Attribute>
            {
                ((IDynamicDefiner)this).SetParameterAttribute()
            };
            if (((IDynamicDefiner)this).ValidateCount.HasValue)
            {
                attCol.Add(new ValidateCountAttribute(
                    ((IDynamicDefiner)this).ValidateCount.Value.Key,
                    ((IDynamicDefiner)this).ValidateCount.Value.Value
                ));
            }
            this.AddTheRest();
            if (_il.Count > 0)
            {
                attCol.Add(new ValidateSetAttribute(_il.ToArray()));
            }
            if (_als.Count > 0)
            {
                attCol.Add(new AliasAttribute(_als.ToArray()));
            }
            return new RuntimeDefinedParameter(this.Name, this.ParameterType, attCol);
        }

        RuntimeDefinedParameterDictionary IDynamicDefiner.NewDictionary()
        {
            return new RuntimeDefinedParameterDictionary
            {
                { this.Name, ((IDynamicDefiner)this).NewParameter() }
            };
        }

        RuntimeDefinedParameterDictionary IDynamicDefiner.NewDictionary(RuntimeDefinedParameter[] parameters)
        {
            var dict = new RuntimeDefinedParameterDictionary();
            for (int i = 0; i < parameters.Length; i++)
            {
                RuntimeDefinedParameter p = parameters[i];
                dict.Add(p.Name, p);
            }
            return dict;
        }

        #endregion
    }
}
