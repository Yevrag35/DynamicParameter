using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace MG.Dynamic.Collections;

[CollectionBuilder(typeof(AttributeCollection), nameof(Create))]
public sealed partial class AttributeCollection : Collection<Attribute>, IEnumerable<Attribute>
{
	private static readonly FrozenSet<Type> s_singleAttributes = FrozenSet.Create(
		typeof(AliasAttribute),
		typeof(DefaultParameterValueAttribute),
		typeof(ValidateCountAttribute),
		typeof(ValidateSetAttribute),
		typeof(ValidateDriveAttribute),
		typeof(ValidateLengthAttribute),
		typeof(ValidateNotNullAttribute),
		typeof(ValidateNotNullOrEmptyAttribute),
		typeof(ValidateNotNullOrWhiteSpaceAttribute),
		typeof(ValidatePatternAttribute),
		typeof(ValidateRangeAttribute),
		typeof(ValidateScriptAttribute),
		typeof(ValidateTrustedDataAttribute),
		typeof(ValidateUserDriveAttribute),
		typeof(AllowEmptyCollectionAttribute),
		typeof(AllowEmptyStringAttribute),
		typeof(System.Management.Automation.AllowNullAttribute)
	);

	private ParameterAttribute _paramAtt;
	private readonly List<Attribute> _rawList;
	private readonly Dictionary<Type, Attribute> _singles;

	/// <summary>
	/// Gets the <see cref="ParameterAttribute"/> that is always present in the collection at index 0.
	/// </summary>
	public ParameterAttribute Parameter => _paramAtt;

	public AttributeCollection() : base(ReturnOut(new(1), out var list))
	{
		ParameterAttribute paramAtt = new();
		_paramAtt = paramAtt;
		list.Add(paramAtt);
		_rawList = list;
		_singles = [];
	}
	private AttributeCollection(List<Attribute> attributes, Dictionary<Type, Attribute> singles, ParameterAttribute? paramAttribute) : base(attributes)
	{
		if (paramAttribute is null)
		{
			paramAttribute = new ParameterAttribute();
			attributes.Insert(0, paramAttribute);
		}

		Debug.Assert(attributes.Count > 0 && attributes[0] is ParameterAttribute, "This should be a ParameterAttribute at this point.");
		_paramAtt = paramAttribute;
		_singles = singles;
		_rawList = attributes;
	}

	public bool CanAdd<T>(T attributeToCheck) where T : Attribute
	{
		if (attributeToCheck is null) return false;
		if (attributeToCheck is ParameterAttribute) return true;

		Type type = typeof(T);
		return !s_singleAttributes.Contains(type) || !_singles.ContainsKey(type);
	}
	public bool ContainsSingleAttributeType<T>() where T : Attribute
	{
		Type type = typeof(T);
		return type.Equals(typeof(ParameterAttribute))
			|| (s_singleAttributes.Contains(type) && _singles.ContainsKey(type));
	}
	public bool TryGetSingle<T>([NotNullWhen(true)] out T? attribute) where T : Attribute
	{
		Type type = typeof(T);
		if (type.Equals(typeof(ParameterAttribute)))
		{
			attribute = Unsafe.As<T>(_paramAtt);
			return true;
		}

		if (s_singleAttributes.Contains(type) && _singles.TryGetValue(type, out var att))
		{
			attribute = (T)att;
			return true;
		}

		attribute = null;
		return false;
	}

	protected override void ClearItems()
	{
		_singles.Clear();
		ListView view = Unsafe.As<ListView>(_rawList);
		view._version++;
		Attribute[] array = view._items;
		Array.Clear(array, 1, array.Length - 1);
		view._size = 1;
	}
	protected override void InsertItem(int index, Attribute item)
	{
		if (index == 0) this.SetItem(index, item);

		ArgumentNullException.ThrowIfNull(item);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _rawList.Count);

