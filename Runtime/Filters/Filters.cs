using System;
using System.Collections.Generic;

namespace SemsamECS
{
    /// <summary>
    /// Class of the filter container.<br/>
    /// <i>It is not recommended to manage filters without using the container.</i>
    /// </summary>
    public sealed class Filters
    {
        private readonly Dictionary<Type, FilterSet> _filterSets;

        /// <summary>
        /// Constructs a filter container with specified size for internal filter array.
        /// </summary>
        public Filters(int size = 32)
        {
            _filterSets = new Dictionary<Type, FilterSet>(size);
        }

        /// <summary>
        /// Creates a filter with specified include and exclude components and sizes for internal array and filter.
        /// </summary>
        public Filter Create(
            Span<Type> includes, Span<Type> excludes,
            SizeSet size = default, SizeFilterSet sizeFilter = default)
        {
            if (size.Sparse == 0)
                size = SizeSet.Default;
            if (sizeFilter.Exclude == 0)
                sizeFilter = SizeFilterSet.Default;
            var filter = new Filter(includes.Length, size);
            AddFilter(filter, includes, excludes, sizeFilter);
            return filter;
        }

        /// <summary>
        /// Registers the component of <typeparamref name="T"/> for specified entity in filters.
        /// </summary>
        public void Register<T>(Entity entity)
        {
            if (!_filterSets.TryGetValue(typeof(T), out var set))
                return;
            foreach (var filter in set.Includes.AsSpan())
                filter.Plus(entity);
            foreach (var filter in set.Excludes.AsSpan())
                filter.Minus(entity);
        }

        /// <summary>
        /// Unregisters the component of <typeparamref name="T"/> from specified entity in filters.
        /// </summary>
        public void Unregister<T>(Entity entity)
        {
            Unregister(typeof(T), entity);
        }

        /// <summary>
        /// Unregisters the component of specified type from specified entity in filters.
        /// </summary>
        public void Unregister(Type t, Entity entity)
        {
            if (!_filterSets.TryGetValue(t, out var set))
                return;
            foreach (var filter in set.Includes.AsSpan())
                filter.Minus(entity);
            foreach (var filter in set.Excludes.AsSpan())
                filter.Plus(entity);
        }

        private void AddFilter(Filter filter,
            Span<Type> includes, Span<Type> excludes,
            SizeFilterSet sizeFilter)
        {
            foreach (var type in includes)
                AddInclude(type, filter, sizeFilter);
            foreach (var type in excludes)
                AddExclude(type, filter, sizeFilter);
        }

        private void AddInclude(Type type, Filter filter, SizeFilterSet size)
        {
            if (_filterSets.TryGetValue(type, out var set))
                set.Includes.Add(filter);
            else
                CreateSet(type, size).Includes.Add(filter);
        }

        private void AddExclude(Type type, Filter filter, SizeFilterSet size)
        {
            if (_filterSets.TryGetValue(type, out var set))
                set.Excludes.Add(filter);
            else
                CreateSet(type, size).Excludes.Add(filter);
        }

        private FilterSet CreateSet(Type type, SizeFilterSet size)
        {
            var filtersIndices = new FilterSet(size);
            _filterSets.Add(type, filtersIndices);
            return filtersIndices;
        }
    }

    /// <summary>
    /// Struct of the filter set.<br/>
    /// It contains filters which include and exclude a necessary component.
    /// </summary>
    public readonly struct FilterSet
    {
        /// <summary>
        /// Filters that are include a necessary component.
        /// </summary>
        public Dense<Filter> Includes { get; }

        /// <summary>
        /// Filters that are exclude a necessary component.
        /// </summary>
        public Dense<Filter> Excludes { get; }

        /// <summary>
        /// Constructs a filter set with specified size for internal arrays.
        /// </summary>
        public FilterSet(SizeFilterSet size)
        {
            Includes = new Dense<Filter>(size.Include);
            Excludes = new Dense<Filter>(size.Exclude);
        }
    }

    /// <summary>
    /// Struct of the sizes for the filter set.<br/>
    /// It contains included and excluded sizes.
    /// </summary>
    public readonly struct SizeFilterSet
    {
        /// <summary>
        /// Size for the included filters array.
        /// </summary>
        public int Include { get; }

        /// <summary>
        /// Size for the excluded filters array.
        /// </summary>
        public int Exclude { get; }

        /// <summary>
        /// Default value for the size filter set.
        /// </summary>
        public static readonly SizeFilterSet Default = new(32, 32);

        /// <summary>
        /// Constructs a filter size set with specified
        /// <see cref="Include"/> and <see cref="Exclude"/> sizes.
        /// </summary>
        public SizeFilterSet(int include, int exclude)
        {
            Include = include;
            Exclude = exclude;
        }
    }
}