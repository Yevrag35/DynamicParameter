namespace MG.Dynamic.Collections;

public sealed partial class AttributeCollection
{
	/// <summary>
	/// Returns an enumerator that iterates through the collection.
	/// </summary>
	/// <returns>An <see cref="Enumerator"/> that can be used to iterate through the collection.</returns>
	[DebuggerStepThrough]
	public new Enumerator GetEnumerator()
	{
		return new(this);
	}
	/// <inheritdoc/>
	[DebuggerStepThrough]
	IEnumerator<Attribute> IEnumerable<Attribute>.GetEnumerator()
	{
		return this.GetEnumerator();
	}
	/// <inheritdoc/>
	[DebuggerStepThrough]
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Enumerator : IEnumerator<Attribute>
	{
		private List<Attribute> _list;
		private Attribute _current;
		private int _index;
		private int _version;

		public readonly Attribute Current => _current;
		readonly object? IEnumerator.Current => _current;

		internal Enumerator(AttributeCollection collection)
		{
			_version = GetVersion(collection._allAttributes);
			_list = collection._allAttributes;
			_index = -1;
			_current = default!;
		}

		public bool MoveNext()
		{
			int next = _index + 1;
			if ((uint)next < (uint)_list.Count)
			{
				if (IsVersionMismatch(_version, _list))
				{
					ThrowModified();
				}

				_current = _list[next];
				_index = next;
				return true;
			}

			_index = _list.Count;
			_current = default!;
			return false;
		}
		void IDisposable.Dispose()
		{
			this = default;
		}
		void IEnumerator.Reset()
		{
			_index = -1;
			_current = default!;
		}

		private static int GetVersion(List<Attribute> list)
		{
			ListView view = System.Runtime.CompilerServices.Unsafe.As<ListView>(list);
			return view._version;
		}
		private static bool IsVersionMismatch(int version, List<Attribute> list)
		{
			return version != GetVersion(list);
		}
		private static void ThrowModified()
		{
			throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
		}
	}
}
