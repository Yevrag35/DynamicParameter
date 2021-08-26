using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;

namespace TempDynamic
{
    public class RuntimeParameter
    {
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

            return new RuntimeDefinedParameter(rp.Name, rp.ParameterType, rp.Attributes);
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
                vItems.CopyTo(vItems, 0);
                attribute = new ValidateSetAttribute(vItems);
            }

            return null != attribute;
        }
    }
}
