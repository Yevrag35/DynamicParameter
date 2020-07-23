using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;

namespace MG.Dynamic.Parameter
{
    public class ComplexDynamicParameter<T1, T2> : DynamicParameter<T1>, IComplexDynamicParameter<T1, T2> where T2 : IConvertible
    {
        public Func<T1, T2> PropertyFunction { get; set; }

        public ComplexDynamicParameter(string name) => base.Name = name;

        public T1 GetPropertyFromValue(T2 chosenValue)
        {
            T1 ret = default;
            foreach (T1 item in this.BackingItems)
            {
                if (this.PropertyFunction(item).Equals(chosenValue))
                {
                    ret = item;
                    break;
                }
            }
            return ret;
        }
        public IEnumerable<T1> GetPropertiesFromValue(T2 chosenValue)
        {
            foreach (T1 item in this.BackingItems)
            {
                if (this.PropertyFunction(item).Equals(chosenValue))
                {
                    yield return item;
                }
            }
        }
        public IEnumerable<T1> GetPropertiesFromValues(IEnumerable<T2> chosenValues)
        {
            foreach (T1 item in this.BackingItems)
            {
                foreach (T2 chosen in chosenValues)
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
        /// <see cref="Expression{Func{T1, T2}}"/> to the parameter's ValidateSet.
        /// </summary>
        /// <param name="items">The collection whose items will be considered the backing items.</param>
        /// <param name="propertyExpression">
        ///     The property expression that will evaluated to find the ValidateSet values.
        /// </param>
        /// <remarks>
        ///     An <see cref="Expression"/> must be provided or <see cref="ComplexDynamicParameter{T1, T2}.PropertyFunction"/> be defined
        ///     first before this method can be executed.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="propertyExpression"/> was null and <see cref="ComplexDynamicParameter{T1, T2}.PropertyFunction"/>
        ///     has not been defined.
        /// </exception>
        public void SetDynamicItems(IEnumerable<T1> items, Expression<Func<T1, T2>> propertyExpression = null) =>
            this.SetDynamicItems(items, false, propertyExpression);

        /// <summary>
        /// Adds the specified collection as backing items while adding the values evaulated by the given or specified 
        /// <see cref="Expression{Func{T1, T2}}"/> to the parameter's ValidateSet while also specifying if <typeparamref name="T2"/>
        /// is an array type.
        /// </summary>
        /// <param name="items">The collection whose items will be considered the backing items.</param>
        /// <param name="isArrayType">Specifies if the <see cref="RuntimeDefinedParameter"/> type will be a single-dimensional array type.</param>
        /// <param name="propertyExpression">
        ///     The property expression that will evaluated to find the ValidateSet values.
        /// </param>
        /// <remarks>
        ///     An <see cref="Expression"/> must be provided or <see cref="ComplexDynamicParameter{T1, T2}.PropertyFunction"/> be defined
        ///     first before this method can be executed.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="propertyExpression"/> was null and <see cref="ComplexDynamicParameter{T1, T2}.PropertyFunction"/>
        ///     has not been defined.
        /// </exception>
        public void SetDynamicItems(IEnumerable<T1> items, bool isArrayType, Expression<Func<T1, T2>> propertyExpression = null)
        {
            if (this.PropertyFunction == null && propertyExpression == null)
                throw new InvalidOperationException("An expression must be specified or PropertyFunction defined before running SetDynamicItems.");
                
            else if (this.PropertyFunction == null)
                this.PropertyFunction = propertyExpression.Compile();

            this.BackingItems = new List<T1>(items);
            foreach (IConvertible icon in items.Select(this.PropertyFunction))
            {
                this.ValidatedItems.Add(Convert.ToString(icon));
            }

            Type type = typeof(T2);
            if (isArrayType)
                type = type.MakeArrayType();

            this.ParameterType = type;
        }
    }
}
