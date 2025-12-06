namespace MG.Dynamic;

public sealed class DynamicRuntimeLibrary
{
	public DynamicRuntimeDictionary Dictionary { get; }

	public DynamicRuntimeLibrary()
    {
		this.Dictionary = new(this);
    }

    internal bool IsParentTo([NotNullWhen(true)] DynamicRuntimeDictionary? dictionary)
    {
        return ReferenceEquals(dictionary, this.Dictionary);
    }
    public static implicit operator DynamicRuntimeDictionary(DynamicRuntimeLibrary library)
    {
        return library.Dictionary;
    }
    public static explicit operator DynamicRuntimeLibrary?(RuntimeDefinedParameterDictionary dictionary)
    {
        return dictionary is DynamicRuntimeDictionary dDict && dDict.TryGetParentLibrary(out var parent)
            ? parent
            : null;
    }
    public static explicit operator DynamicRuntimeLibrary?(DynamicRuntimeDictionary dictionary)
    {
        return dictionary.TryGetParentLibrary(out var parent) ? parent : null;
    }
}
