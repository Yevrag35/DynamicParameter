using MG.Dynamic.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic.Parameter
{
    /// <summary>
    /// Defines properties and methods to store underlying values that 
    /// create a dynamic 'ValidateSet' off of one of its properties to be used within a <see cref="IDynamicLibrary"/>.
    /// </summary>
    public interface IDynParam : IRuntimeParameter
    {
        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        object GetItemFromChosenValue(object chosenValue);

        /// <summary>
        /// Finds the underlying objects that match the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValues">The values selected after IDynamicParameters has been processed.</param>
        IEnumerable<object> GetItemsFromChosenValues(IEnumerable<object> chosenValues);

        /// <summary>
        /// Retrieves all the underlying objects that were used to build the ValidateSet.
        /// </summary>
        ICollection GetBackingItems();
        //object[] GetBackingItems();
    }
}