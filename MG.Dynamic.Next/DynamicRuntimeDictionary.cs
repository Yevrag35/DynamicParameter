namespace MG.Dynamic;

public sealed class DynamicRuntimeDictionary : RuntimeDefinedParameterDictionary
{
	private readonly AlternateLookup<ReadOnlySpan<char>> _alternate;
	private readonly DynamicRuntimeLibrary? _parent;

	public DynamicRuntimeDictionary() : base()
	{
		_alternate = this.GetAlternateLookup<ReadOnlySpan<char>>();
	}
	internal DynamicRuntimeDictionary(DynamicRuntimeLibrary? parentLibrary) : this()
	{
		Debug.Assert(parentLibrary is null || parentLibrary.IsParentTo(this), "The provided parent library does not own this dictionary.");
		_parent = parentLibrary;
	}

	public void Add(RuntimeDefinedParameter parameter)
	{
		this.Add(parameter.Name, parameter);
	}
	public bool ContainsKey(ReadOnlySpan<char> key)
	{
		return _alternate.ContainsKey(key);
	}
	public bool Remove(ReadOnlySpan<char> key)
	{
		return _alternate.Remove(key);
	}
	internal bool TryGetParentLibrary([NotNullWhen(true)] out DynamicRuntimeLibrary? parentLibrary)
	{
		parentLibrary = _parent;
		return parentLibrary is not null;
	}
	public bool TryGetValue(ReadOnlySpan<char> key, out RuntimeDefinedParameter? value)
	{
		return _alternate.TryGetValue(key, out value);
	}
}
