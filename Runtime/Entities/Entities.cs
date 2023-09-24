using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the <see cref="Entity"/> container.<br/>
    /// <i>It is not recommended to manage entities without using the container.</i>
    /// </summary>
    public sealed class Entities
    {
        private readonly Set _set;
        private int _removed;
        private int _id;

        /// <summary>
        /// Array of the existing entities.
        /// </summary>
        public ReadOnlySpan<Entity> Existing => _set.Dense[1..];

        /// <summary>
        /// Constructs an entity container with specified <see cref="SizeSet"/> for the <see cref="Set"/>.
        /// </summary>
        public Entities(SizeSet size = default)
        {
            if (size.Sparse == 0)
                size = SizeSet.Default;
            _set = new Set(size);
            _removed = 0;
            _id = 1;
        }

        /// <summary>
        /// Creates a new <see cref="Entity"/>.
        /// </summary>
        public Entity Create()
        {
            if (_removed == 0)
                return CreateNew();
            _removed--;
            return Recycle();
        }

        /// <summary>
        /// Removes the specified <see cref="Entity"/>.
        /// </summary>
        public void Remove(Entity entity)
        {
            _removed++;
            _set.Remove(entity);
        }

        /// <summary>
        /// Checks if the specified <see cref="Entity"/> exists.
        /// </summary>
        public bool Have(Entity entity)
        {
            return _set.Have(entity);
        }

        private Entity CreateNew()
        {
            var entity = new Entity(_id++, 0);
            _set.Add(entity);
            return entity;
        }

        private Entity Recycle()
        {
            var recycle = _set.RawDense[_set.Dense.Length];
            var entity = recycle.Gen == int.MaxValue
                ? new Entity(_id++, 0)
                : new Entity(recycle.Id, recycle.Gen + 1);
            _set.Add(entity);
            return entity;
        }
    }
}