		if (IsSingleType(item, out var type))
		{
			if (!_singles.TryAdd(type, item))
			{
				throw new ArgumentException($"An attribute of type '{type.FullName ?? type.Name}' already exists in the collection.", nameof(item));
			}
		}

		_rawList[index] = item;
	}
	protected override void RemoveItem(int index)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		if (index == 0) throw new ArgumentException("Cannot remove the 'ParameterAttribute' for this collection.", nameof(index));

		Attribute attribute = _rawList[index];
		Type type = attribute.GetType();

		if (s_singleAttributes.Contains(type))
			_singles.Remove(type);

		_rawList.RemoveAt(index);
	}
	protected override void SetItem(int index, Attribute item)
	{
		ArgumentNullException.ThrowIfNull(item);
		if (index == 0)
		{
			if (item is ParameterAttribute paramAtt)
			{
				if (!ReferenceEquals(_paramAtt, paramAtt))
				{
					_rawList[0] = paramAtt;
					_paramAtt = paramAtt;
				}
			}
			else
			{
				throw new ArgumentException("Only a ParameterAttribute can be set at index 0.");
			}

			return;
		}

		if (IsSingleType(item, out var type))
		{
			_singles[type] = item;
		}

		_rawList[index] = item;
	}

	private static bool IsSingleType(Attribute attribute, out Type attributeType)
	{
		attributeType = attribute.GetType();
		return s_singleAttributes.Contains(attributeType);
	}
	private static List<Attribute> ReturnOut(List<Attribute> list, out List<Attribute> output)
	{
		output = list;
		return list;
	}

	private sealed class ListView
	{
		internal Attribute[] _items = null!;
		internal int _size;
		internal int _version;
	}

	[SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "This is a Create method.")]
	public static AttributeCollection CreateFrom(IEnumerable<Attribute> collection)
	{
		return collection switch
		{
			Attribute[] array => Create(values: array),
			List<Attribute> list => Create(values: GetListAsSpan(list)),
			_ => fromEnumerable(collection),
		};

		static AttributeCollection fromEnumerable(IEnumerable<Attribute> attributes)
		{
			if (attributes.TryGetNonEnumeratedCount(out int count) && count == 0)
			{
				return new AttributeCollection();
			}

			ParameterAttribute? pAtt = null;
			Dictionary<Type, Attribute> singles = [];
			List<Attribute> list = new(1 + count);

			foreach (Attribute attribute in attributes)
			{
				AddAttributeToListAndDictionary(attribute, list, singles, ref pAtt);
			}

			return new(list, singles, pAtt);
		}
	}

	[SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "This is the Create method.")]
	public static AttributeCollection Create(params ReadOnlySpan<Attribute> values)
	{
		if (values.IsEmpty)
		{
			return new AttributeCollection();
		}

		ParameterAttribute? pAtt = null;
		Dictionary<Type, Attribute> singles = [];
		List<Attribute> list = new(values.Length);

		foreach (Attribute attribute in values)
		{
			AddAttributeToListAndDictionary(attribute, list, singles, ref pAtt);
		}

		return new(list, singles, pAtt);
	}

	private static void AddAttributeToListAndDictionary(Attribute attribute, List<Attribute> list, Dictionary<Type, Attribute> singles, ref ParameterAttribute? pAtt)
	{
		ArgumentNullException.ThrowIfNull(attribute);
		Type type = attribute.GetType();

		if (type.Equals(typeof(ParameterAttribute)) && pAtt is null)
		{
			pAtt = (ParameterAttribute)attribute;
			list.Insert(0, pAtt);
			return;
		}

		if (!s_singleAttributes.Contains(type) || singles.TryAdd(type, attribute))
		{
			list.Add(attribute);
		}
	}
	private static ReadOnlySpan<Attribute> GetListAsSpan(List<Attribute> list)
	{
		if (list.Count == 0)
			return [];

		var view = Unsafe.As<ListView>(list);
		Attribute[] array = view._items;
		int size = view._size;

		return new ReadOnlySpan<Attribute>(array, 0, size);
	}
}

