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
    public class CmdletMetadataCollection : Collection<Attribute>
    {
        private IEqualityComparer<Attribute> _comparer;
        private Dictionary<Type, Attribute> _attDict;

        public bool HasAliasAttribute => this.Contains<AliasAttribute>();
        public bool HasParameterAttribute => this.Contains<ParameterAttribute>();
        public ParameterAttribute Parameter => _attDict.TryGetValue(typeof(ParameterAttribute), out Attribute pa)
            ? (ParameterAttribute)pa
            : null;

        public CmdletMetadataCollection()
            : base()
        {
            _attDict = new Dictionary<Type, Attribute>(2);
            this.Add(new ParameterAttribute());
        }

        public bool Contains<T>() where T : Attribute
        {
            return _attDict.ContainsKey(typeof(T));
        }
        public T GetAttribute<T>() where T : Attribute
        {
            return _attDict.TryGetValue(typeof(T), out Attribute att)
                ? (T)att
                : null;
        }
        protected override void InsertItem(int index, Attribute item)
        {
            if (_attDict.ContainsKey(item.GetType()))
            {
                throw new ExistingParameterException<Attribute>(item);
            }

            base.InsertItem(index, item);
            _attDict.Add(item.GetType(), item);
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
            if (!_attDict.ContainsKey(t))
            {
                Attribute a = this[index];
                if (a is ParameterAttribute)
                    throw ExceptionFactory.ThrowArgument("Cannot remove the {0}.", nameof(ParameterAttribute));

                if (_attDict.Remove(a.GetType()))
                {
                    base.SetItem(index, item);
                    _attDict.Add(t, item);
                }
            }
        }

        public void Remove<T>() where T : CmdletMetadataAttribute
        {
            Type t = typeof(T);
            if (_attDict.TryGetValue(t, out Attribute att))
            {
                this.Remove(att);
            }
        }
    }
}
