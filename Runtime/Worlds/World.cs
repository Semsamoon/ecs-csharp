using System;

namespace SemsamECS
{
    /// <summary>
    /// Class of the world.<br/>
    /// <i>It is not recommended to manage <see cref="Entities"/>, <see cref="Pools"/>, <see cref="Filters"/> and <see cref="Systems"/> without using the world.</i>
    /// </summary>
    public sealed class World
    {
        private readonly Sparse<int> _indices;
        private readonly DenseDense<Type> _components;

        /// <inheritdoc cref="SemsamECS.Entities"/>
        public Entities Entities { get; }

        /// <inheritdoc cref="SemsamECS.Pools"/>
        public Pools Pools { get; }

        /// <inheritdoc cref="SemsamECS.Filters"/>
        public Filters Filters { get; }

        /// <inheritdoc cref="SemsamECS.Systems"/>
        public Systems Systems { get; }

        /// <summary>
        /// Creates a world with specified sizes for internal elements.
        /// </summary>
        public World(
            SizeSet entitiesSize = default,
            SizeDenseDense componentsSize = default,
            int poolsSize = 32,
            int filtersSize = 32,
            SizeSystems systemsSize = default)
        {
            if (entitiesSize.Sparse == 0)
                entitiesSize = SizeSet.Default;
            _indices = new Sparse<int>(entitiesSize.Sparse);
            _components = new DenseDense<Type>(componentsSize);
            Entities = new Entities(entitiesSize);
            Pools = new Pools(poolsSize);
            Filters = new Filters(filtersSize);
            Systems = new Systems(systemsSize);
        }

        /// <inheritdoc cref="SemsamECS.Systems.Initialize"/>
        public void InitializeSystems()
        {
            Systems.Initialize(this);
        }

        /// <inheritdoc cref="SemsamECS.Entities.Create"/>
        public Entity CreateEntity()
        {
            var entity = Entities.Create();
            _indices.Add(_components.Count, entity.Id);
            _components.Add();
            return entity;
        }

        /// <summary>
        /// Registers the specified <i>empty</i> entity in the world. 
        /// </summary>
        /// <remarks>
        /// This method only registers the entity without components that has already been created in the world's entity container.
        /// It is possible to create an entity directly in the <see cref="Entities"/>, but for correct work with components it must be registered.
        /// </remarks>
        public void RegisterEntity(Entity entity)
        {
            _indices.Add(_components.Count, entity.Id);
            _components.Add();
        }

        /// <inheritdoc cref="SemsamECS.Entities.Remove"/>
        public void RemoveEntity(Entity entity)
        {
            ClearEntity(entity);
            _components.Remove(_indices[entity.Id]);
            _indices.Remove(entity.Id);
            Entities.Remove(entity);
        }

        /// <summary>
        /// Unregisters the specified <i>empty</i> entity from the world. 
        /// </summary>
        /// <remarks>
        /// This method only unregisters the entity without components that has already been removed in the world's entity container.
        /// It is possible to remove the entity directly in the <see cref="Entities"/>, but for correct work it must be unregistered.
        /// </remarks>
        public void UnregisterEntity(Entity entity)
        {
            _components.Remove(_indices[entity.Id]);
            _indices.Remove(entity.Id);
        }

        /// <summary>
        /// Adds the specified component to the specified entity in the world.
        /// </summary>
        public void AddComponent<T>(Entity entity, T value = default)
        {
            Pools.Get<T>().Add(entity, value);
            _components[_indices[entity.Id]].Add(typeof(T));
            Filters.Register<T>(entity);
        }

        /// <summary>
        /// Registers the specified component to the specified entity in the world. 
        /// </summary>
        /// <remarks>
        /// This method only registers the component to the entity that has already been created in the pool in the world's pool container.
        /// It is possible to create a component directly in the pool in the <see cref="Pools"/>, but for correct work with this component it must be registered.
        /// </remarks>
        public void RegisterComponent<T>(Entity entity)
        {
            _components[_indices[entity.Id]].Add(typeof(T));
            Filters.Register<T>(entity);
        }

        /// <summary>
        /// Removes the specified component from the specified entity in the world.
        /// </summary>
        public void RemoveComponent<T>(Entity entity)
        {
            Filters.Unregister<T>(entity);
            RemoveFromComponents(entity, typeof(T));
            Pools.Get<T>().Remove(entity);
        }

        /// <summary>
        /// Unregisters the specified component from the specified entity in the world. 
        /// </summary>
        /// <remarks>
        /// This method only unregisters the component from the entity that has already been removed in the pool in the world's pool container.
        /// It is possible to remove the component directly in the pool in the <see cref="Pools"/>, but for correct work it must be registered.
        /// </remarks>
        public void UnregisterComponent<T>(Entity entity)
        {
            Filters.Unregister<T>(entity);
            RemoveFromComponents(entity, typeof(T));
        }

        private void ClearEntity(Entity entity)
        {
            var index = _indices[entity.Id];
            foreach (var type in _components[index].AsSpan())
            {
                Filters.Unregister(type, entity);
                Pools.Get(type).Remove(entity);
            }
        }

        private void RemoveFromComponents(Entity entity, Type type)
        {
            var components = _components[_indices[entity.Id]];
            for (var i = 0; i < components.Count; i++)
                if (components[i] == type)
                {
                    components.Remove(i);
                    return;
                }
        }
    }
}