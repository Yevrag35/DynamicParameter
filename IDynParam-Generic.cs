using System;

namespace MG.Dynamic
{
    public interface IDynParam<T> : IDynParam
    {
        T GetChosenValue(Func<T, bool> predicate);
    }
}
