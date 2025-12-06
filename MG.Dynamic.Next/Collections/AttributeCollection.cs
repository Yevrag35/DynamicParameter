using MG.Dynamic.Internal;
using System.Collections.Frozen;
using System.Collections.ObjectModel;

namespace MG.Dynamic.Collections;

[CollectionBuilder(typeof(AttributeCollection), nameof(Create))]
public partial class AttributeCollection : Collection<Attribute>, IEnumerable<Attribute>
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

	private readonly List<Attribute> _allAttributes;
	private List<ParameterAttribute>? _allParameters; // lazily instantiated
	private ParameterAttribute _paramAtt;
	private readonly Dictionary<Type, Attribute> _singles;

	public new Attribute this[int index]
	{
		get => _allAttributes[index];
		set => base.SetItem(index, value);
	}

	/// <summary>
	/// Gets the <see cref="ParameterAttribute"/> that is always present in the collection at index 0.
	/// </summary>
	public ParameterAttribute DefaultParameter => _paramAtt;

	public AttributeCollection() : base(ReturnOut(new(1), out var list))
	{
		ParameterAttribute paramAtt = new();
		_paramAtt = paramAtt;
		list.Add(paramAtt);
		_allAttributes = list;
		_singles = [];
		_allParameters = null; // only created when a second parameter is added
	}

	private protected AttributeCollection(List<Attribute> attributes, Dictionary<Type, Attribute> singles, ParameterAttribute? paramAttribute)
		: base(attributes)
	{
		bool hasParamAtt = paramAttribute is not null;
		if (!hasParamAtt)
		{
			paramAttribute = new ParameterAttribute();
			attributes.Insert(0, paramAttribute);
		}

		Debug.Assert(attributes.Count > 0 && attributes[0] is ParameterAttribute, "This should be a ParameterAttribute at this point.");

		_allAttributes = attributes;
		_singles = singles;
		_paramAtt = paramAttribute!;

		// Lazily build _allParameters only if there is more than one ParameterAttribute
		List<ParameterAttribute>? allParams = null;

		if (hasParamAtt)
		{
			for (int i = 1; i < attributes.Count; i++)
			{
				if (attributes[i] is ParameterAttribute p)
				{
					if (allParams is null)
					{
						allParams = [_paramAtt, p];
					}
					else
					{
						allParams.Add(p);
					}
				}
			}
		}

		_allParameters = allParams;
	}

	public virtual bool CanAdd<T>(T attributeToCheck) where T : Attribute
	{
		if (attributeToCheck is null) return false;
		if (attributeToCheck is ParameterAttribute) return true;

		Type type = typeof(T);
		return !s_singleAttributes.Contains(type) || !_singles.ContainsKey(type);
	}

	public bool ContainsSingleAttributeType<T>() where T : Attribute
	{
		Type type = typeof(T);
		return s_singleAttributes.Contains(type) && _singles.ContainsKey(type);
	}

	public T GetOrAdd<T>() where T : Attribute, new()
	{
		Type type = typeof(T);
		if (s_singleAttributes.Contains(type))
		{
			ref Attribute? attr = ref CollectionsMarshal.GetValueRefOrAddDefault(_singles, type, out bool exists);
			if (exists) return (T)attr!;

			T tAtt = new();
			_allAttributes.Add(tAtt);
			attr = tAtt;
			return tAtt;
		}

		T newAtt = new();
		_allAttributes.Add(newAtt);
		return newAtt;
	}
	public T GetOrAdd<T>(Func<T> factory) where T : Attribute
	{
		Type type = typeof(T);
		if (s_singleAttributes.Contains(type))
		{
			ref Attribute? attr = ref CollectionsMarshal.GetValueRefOrAddDefault(_singles, type, out bool exists);
			if (exists) return (T)attr!;

			T tAtt = factory();
			_allAttributes.Add(tAtt);
			attr = tAtt;
			return tAtt;
		}

		T newAtt = factory();
		_allAttributes.Add(newAtt);
		return newAtt;
	}
	public T GetOrAdd<T, TState>(TState state, Func<TState, T> factory) where T : Attribute where TState : allows ref struct
	{
		Type type = typeof(T);
		if (s_singleAttributes.Contains(type))
		{
			ref Attribute? attr = ref CollectionsMarshal.GetValueRefOrAddDefault(_singles, type, out bool exists);
			if (exists) return (T)attr!;

			T tAtt = factory(state);
			_allAttributes.Add(tAtt);
			attr = tAtt;
			return tAtt;
		}

		T newAtt = factory(state);
		_allAttributes.Add(newAtt);
		return newAtt;
	}

	public ReadOnlySpan<ParameterAttribute> GetParameterAttributes()
	{
		return _allParameters is null
			? MemoryMarshal.CreateReadOnlySpan(in _paramAtt, 1)
			: GetListAsSpan(_allParameters);
	}

	public bool TryAdd<T>(T attribute) where T : Attribute
	{
		if (attribute is null) return false;

		if (attribute is ParameterAttribute pAtt)
		{
			this.AddNonDefaultParameterToAllParameters(pAtt);
			_allAttributes.Add(pAtt);
			return true;
		}

		Type type = typeof(T);
		if (!s_singleAttributes.Contains(type) || _singles.TryAdd(type, attribute))
		{
			_allAttributes.Add(attribute);
			return true;
		}

		return false;
	}

	public bool TryGetSingle<T>([NotNullWhen(true)] out T? attribute) where T : Attribute
	{
		Type type = typeof(T);
		if (s_singleAttributes.Contains(type) && _singles.TryGetValue(type, out var att))
		{
			attribute = (T)att;
			return true;
		}

		attribute = null;
		return false;
	}

	protected sealed override void ClearItems()
	{
		this.ClearCore(startingIndex: 1);
	}
	protected virtual void ClearItems(int startingIndex)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(startingIndex);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(startingIndex, _allAttributes.Count);

		this.ClearCore(startingIndex);
	}
	private void ClearCore(int startingIndex)
	{
		_singles.Clear();
		ListView<Attribute> view = Unsafe.As<ListView<Attribute>>(_allAttributes);
		view._version++;
		Attribute[] array = view._items;
		Array.Clear(array, startingIndex, array.Length - startingIndex);
		view._size = startingIndex;

		if (_allParameters is not null)
		{
			ListView<ParameterAttribute> pView = Unsafe.As<ListView<ParameterAttribute>>(_allParameters);
			pView._version++;
			ParameterAttribute[] pArray = pView._items;
			Array.Clear(pArray, 1, pArray.Length - 1);
			pView._size = 1;

			Debug.Assert(ReferenceEquals(pArray[0], _paramAtt), "The default parameter attribute should still be present.");
			Debug.Assert(ReferenceEquals(pArray[0], _allAttributes[0]), "The default parameter attribute should still be present.");
		}

		_paramAtt.Reset();
	}

	protected override void InsertItem(int index, Attribute item)
	{
		if (index == 0)
		{
			this.SetItem(0, item);
			return;
		}

		ArgumentNullException.ThrowIfNull(item);
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(index, _allAttributes.Count);

		if (item is ParameterAttribute pAtt)
		{
			this.AddNonDefaultParameterToAllParameters(pAtt);
		}

		if (IsSingleType(item, out var type))
		{
			if (!_singles.TryAdd(type, item))
			{
				throw new ArgumentException(
					$"An attribute of type '{type.FullName ?? type.Name}' already exists in the collection.",
					nameof(item));
			}
		}

		_allAttributes.Insert(index, item);
	}

	protected override void RemoveItem(int index)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(index);
		if (index == 0)
		{
			throw new ArgumentException("Cannot remove the 'ParameterAttribute' for this collection.", nameof(index));
		}

		Attribute attribute = _allAttributes[index];
		Type type = attribute.GetType();

		if (s_singleAttributes.Contains(type))
		{
			_singles.Remove(type);
		}

		if (attribute is ParameterAttribute pAtt)
		{
			this.RemoveNonDefaultParameterFromAllParameters(pAtt);
		}

		_allAttributes.RemoveAt(index);
	}

	protected override void SetItem(int index, Attribute item)
	{
		ArgumentNullException.ThrowIfNull(item);

		if (index == 0)
		{
			if (item is not ParameterAttribute paramAtt)
			{
				throw new ArgumentException("Only a ParameterAttribute can be set at index 0.");
			}

			this.ReplaceDefaultParameter(paramAtt);
			return;
		}

		Attribute existing = _allAttributes[index];

		if (existing is ParameterAttribute existingParam && !ReferenceEquals(existingParam, _paramAtt))
		{
			this.RemoveNonDefaultParameterFromAllParameters(existingParam);
		}

		if (item is ParameterAttribute newParam && !ReferenceEquals(newParam, _paramAtt))
		{
			this.AddNonDefaultParameterToAllParameters(newParam);
		}

		if (IsSingleType(item, out var type))
		{
			_singles[type] = item;
		}

		_allAttributes[index] = item;
	}

	private void ReplaceDefaultParameter(ParameterAttribute newDefault)
	{
		ParameterAttribute oldDefault = _paramAtt;

		// Same instance: just normalize ordering if needed.
		if (ReferenceEquals(oldDefault, newDefault))
		{
			// Ensure default is at index 0 in _allAttributes
			if (!ReferenceEquals(_allAttributes[0], oldDefault))
			{
				int idx = _allAttributes.IndexOf(oldDefault);
				if (idx > 0)
				{
					this.RemoveItem(idx);
					this.InsertItem(0, oldDefault);
				}
			}

			// Ensure default is at index 0 in _allParameters if it exists.
			if (_allParameters is not null)
			{
				int idx = _allParameters.IndexOf(oldDefault);
				if (idx > 0)
				{
					_allParameters.RemoveAt(idx);
					_allParameters.Insert(0, oldDefault);
				}
			}

			return;
		}

		// If the new default already exists as a non-default parameter, remove that occurrence.
		int existingIndex = _allAttributes.IndexOf(newDefault);
		if (existingIndex >= 0)
		{
			this.RemoveItem(existingIndex);
		}

		// Insert new default at index 0; old default moves to index 1 automatically.
		this.InsertItem(0, newDefault);
		_paramAtt = newDefault;

		if (_allParameters is null)
		{
			// This is the first time we have two ParameterAttribute instances.
			_allParameters = [newDefault, oldDefault];
		}
		else
		{
			// Normalize _allParameters: new default at index 0, old default at index 1.
			_allParameters.Remove(newDefault);
			_allParameters.Remove(oldDefault);

			_allParameters.Insert(0, newDefault);
			_allParameters.Insert(1, oldDefault);
		}
	}

	private void AddNonDefaultParameterToAllParameters(ParameterAttribute parameter)
	{
		// Default always tracked as _paramAtt; this helper is only for "additional" parameters.
		if (ReferenceEquals(parameter, _paramAtt))
		{
			// Do not treat the default as an "additional" parameter.
			if (_allParameters is not null)
			{
				// Ensure it is at index 0 if it somehow was not.
				int idx = _allParameters.IndexOf(parameter);
				if (idx > 0)
				{
					_allParameters.RemoveAt(idx);
					_allParameters.Insert(0, parameter);
				}
			}

			return;
		}

		if (_allParameters is null)
		{
			_allParameters = [_paramAtt, parameter];
		}
		else
		{
			_allParameters.Add(parameter);
		}
	}

	private void RemoveNonDefaultParameterFromAllParameters(ParameterAttribute parameter)
	{
		if (_allParameters is null)
		{
			return;
		}

		int index = _allParameters.IndexOf(parameter);
		if (index <= 0)
		{
			// Either not found or it is the default at index 0 (which we never remove here).
			return;
		}

		_allParameters.RemoveAt(index);

		// If we are back down to just the default, we can drop the list again.
		if (_allParameters.Count == 1 && ReferenceEquals(_allParameters[0], _paramAtt))
		{
			_allParameters = null;
		}
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

	private static void AddAttributeToListAndDictionary(
		Attribute attribute,
		List<Attribute> list,
		Dictionary<Type, Attribute> singles,
		ref ParameterAttribute? pAtt)
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

	private sealed class ListView<T> where T : Attribute
	{
		internal T[] _items = null!;
		internal int _size;
		internal int _version;
	}
	private static ReadOnlySpan<T> GetListAsSpan<T>(List<T> list) where T : Attribute
	{
		if (list.Count == 0)
		{
			return [];
		}

		var view = Unsafe.As<ListView<T>>(list);
		T[] array = view._items;
		int size = view._size;

		return new ReadOnlySpan<T>(array, 0, size);
	}
}
