using Godot;
using System.Collections.Generic;

namespace Guide {
	/// Helper class for modelling sets
	public partial class GUIDESet : GodotObject {
		private readonly Dictionary<Variant, Variant> _values = new();

		/// Adds the given value to the set.
		/// If the value is already in the set, it will not be added again.
		public void Add( Variant value ) {
			_values[ value ] = value;
		}

		/// Adds all values in the given array to the set.
		/// If a value is already in the set, it will not be added again.
		public void AddAll( List<Variant> values ) {
			foreach ( var value in values )
				_values[ value ] = value;
		}

		/// Removes the given value from the set.
		public void Remove( Variant value ) {
			_values.Remove( value );
		}

		/// Removes all values from the set.
		public void Clear() {
			_values.Clear();
		}

		/// Returns true if the set is empty, false otherwise.
		public bool IsEmpty() {
			return _values.Count == 0;
		}

		/// Returns a new set containing only the values for which the given predicate returns true.
		/// The predicate should take a single argument and return a boolean.
		public GUIDESet Filter( Callable predicate ) {
			var result = new GUIDESet();
			foreach ( var key in _values.Keys ) {
				if ( (bool)predicate.Call( key ) )
					result.Add( key );
			}
			return result;
		}

		/// Returns the first item in the set and removes it from the set.
		/// If the set is empty, returns null.
		public Variant Pull() {
			if ( IsEmpty() )
				return default;

			var key = _values.Keys.GetEnumerator();
			key.MoveNext();
			var value = key.Current;
			Remove( value );
			return value;
		}

		/// Checks whether the set contains the given value.
		public bool Has( Variant value ) {
			return _values.ContainsKey( value );
		}

		/// Returns the first item for which the given matcher function returns
		/// a true value.
		public Variant FirstMatch( Callable matcher ) {
			foreach ( var key in _values.Keys ) {
				if ( (bool)matcher.Call( key ) )
					return key;
			}
			return default;
		}

		/// Assigns all values in the set to the given array.
		public void AssignTo( Godot.Collections.Array values ) {
			values = [ .. _values.Keys ];
		}

		/// Returns an array of all values in the set.
		public List<Variant> Values() {
			return [ .. _values.Keys ];
		}

		/// Returns the number of items in the set.
		public int Size() {
			return _values.Count;
		}
	}
};