using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the sparse set.<br/>
    /// It contains <see cref="Sparse"/> and <see cref="Dense"/> arrays.
    /// </summary>
    public sealed class Set : ISet
    {
        private readonly Sparse<int> _sparse;
        private readonly Dense<Entity> _dense;

        /// <summary>
        /// Index for the sparse array is an entity id. Sparse array holds indices for the dense array.
        /// </summary>
        public ReadOnlySpan<int> Sparse => _sparse.AsSpan();

        /// <summary>
        /// Dense array holds entities contiguously.
        /// </summary>
        public ReadOnlySpan<Entity> Dense => _dense.AsSpan();

        /// <summary>
        /// <see cref="Dense"/> array with unused part.
        /// </summary>
        public ReadOnlySpan<Entity> RawDense => _dense.AsRawSpan();

        /// <summary>
        /// Constructs a sparse set with specified <see cref="SizeSet"/>.
        /// </summary>
        public Set(SizeSet size = default)
        {
            if (size.Sparse == 0)
                size = SizeSet.Default;
            _sparse = new Sparse<int>(size.Sparse);
            _dense = new Dense<Entity>(size.Dense);
            _dense.Add(new Entity());
        }

        /// <summary>
        /// Adds the specified <see cref="Entity"/>.
        /// </summary>
        public void Add(Entity entity)
        {
            _sparse.Add(_dense.Count, entity.Id);
            _dense.Add(entity);
        }

        /// <summary>
        /// Removes the specified <see cref="Entity"/>.
        /// </summary>
        public void Remove(Entity entity)
        {
            var index = _sparse[entity.Id];
            var last = _dense[^1];
            _dense.Remove(index);
            _sparse[last.Id] = index;
            _sparse[entity.Id] = 0;
        }

        /// <summary>
        /// Checks if the specified <see cref="Entity"/> placed in the sparse set. 
        /// </summary>
        public bool Have(Entity entity)
        {
            var id = entity.Id;
            if (_sparse.Size <= id)
                return false;
            return _dense[_sparse[entity.Id]] == entity;
        }
    }

    /// <summary>
    /// Class of the sparse set with additional items.<br/>
    /// It contains <see cref="Sparse"/>, <see cref="Dense"/> and <see cref="_denseItems"/> arrays.
    /// </summary>
    public sealed class Set<T> : ISet
    {
        private readonly Sparse<int> _sparse;
        private readonly Dense<Entity> _dense;
        private readonly Dense<T> _denseItems;

        /// <summary>
        /// Index for the sparse array is an entity id. Sparse array holds indices for the dense array.
        /// </summary>
        public ReadOnlySpan<int> Sparse => _sparse.AsSpan();

        /// <summary>
        /// Dense array holds entities contiguously.
        /// </summary>
        public ReadOnlySpan<Entity> Dense => _dense.AsSpan();

        /// <summary>
        /// <see cref="Dense"/> array with unused part.
        /// </summary>
        public ReadOnlySpan<Entity> RawDense => _dense.AsRawSpan();

        /// <summary>
        /// Dense item array holds items contiguously.
        /// </summary>
        public ReadOnlySpan<T> DenseItems => _denseItems.AsSpan();

        /// <summary>
        /// <see cref="DenseItems"/> array with unused part.
        /// </summary>
        public ReadOnlySpan<T> RawDenseItems => _denseItems.AsRawSpan();

        /// <summary>
        /// Constructs a sparse set with specified <see cref="SizeSet"/>.
        /// </summary>
        public Set(SizeSet size = default)
        {
            if (size.Sparse == 0)
                size = SizeSet.Default;
            _sparse = new Sparse<int>(size.Sparse);
            _dense = new Dense<Entity>(size.Dense);
            _denseItems = new Dense<T>(size.Dense);
            _dense.Add(new Entity());
            _denseItems.Add(default);
        }

        /// <summary>
        /// Adds the specified <see cref="Entity"/> with their items of <typeparamref name="T"/>.
        /// </summary>
        public void Add(Entity entity, T item)
        {
            _sparse.Add(_dense.Count, entity.Id);
            _dense.Add(entity);
            _denseItems.Add(item);
        }

        /// <summary>
        /// Adds the specified <see cref="Entity"/> with their items of <typeparamref name="T"/>.
        /// </summary>
        public void Add(Entity entity)
        {
            _sparse.Add(_dense.Count, entity.Id);
            _dense.Add(entity);
            _denseItems.Add(default);
        }

        /// <summary>
        /// Removes the specified <see cref="Entity"/> with its item of <typeparamref name="T"/>.
        /// </summary>
        public void Remove(Entity entity)
        {
            var index = _sparse[entity.Id];
            var last = _dense[^1];
            _dense.Remove(index);
            _denseItems.Remove(index);
            _sparse[last.Id] = index;
            _sparse[entity.Id] = 0;
        }

        /// <summary>
        /// Checks if the specified <see cref="Entity"/> with its item placed in the sparse set. 
        /// </summary>
        public bool Have(Entity entity)
        {
            var id = entity.Id;
            if (_sparse.Size <= id)
                return false;
            return _dense[_sparse[id]] == entity;
        }

        /// <summary>
        /// Returns an item of the specified entity.
        /// </summary>
        public ref T Get(Entity entity)
        {
            return ref _denseItems[_sparse[entity.Id]];
        }
    }

    /// <summary>
    /// Interface of the sparse set.
    /// </summary>
    public interface ISet
    {
        /// <summary>
        /// Adds the specified <see cref="Entity"/>.
        /// </summary>
        void Add(Entity entity);

        /// <summary>
        /// Removes the specified <see cref="Entity"/>.
        /// </summary>
        void Remove(Entity entity);

        /// <summary>
        /// Checks if the specified <see cref="Entity"/> placed in the sparse set. 
        /// </summary>
        bool Have(Entity entity);
    }

    /// <summary>
    /// Struct of the sizes for the sparse set.<br/>
    /// It contains sparse and dense sizes.
    /// </summary>
    public readonly struct SizeSet
    {
        /// <summary>
        /// Size for the sparse array.
        /// </summary>
        public int Sparse { get; }

        /// <summary>
        /// Size for the dense array.
        /// </summary>
        public int Dense { get; }

        /// <summary>
        /// Default value for the size set.
        /// </summary>
        public static readonly SizeSet Default = new(32, 32);

        /// <summary>
        /// Constructs a size set with specified <see cref="Sparse"/> and <see cref="Dense"/> sizes.
        /// </summary>
        public SizeSet(int sparse, int dense)
        {
            Sparse = sparse;
            Dense = dense;
        }
    }
}