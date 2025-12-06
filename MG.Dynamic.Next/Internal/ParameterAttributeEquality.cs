
namespace MG.Dynamic.Internal;

internal sealed class ParameterAttributeEquality : IEqualityComparer<ParameterAttribute>
{
	public bool Equals(ParameterAttribute? x, ParameterAttribute? y)
	{
		if (ReferenceEquals(x, y)) return true;

		if (x is null || y is null) return false;

		return x.Position == y.Position
			&& x.ParameterSetName.Equals(y.ParameterSetName, StringComparison.Ordinal);
    }

	public int GetHashCode([DisallowNull] ParameterAttribute obj)
	{
		HashCode code = new();
		code.Add(obj.Position);
		code.Add(obj.ParameterSetName, StringComparer.Ordinal);
		return code.ToHashCode();
    }
}
