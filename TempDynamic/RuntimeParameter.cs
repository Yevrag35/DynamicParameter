using MG.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace TempDynamic
{
    public class RuntimeParameter
    {
        private RuntimeDefinedParameter _rpRef;

        public List<string> Aliases { get; } = new List<string>();
        public CmdletMetadataCollection Attributes { get; }
        public bool AllowEmptyCollection
        {
            get => this.Attributes.Contains<AllowEmptyCollectionAttribute>();
            set => this.SetAttribute<AllowEmptyCollectionAttribute>(value);
        }
        public bool AllowEmptyString
        {
            get => this.Attributes.Contains<AllowEmptyStringAttribute>();
            set => this.SetAttribute<AllowEmptyStringAttribute>(value);
        }
        public bool AllowNull
        {
            get => this.Attributes.Contains<AllowNullAttribute>();
            set => this.SetAttribute<AllowNullAttribute>(value);
        }
        public bool DontShow
        {
            get => this.Attributes.Parameter.DontShow;
            set => this.Attributes.Parameter.DontShow = value;
        }
        public string HelpMessage
        {
            get => this.Attributes.Parameter.HelpMessage;
            set => this.Attributes.Parameter.HelpMessage = value;
        }
        public string HelpMessageBaseName
        {
            get => this.Attributes.Parameter.HelpMessageBaseName;
            set => this.Attributes.Parameter.HelpMessageBaseName = value;
        }
        public string HelpMessageResourceId
        {
            get => this.Attributes.Parameter.HelpMessageResourceId;
            set => this.Attributes.Parameter.HelpMessageResourceId = value;
        }
        public bool Mandatory
        {
            get => this.Attributes.Parameter.Mandatory;
            set => this.Attributes.Parameter.Mandatory = value;
        }
        public string Name { get; set; }
        public string ParameterSetName
        {
            get => this.Attributes.Parameter.ParameterSetName;
            set => this.Attributes.Parameter.ParameterSetName = value;
        }
        public Type ParameterType { get; set; }
        public int Position
        {
            get => this.Attributes.Parameter.Position;
            set => this.Attributes.Parameter.Position = value;
        }
        public bool SupportsWildcards
        {
            get => this.Attributes.Contains<SupportsWildcardsAttribute>();
            set => this.SetAttribute<SupportsWildcardsAttribute>(value);
        }
        public Range? ValidateCount
        {
            get => Range.TryMakeRange(this.Attributes.GetAttribute<ValidateCountAttribute>());
            set => this.ReplaceAttribute(value, r => new ValidateCountAttribute(r.Minimum, r.Maximum));
        }
        public Range? ValidateLength
        {
            get => Range.TryMakeRange(this.Attributes.GetAttribute<ValidateLengthAttribute>());
            set => this.ReplaceAttribute(value, r => new ValidateLengthAttribute(r.Minimum, r.Maximum));
        }
        public ISet<string> ValidatedItems { get; set; } = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase);
        public bool ValidateNotNull
        {
            get => this.Attributes.Contains<ValidateNotNullAttribute>();
            set => this.SetAttribute<ValidateNotNullAttribute>(value);
        }
        public bool ValidateNotNullOrEmpty
        {
            get => this.Attributes.Contains<ValidateNotNullOrEmptyAttribute>();
            set => this.SetAttribute<ValidateNotNullOrEmptyAttribute>(value);
        }
        public bool ValueFromPipeline
        {
            get => this.Attributes.Parameter.ValueFromPipeline;
            set => this.Attributes.Parameter.ValueFromPipeline = true;
        }
        public bool ValueFromPipelineByPropertyName
        {
            get => this.Attributes.Parameter.ValueFromPipelineByPropertyName;
            set => this.Attributes.Parameter.ValueFromPipelineByPropertyName = value;
        }
        public bool ValueFromRemainingArguments
        {
            get => this.Attributes.Parameter.ValueFromRemainingArguments;
            set => this.Attributes.Parameter.ValueFromRemainingArguments = value;
        }

        public RuntimeParameter(string name)
            : this(name, typeof(string[]))
        {
        }
        public RuntimeParameter(string name, Type parameterType)
        {
            this.Name = name;
            this.ParameterType = parameterType;
            this.Attributes = new CmdletMetadataCollection();
        }
        public RuntimeParameter(string name, Type parameterType, IEnumerable<CmdletMetadataAttribute> attributes)
            : this(name, parameterType)
        {
            foreach (CmdletMetadataAttribute attribute in attributes
                .Where(x => !typeof(ParameterAttribute).Equals(x?.GetType())))
            {
                this.Attributes.Add(attribute);
            }
        }

        public static implicit operator RuntimeDefinedParameter(RuntimeParameter rp)
        {
            if (!rp.Attributes.Contains<AliasAttribute>() && TryCopyAliases(rp.Aliases, out AliasAttribute aliases))
                rp.Attributes.Add(aliases);

            if (TryCopyValidatedItems(rp.ValidatedItems, out ValidateSetAttribute attribute) && !rp.Attributes.Contains<ValidateSetAttribute>())
                rp.Attributes.Add(attribute);

            rp._rpRef = new RuntimeDefinedParameter(rp.Name, rp.ParameterType, rp.Attributes);
            return rp._rpRef;
        }

        /// <summary>
        /// Retrieves the value of the parameter.
        /// </summary>
        /// <typeparam name="T">
        ///     The type to return the selected value as.  This should match the type defined in <see cref="ParameterType"/>.
        /// </typeparam>
        /// <returns>
        ///     The single value selected by the parameter casted to the parameter type <typeparamref name="T"/>.  If no value
        ///     was selected, the default value for <typeparamref name="T"/> is returned.
        /// </returns>
        /// <exception cref="ArgumentException"><typeparamref name="T"/> does not match the type <see cref="ParameterType"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="ParameterType"/> is an array type.</exception>
        public T GetChosenValue<T>()
        {
            if (null == _rpRef)
                return default;

            if (this.ParameterType.IsArray)
                throw new InvalidOperationException(string.Format("{0} was defined as an array type.  Use '{1}<{2}>()' instead.",
                    this.Name, nameof(GetChosenValues), nameof(T)));

            if (!this.ParameterType.Equals(typeof(T)))
                throw new ArgumentException(
                    string.Format("The argument 'T' of type '{0}' is not equal to the parameter's defined type '{1}'.",
                    typeof(T).FullName, this.ParameterType.FullName));

            return (T)_rpRef.Value;
        }
        public T[] GetChosenValues<T>()
        {
            if (null == _rpRef)
                return null;

            if (!this.ParameterType.Equals(typeof(T[])))
                throw new ArgumentException(
                    string.Format("The argument 'T' of type '{0}' is not equal to the parameter's defined type '{1}'.",
                    typeof(T[]).FullName, this.ParameterType.FullName));

            if (this.ParameterType.IsArray && _rpRef.Value is ICollection icol)
            {
                return icol.Cast<T>().ToArray();
            }
            else
            {
                return new T[] { (T)_rpRef.Value };
            }
        }
        private void ReplaceAttribute<T>(Range? range, Func<Range, T> ctor)
            where T : ValidateArgumentsAttribute
        {
            if (range.HasValue)
            {
                if (this.Attributes.Contains<T>())
                    this.Attributes.Remove<T>();

                this.Attributes.Add(ctor(range.Value));
            }
            else
            {
                this.Attributes.Remove<T>();
            }
        }
        private void SetAttribute<T>(bool toggle)
            where T : CmdletMetadataAttribute, new()
        {
            this.SetAttribute(toggle, () => new T());
        }
        private void SetAttribute<T>(bool toggle, Func<T> ctor)
            where T : CmdletMetadataAttribute
        {
            if (toggle && !this.Attributes.Contains<T>())
                this.Attributes.Add(ctor());

            else if (!toggle)
                this.Attributes.Remove<T>();
        }
        private static bool TryCopyAliases(IList<string> list, out AliasAttribute attribute)
        {
            attribute = null;
            if (null != list && list.Count > 0)
            {
                string[] aliases = new string[list.Count];
                list.CopyTo(aliases, 0);
                attribute = new AliasAttribute(aliases);
            }

            return null != attribute;
        }
        private static bool TryCopyValidatedItems(ICollection<string> items, out ValidateSetAttribute attribute)
        {
            attribute = null;
            if (null != items && items.Count > 0)
            {
                string[] vItems = new string[items.Count];
                items.CopyTo(vItems, 0);
                attribute = new ValidateSetAttribute(vItems);
            }

            return null != attribute;
        }
    }
}
