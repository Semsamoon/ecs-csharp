using System;
using UnityEngine;

namespace SemsamECS.Tests
{
    public sealed class TestSystems : MonoBehaviour
    {
        private void Start()
        {
            var world = new World();
            var systems = world.Systems;
            systems.Add<A>();
            world.InitializeSystems();

            var pool = world.Pools.Get<int>();
            if (pool.Dense.Length != 3 || pool.DenseItems[1] != 1 || pool.DenseItems[2] != 1)
                throw new Exception("Systems: Failed initializing systems");

            systems.Start();
            if (pool.Dense.Length != 3 || pool.DenseItems[1] != 2 || pool.DenseItems[2] != 2)
                throw new Exception("Systems: Failed starting systems");

            systems.Update();
            if (pool.Dense.Length != 3 || pool.DenseItems[1] != 3 || pool.DenseItems[2] != 3)
                throw new Exception("Systems: Failed updating systems");

            Debug.Log("Systems: OK");
        }

        private sealed class A : IInitializeAble, IStartAble, IUpdateAble
        {
            private Set<int> _pool;
            private Filter _filter;

            public void Initialize(World world)
            {
                _pool = world.Pools.Create<int>(new SizeSet(1, 1));
                _filter = world.Filters.Create(
                    new[] { typeof(int) },
                    Span<Type>.Empty,
                    new SizeSet(1, 1),
                    new SizeFilterSet(1, 1));
                world.AddComponent(world.CreateEntity(), 1);
                world.AddComponent(world.CreateEntity(), 1);
            }

            public void Start()
            {
                foreach (var entity in _filter.Entities)
                    _pool.Get(entity)++;
            }

            public void Update()
            {
                foreach (var entity in _filter.Entities)
                    _pool.Get(entity)++;
            }
        }
    }
}