using MG.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;

namespace TempDynamic
{
    public class CmdletMetadataCollection : Collection<Attribute>, IMetadataAttributeCollection
    {
        //private readonly IEqualityComparer<Attribute> _comparer;
        private readonly Dictionary<Type, CmdletMetadataAttribute> _attDict;
        private readonly ParameterAttribute _pAtt;

        public bool HasAliasAttribute => this.Contains<AliasAttribute>();
        public bool HasParameterAttribute => true;
        public ParameterAttribute Parameter => _pAtt;
        //public ParameterAttribute Parameter => _attDict.TryGetValue(typeof(ParameterAttribute), out Attribute pa)
        //    ? (ParameterAttribute)pa
        //    : null;

        public CmdletMetadataCollection()
            : this(2)
        {
        }
        public CmdletMetadataCollection(int capacity)
            : base()
        {
            _pAtt = new ParameterAttribute();
            _attDict = new Dictionary<Type, CmdletMetadataAttribute>(capacity)
            {
                { _pAtt.GetType(), _pAtt }
            };
            this.Add(_pAtt);
        }

        public bool Add(CmdletMetadataAttribute cmdAtt)
        {
            bool result = false;
            if (!_attDict.ContainsKey(cmdAtt.GetType()))
            {
                _attDict.Add(cmdAtt.GetType(), cmdAtt);
                base.InsertItem(this.Count, cmdAtt);

                result = true;
            }

            return result;
        }
        public bool Contains<T>() where T : CmdletMetadataAttribute
        {
            return this.Contains(typeof(T));
        }
        public bool Contains(Type cmdletMetadataAttributeType)
        {
            return _attDict.ContainsKey(cmdletMetadataAttributeType);
        }
        public T GetAttribute<T>() where T : CmdletMetadataAttribute
        {
            return _attDict.TryGetValue(typeof(T), out CmdletMetadataAttribute att)
                ? (T)att
                : null;
        }
        protected override void InsertItem(int index, Attribute item)
        {
            if (!(item is CmdletMetadataAttribute cmdAtt))
                throw new ArgumentException(string.Format("{0} is not of the type '{1}'.", nameof(item), nameof(CmdletMetadataAttribute)));

            if (_attDict.ContainsKey(item.GetType()))
            {
                throw new ExistingParameterException<Attribute>(item);
            }

            base.InsertItem(index, item);
            _attDict.Add(item.GetType(), cmdAtt);
        }
        protected override void RemoveItem(int index)
        {
            Attribute att = this[index];
            if (att is ParameterAttribute)
                throw ExceptionFactory.ThrowArgument("Cannot remove the {0}.", nameof(ParameterAttribute));

            if (_attDict.Remove(att?.GetType()))
            {
                base.RemoveItem(index);
            }
        }
        protected override void SetItem(int index, Attribute item)
        {
            Type t = item.GetType();
            if (item is CmdletMetadataAttribute cmdAtt && !_attDict.ContainsKey(t))
            {
                Attribute a = this[index];
                if (a is ParameterAttribute)
                    throw ExceptionFactory.ThrowArgument("Cannot remove the {0}.", nameof(ParameterAttribute));

                if (_attDict.Remove(a.GetType()))
                {
                    base.SetItem(index, cmdAtt);
                    _attDict.Add(t, cmdAtt);
                }
            }
        }

        public bool Remove<T>() where T : CmdletMetadataAttribute
        {
            return this.Remove(typeof(T));
        }
        public bool Remove(Type cmdletMetadataAttributeType)
        {
            bool result = false;
            if (_attDict.TryGetValue(cmdletMetadataAttributeType, out CmdletMetadataAttribute att))
            {
                if (ReferenceEquals(att, _pAtt))
                    throw ExceptionFactory.ThrowArgument("Cannot remove the {0} from the collection.", nameof(ParameterAttribute));

                result = this.Remove(att);
            }

            return result;
        }

        Collection<Attribute> IMetadataAttributeCollection.ToCollection()
        {
            return this;
        }
    }
}
