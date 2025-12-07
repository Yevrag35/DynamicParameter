
namespace MG.Dynamic.Internal;

internal sealed class ParameterAttributeComparer : IComparer<ParameterAttribute>, IEqualityComparer<ParameterAttribute>
{
	public int Compare(ParameterAttribute? x, ParameterAttribute? y)
	{
		if (ReferenceEquals(x, y)) return 0;
		if (x is null) return -1;
		if (y is null) return 1;

		int comparison = x.ParameterSetName.CompareTo(y.ParameterSetName);
		if (comparison == 0)
		{
			comparison = x.Position.CompareTo(y.Position);
		}

		return comparison;
	}

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
