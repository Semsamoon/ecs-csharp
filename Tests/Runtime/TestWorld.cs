using System;
using UnityEngine;

namespace SemsamECS.Tests
{
    public class TestWorld : MonoBehaviour
    {
        private void Start()
        {
            var world = new World();
            var entities = world.Entities;
            var pools = world.Pools;
            var filters = world.Filters;

            if (entities == null || pools == null || filters == null)
                throw new Exception("World: Failed on creation");

            var entity = world.CreateEntity();
            if (!entities.Have(entity))
                throw new Exception("World: Failed on creation entity");

            pools.Create<int>(new SizeSet(1, 1));
            var poolInt = pools.Get<int>();

            var filterInt = filters.Create(
                new[] { typeof(int) },
                Span<Type>.Empty,
                new SizeSet(1, 1),
                new SizeFilterSet(1, 1));

            world.AddComponent(entity, 3);
            if (!poolInt.Have(entity) 
                || poolInt.Get(entity) != 3
                || filterInt.Entities.Length != 1 
                || filterInt.Entities[0] != entity)
                throw new Exception("World: Failed on adding component");

            world.RemoveComponent<int>(entity);
            if (poolInt.Have(entity) 
                || filterInt.Entities.Length != 0)
                throw new Exception("World: Failed on removing component");
            
            poolInt.Add(entity, 5);
            world.RegisterComponent<int>(entity);
            if (filterInt.Entities.Length != 1 
                || filterInt.Entities[0] != entity)
                throw new Exception("World: Failed on registering component");
            
            poolInt.Remove(entity);
            world.UnregisterComponent<int>(entity);
            if (filterInt.Entities.Length != 0)
                throw new Exception("World: Failed on unregistering component");

            world.AddComponent(entity, 3);
            world.RemoveEntity(entity);
            if (entities.Have(entity)
            || poolInt.Have(entity) 
                || filterInt.Entities.Length != 0)
                throw new Exception("World: Failed on removing entity");

            entity = entities.Create();
            world.RegisterEntity(entity);
            world.AddComponent(entity, 6);
            if (!poolInt.Have(entity) 
                || poolInt.Get(entity) != 6
                || filterInt.Entities.Length != 1 
                || filterInt.Entities[0] != entity)
                throw new Exception("World: Failed on adding component after repeating creation entity");
            
            world.RemoveComponent<int>(entity);
            entities.Remove(entity);
            world.UnregisterEntity(entity);
            
            Debug.Log("World: OK");
        }
    }
}