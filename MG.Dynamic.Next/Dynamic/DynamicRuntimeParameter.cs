using MG.Dynamic.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MG.Dynamic;

public sealed class DynamicRuntimeParameter : RuntimeDefinedParameter
{
    private readonly AttributeCollection _attributes;

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
    /// Gets the parameter attribute that identifies this parameter to cmdlets.
    /// </summary>
    public ParameterAttribute ParameterAttribute => _attributes.Parameter;
    /// <inheritdoc cref="RuntimeDefinedParameter.ParameterType" path="/*[not(self::exception)]"/>
    public new Type? ParameterType
    {
        get => GetParameterTypeField(this);
        set => GetParameterTypeField(this) = value;
    }

    public DynamicRuntimeParameter(params ReadOnlySpan<Attribute> attributes) : base()
    {
        _attributes = AttributeCollection.Create(attributes);
        GetAttributes(this) = _attributes;
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
