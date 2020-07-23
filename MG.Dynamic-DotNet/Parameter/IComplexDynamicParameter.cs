using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MG.Dynamic.Parameter
{
    public interface IComplexDynamicParameter<T1, T2> : IRuntimeParameter where T2 : IConvertible
    {
        ICollection<T1> GetBackingItems();
        T1 GetPropertyFromValue(T2 chosenValue);
        IEnumerable<T1> GetPropertiesFromValue(T2 chosenValue);
        IEnumerable<T1> GetPropertiesFromValues(IEnumerable<T2> chosenValues);

        void SetDynamicItems(IEnumerable<T1> items, Expression<Func<T1, T2>> propertyExpression);
        void SetDynamicItems(IEnumerable<T1> items, bool isArrayType, Expression<Func<T1, T2>> propertyExpression);
    }
}
