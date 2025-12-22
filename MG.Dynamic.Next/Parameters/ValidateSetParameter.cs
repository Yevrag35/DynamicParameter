using MG.Dynamic.Collections;

namespace MG.Dynamic.Parameters;

public sealed class ValidateSetParameter<T> : DynamicRuntimeParameter where T : notnull
{
	private readonly ValidateSetCollection<T> _collection;

	public ValidateSetParameter(IEqualityComparer<string>? comparer = null) : base(ReturnAndOut(comparer, out var collection))
	{
		_collection = collection;
	}

	public bool TryAddValidValue(string key, T value)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);
		return _collection.AddValidValue(key, value);
	}
	public bool TryGetResult([NotNullWhen(true)] out T? result)
	{
		if (this.IsSet
			&&
			LanguagePrimitives.TryConvertTo(this.Value, out string? key)
			&&
			key is not null)
		{
			return _collection.TryGetResult(key, out result);
		}

		result = default;
		return false;
	}

	private static ValidateSetCollection<T> ReturnAndOut(IEqualityComparer<string>? comparer, out ValidateSetCollection<T> collection)
	{
		return collection = new ValidateSetCollection<T>(comparer);
	}
}
