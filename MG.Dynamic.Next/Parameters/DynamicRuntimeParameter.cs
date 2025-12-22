using MG.Dynamic.Collections;
using System.Collections.ObjectModel;

namespace MG.Dynamic.Parameters;

public partial class DynamicRuntimeParameter : RuntimeDefinedParameter, IEnumerable<Attribute>
{
	private readonly AttributeCollection _attributes;

	/// <inheritdoc cref="RuntimeDefinedParameter.Attributes"/>
	public new AttributeCollection Attributes => _attributes;

	/// <inheritdoc cref="RuntimeDefinedParameter.Name" path="/*[not(self::exception)]"/>
	public new string Name
	{
		get => GetNameField(this);
		set
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				value = string.Empty;
			}

			GetNameField(this) = value;
		}
	}
	/// <summary>
	/// Gets the default parameter attribute that identifies this parameter to cmdlets.
	/// </summary>
	public ParameterAttribute DefaultParameter => _attributes.DefaultParameter;

	/// <inheritdoc cref="RuntimeDefinedParameter.ParameterType" path="/*[not(self::exception)]"/>
	public new Type? ParameterType
	{
		get => GetParameterTypeField(this);
		set => GetParameterTypeField(this) = value;
	}

	public DynamicRuntimeParameter(params ReadOnlySpan<Attribute> attributes)
		: this(AttributeCollection.Create(attributes))
	{
	}
	public DynamicRuntimeParameter(string name, Type parameterType, params ReadOnlySpan<Attribute> attributes)
		: base(name, parameterType, ReturnAndOut(out var collection, attributes))
	{
		_attributes = collection;
	}
	public DynamicRuntimeParameter(string name, Type parameterType, IEnumerable<Attribute> attributes)
		: base(name, parameterType, ReturnAndOut(out var collection, attributes))
	{
		_attributes = collection;
	}
	protected DynamicRuntimeParameter(AttributeCollection attributes) : base()
	{
		_attributes = attributes;
		GetAttributes(this) = _attributes;
	}

	public AttributeCollection.Enumerator GetEnumerator()
	{
		return _attributes.GetEnumerator();
	}
	IEnumerator<Attribute> IEnumerable<Attribute>.GetEnumerator()
	{
		return this.GetEnumerator();
	}
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	private static AttributeCollection ReturnAndOut(out AttributeCollection collection, ReadOnlySpan<Attribute> values)
	{
		return collection = AttributeCollection.Create(values);
	}
	private static AttributeCollection ReturnAndOut(out AttributeCollection collection, IEnumerable<Attribute> attributes)
	{
		return collection = AttributeCollection.CreateFrom(attributes);
	}

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Attributes>k__BackingField")]
	private static extern ref Collection<Attribute>? GetAttributes(RuntimeDefinedParameter parameter);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_name")]
	private static extern ref string GetNameField(RuntimeDefinedParameter parameter);

	[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_parameterType")]
	private static extern ref Type? GetParameterTypeField(RuntimeDefinedParameter parameter);
}
