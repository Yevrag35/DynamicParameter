using MG.Dynamic.Parameter.Generic;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Dynamic.Library.Generic
{
    public interface IComplexDynamicLibrary<TKey, TOutput> : IEnumerable<IComplexDynamicParameter<TKey, TOutput>>, IRuntimeLibrary where TKey : IConvertible
    {

        void Add(IComplexDynamicParameter<TKey, TOutput> parameter);
        void Add(IEnumerable<IComplexDynamicParameter<TKey, TOutput>> parameters);

        IList<TOutput> GetBackingItems(TKey key);
        TOutput GetOutputValue(TKey key);
        IEnumerable<TOutput> GetOutputValues(TKey key);

    }
}
