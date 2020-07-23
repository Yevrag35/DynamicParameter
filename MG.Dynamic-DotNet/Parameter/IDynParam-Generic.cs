using MG.Dynamic.Library;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Dynamic.Parameter
{
    /// <summary>
    /// Defines properties and methods to store underlying values that 
    /// create a dynamic 'ValidateSet' off of one of its properties to be used within a <see cref="IDynamicLibrary"/>.
    /// </summary>
    /// <typeparam name="T">The .NET type of the underlying collections of dynamic items.</typeparam>
    public interface IDynParam<T> : IRuntimeParameter
    {
        /// <summary>
        /// Retrieves all the underlying objects that were used to build the ValidateSet.
        /// </summary>
        ICollection<T> GetBackingItems();
        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        T GetItemFromChosenValue(object chosenValue);
        /// <summary>
        /// Finds the underlying objects that match the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValues">The values selected after IDynamicParameters has been processed.</param>
        IEnumerable<T> GetItemsFromChosenValues(IEnumerable<object> chosenValues);
    }
}
