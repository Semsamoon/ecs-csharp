using System;
using System.Collections.Generic;

namespace SemsamECS
{
    /// <summary>
    /// Class of the pool container.<br/>
    /// <i>It is not recommended to manage components without using the container.</i>
    /// </summary>
    public sealed class Pools
    {
        private readonly Dictionary<Type, ISet> _sets;
        private int _id;

        /// <summary>
        /// Constructs a pool container with specified size for the internal pool array.
        /// </summary>
        public Pools(int size = 32)
        {
            _sets = new Dictionary<Type, ISet>(size);
        }

        /// <summary>
        /// Creates a pool with specified <see cref="SizeSet"/>.
        /// </summary>
        public Set<T> Create<T>(SizeSet size = default)
        {
            if (size.Sparse == 0)
                size = SizeSet.Default;
            var set = new Set<T>(size);
            _sets.Add(typeof(T), set);
            return set;
        }

        /// <summary>
        /// Checks if the pool for specified type placed in the sparse set.
        /// </summary>
        public bool Have<T>()
        {
            return _sets.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Returns the pool for specified type.
        /// </summary>
        public Set<T> Get<T>()
        {
            return (Set<T>)_sets[typeof(T)];
        }

        /// <summary>
        /// Returns the pool for specified type.
        /// </summary>
        public ISet Get(Type type)
        {
            return _sets[type];
        }
    }
}