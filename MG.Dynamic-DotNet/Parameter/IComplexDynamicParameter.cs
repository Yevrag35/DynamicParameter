using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MG.Dynamic.Parameter.Generic
{
    public interface IComplexDynamicParameter<TKey, TOutput> : IRuntimeParameter where TKey : IConvertible
    {
        Func<TOutput, TKey> PropertyFunction { get; }

        ICollection<TOutput> GetBackingItems();
        TOutput GetPropertyFromValue(TKey chosenValue);
        IEnumerable<TOutput> GetPropertiesFromValue(TKey chosenValue);
        IEnumerable<TOutput> GetPropertiesFromValues(IEnumerable<TKey> chosenValues);

        void SetDynamicItems(IEnumerable<TOutput> items, Expression<Func<TOutput, TKey>> propertyExpression);
        void SetDynamicItems(IEnumerable<TOutput> items, bool isArrayType, Expression<Func<TOutput, TKey>> propertyExpression);
    }
}
