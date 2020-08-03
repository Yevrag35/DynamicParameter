using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;

namespace MG.Dynamic.Parameter.Generic
{
    public class ComplexDynamicParameter<TKey, TOutput> : DynamicParameter<TOutput>, IComplexDynamicParameter<TKey, TOutput> where TKey : IConvertible
    {
        private Type _paramType = typeof(TKey);


        public Func<TOutput, TKey> PropertyFunction { get; set; }
        public override Type ParameterType
        {
            get => _paramType;
            set => _paramType = value;
        }

        public ComplexDynamicParameter(string name) : base(name) { }

        ICollection<TOutput> IComplexDynamicParameter<TKey, TOutput>.GetBackingItems() => base.BackingItems;
        public TOutput GetPropertyFromValue(TKey chosenValue)
        {
            TOutput ret = default;
            foreach (TOutput item in this.BackingItems)
            {
                if (this.PropertyFunction(item).Equals(chosenValue))
                {
                    ret = item;
                    break;
                }
            }
            return ret;
        }
        public IEnumerable<TOutput> GetPropertiesFromValue(TKey chosenValue)
        {
            foreach (TOutput item in this.BackingItems)
            {
                if (this.PropertyFunction(item).Equals(chosenValue))
                {
                    yield return item;
                }
            }
        }
        public IEnumerable<TOutput> GetPropertiesFromValues(IEnumerable<TKey> chosenValues)
        {
            foreach (TOutput item in this.BackingItems)
            {
                foreach (TKey chosen in chosenValues)
                {
                    if (this.PropertyFunction(item).Equals(chosen))
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified collection as backing items while adding the values evaulated by the given or specified 
        /// <see cref="Expression{Func{TKey, TOutput}}"/> to the parameter's ValidateSet.
        /// </summary>
        /// <param name="items">The collection whose items will be considered the backing items.</param>
        /// <param name="propertyExpression">
        ///     The property expression that will evaluated to find the ValidateSet values.
        /// </param>
        /// <remarks>
        ///     An <see cref="Expression"/> must be provided or <see cref="ComplexDynamicParameter{TKey, TOutput}.PropertyFunction"/> be defined
        ///     first before this method can be executed.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="propertyExpression"/> was null and <see cref="ComplexDynamicParameter{TKey, TOutput}.PropertyFunction"/>
        ///     has not been defined.
        /// </exception>
        public void SetDynamicItems(IEnumerable<TOutput> items, Expression<Func<TOutput, TKey>> propertyExpression = null) =>
            this.SetDynamicItems(items, false, propertyExpression);

        /// <summary>
        /// Adds the specified collection as backing items while adding the values evaulated by the given or specified 
        /// <see cref="Expression{Func{TKey, TOutput}}"/> to the parameter's ValidateSet while also specifying if <typeparamref name="TKey"/>
        /// is an array type.
        /// </summary>
        /// <param name="items">The collection whose items will be considered the backing items.</param>
        /// <param name="isArrayType">Specifies if the <see cref="RuntimeDefinedParameter"/> type will be a single-dimensional array type.</param>
        /// <param name="propertyExpression">
        ///     The property expression that will evaluated to find the ValidateSet values.
        /// </param>
        /// <remarks>
        ///     An <see cref="Expression"/> must be provided or <see cref="ComplexDynamicParameter{TKey, TOutput}.PropertyFunction"/> be defined
        ///     first before this method can be executed.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="propertyExpression"/> was null and <see cref="ComplexDynamicParameter{TKey, TOutput}.PropertyFunction"/>
        ///     has not been defined.
        /// </exception>
        public void SetDynamicItems(IEnumerable<TOutput> items, bool isArrayType, Expression<Func<TOutput, TKey>> propertyExpression = null)
        {
            if (this.PropertyFunction == null && propertyExpression == null)
                throw new InvalidOperationException("An expression must be specified or PropertyFunction defined before running SetDynamicItems.");
                
            else if (this.PropertyFunction == null)
                this.PropertyFunction = propertyExpression.Compile();

            _backingItems = new List<TOutput>(items);
            foreach (IConvertible icon in items.Select(this.PropertyFunction))
            {
                this.ValidatedItems.Add(Convert.ToString(icon));
            }

            Type type = typeof(TKey);
            if (isArrayType)
                type = type.MakeArrayType();

            this.ParameterType = type;
        }
    }
}
