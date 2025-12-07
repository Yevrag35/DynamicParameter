using MG.Dynamic.Internal;

namespace MG.Dynamic.Collections;

public sealed class ValidateSetCollection<TValue> : AttributeCollection where TValue : notnull
{
	private static readonly string[] s_bogus = [ParameterAttribute.AllParameterSets];

	private readonly Dictionary<string, TValue> _lookup;
	private ValidateSetAttribute _constructedSet;

	public int LookupCount => _lookup.Count;
	public IReadOnlyList<string> ValidValues => ((IReadOnlyList<string>?)_constructedSet.ValidValues) ?? Array.Empty<string>();

	public ValidateSetCollection(IEqualityComparer<string>? comparer = null) : base()
	{
		_lookup = new(comparer ?? StringComparer.OrdinalIgnoreCase);
		ValidateSetAttribute vs = new(s_bogus);
		this.Add(vs);
		_constructedSet = vs;
		ValidateSetMarshal.GetValuesRef(_constructedSet) = [];
	}

	public bool AddValidValue(string key, TValue value)
	{
		if (_lookup.TryAdd(key, value))
		{
			ref string[] values = ref ValidateSetMarshal.GetValuesRef(_constructedSet)!;
			Array.Resize(ref values, values.Length + 1);
			values[^1] = key;
			Array.Sort(values, StringComparer.OrdinalIgnoreCase);

			return true;
		}

		return false;
	}
	public override bool CanAdd<T>(T attributeToCheck)
	{
		return attributeToCheck is not ValidateSetAttribute && base.CanAdd(attributeToCheck);
	}

	public bool TryGetResult(string key, [NotNullWhen(true)] out TValue? result)
	{
		ArgumentNullException.ThrowIfNull(key);
		return _lookup.TryGetValue(key, out result);
	}

	protected override void ClearItems(int startingIndex)
	{
		_lookup.Clear();
		ValidateSetMarshal.SetValuesToEmpty(_constructedSet);
		base.ClearItems(startingIndex + 1);
	}

	protected override void InsertItem(int index, Attribute item)
	{
		if (item is not ValidateSetAttribute vSet)
		{
			base.InsertItem(index, item);
			return;
		}

		if (index == 1)
		{
			if (ReferenceEquals(vSet, _constructedSet))
			{
				return;
			}

			_constructedSet = vSet;
			base.InsertItem(1, vSet);
			return;
		}

		throw new InvalidOperationException("Cannot insert additional ValidateSetAttribute instances into the collection.");
	}
	protected override void RemoveItem(int index)
	{
		if (index == 1) throw new ArgumentException("Cannot remove the ValidateSetAttribute from the collection.");
		base.RemoveItem(index);
	}
	protected override void SetItem(int index, Attribute item)
	{
		if (index == 1)
		{
			if (item is not ValidateSetAttribute vSet) throw new ArgumentException("The item at index '1' must be a ValidateSetAttribute instance.");

			if (ReferenceEquals(vSet, _constructedSet))
			{
				return;
			}

			_constructedSet = vSet;
		}

		base.SetItem(index, item);
	}
}
