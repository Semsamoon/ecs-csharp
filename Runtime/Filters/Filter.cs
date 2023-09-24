using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the filter.<br/>
    /// It stores entities that match requirement of including and excluding components.
    /// </summary>
    public sealed class Filter
    {
        private readonly int _target;
        private readonly Set _matches;
        private readonly Sparse<int> _sums;

        /// <summary>
        /// Array of the match entities.
        /// </summary>
        public ReadOnlySpan<Entity> Entities => _matches.Dense[1..];

        /// <summary>
        /// Construct a filter with specified amount of included components and <see cref="SizeSet"/> for the entity array.
        /// </summary>
        public Filter(int target, SizeSet size = default)
        {
            if (size.Sparse == 0)
                size = SizeSet.Default;
            _target = target;
            _matches = new Set(size);
            _sums = new Sparse<int>(size.Sparse);
        }

        /// <summary>
        /// Indicates that specified entity became more fitting for requirements.
        /// </summary>
        public void Plus(Entity entity)
        {
            if (_sums.Size <= entity.Id)
                Add(entity, 1);
            else
                Increment(entity);
        }

        /// <summary>
        /// Indicates that specified entity became less fitting for requirements.
        /// </summary>
        /// <param name="entity"></param>
        public void Minus(Entity entity)
        {
            if (_sums.Size <= entity.Id)
                Add(entity, -1);
            else
                Decrement(entity);
        }

        private void Add(Entity entity, int initialSum)
        {
            _sums.Add(initialSum, entity.Id);
            if (_target == initialSum)
                _matches.Add(entity);
        }

        private void Increment(Entity entity)
        {
            _sums[entity.Id]++;
            if (_sums[entity.Id] == _target)
                _matches.Add(entity);
        }

        private void Decrement(Entity entity)
        {
            if (_sums[entity.Id] == _target)
                _matches.Remove(entity);
            _sums[entity.Id]--;
        }
    }
}