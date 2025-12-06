namespace MG.Dynamic.Internal;

internal static class ValidateSetMarshal
{
    internal static ref string[]? GetValuesRef(ValidateSetAttribute attribute)
    {
        return ref GetValidValuesField(attribute);
    }

    internal static string[] ResizeValues(ValidateSetAttribute attribute, int newSize)
    {
        ref string[]? values = ref GetValidValuesField(attribute);
        if (Unsafe.IsNullRef(in values) || values is null)
        {
            values = new string[newSize];
            return values;
        }

        Array.Resize(ref values, newSize);
        return values;
    }
    internal static void SetValuesToEmpty(ValidateSetAttribute attribute)
    {
        GetValidValuesField(attribute) = [];
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_validValues")]
    private static extern ref string[]? GetValidValuesField(ValidateSetAttribute attribute);
}
