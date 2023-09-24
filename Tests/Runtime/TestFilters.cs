using System;
using UnityEngine;

namespace SemsamECS.Tests
{
    public class TestFilters : MonoBehaviour
    {
        private void Start()
        {
            var filters = new Filters(1);
            
            var filterA = filters.Create(
                new[] { typeof(A) }, 
                Span<Type>.Empty,
                new SizeSet(1, 1), 
                new SizeFilterSet(1, 1));
            var filterB = filters.Create(
                new[] { typeof(B) }, 
                Span<Type>.Empty,
                new SizeSet(1, 1), 
                new SizeFilterSet(1, 1));
            var filterAB = filters.Create(
                new[] { typeof(A), typeof(B) }, 
                Span<Type>.Empty,
                new SizeSet(1, 1), 
                new SizeFilterSet(1, 1));
            var filterA_B = filters.Create(
                new[] { typeof(A) }, 
                new[] { typeof(B) },
                new SizeSet(1, 1), 
                new SizeFilterSet(1, 1));

            if (filterA == null || filterB == null || filterAB == null || filterA_B == null)
                throw new Exception("Filters: Failed on creation");

            var entity1 = new Entity(1, 0);
            filters.Register<A>(entity1);

            var entity2 = new Entity(2, 0);
            filters.Register<B>(entity2);
            
            var entity3 = new Entity(3, 0);
            filters.Register<A>(entity3);
            filters.Register<B>(entity3);

            if (filterA.Entities.Length != 2 || filterA.Entities[0] != entity1 || filterA.Entities[1] != entity3)
                throw new Exception("Filters: Failed adding to A filter");
            if (filterB.Entities.Length != 2 || filterB.Entities[0] != entity2 || filterB.Entities[1] != entity3)
                throw new Exception("Filters: Failed adding to B filter");
            if (filterAB.Entities.Length != 1 || filterAB.Entities[0] != entity3)
                throw new Exception("Filters: Failed adding to AB filter");
            if (filterA_B.Entities.Length != 1 || filterA_B.Entities[0] != entity1)
                throw new Exception("Filters: Failed adding to A_B filter");
            
            filters.Unregister(typeof(B), entity3);

            if (filterA.Entities.Length != 2)
                throw new Exception("Filters: Failed removing from A filter");
            if (filterB.Entities.Length != 1)
                throw new Exception("Filters: Failed removing from B filter");
            if (filterAB.Entities.Length != 0)
                throw new Exception("Filters: Failed removing from AB filter");
            if (filterA_B.Entities.Length != 2 || filterA_B.Entities[1] != entity3)
                throw new Exception("Filters: Failed removing from A_B filter");
            
            filters.Register<B>(entity3);
            
            if (filterA.Entities.Length != 2 || filterA.Entities[1] != entity3)
                throw new Exception("Filters: Failed repeating adding to A filter");
            if (filterB.Entities.Length != 2 || filterB.Entities[1] != entity3)
                throw new Exception("Filters: Failed repeating adding to B filter");
            if (filterAB.Entities.Length != 1 || filterAB.Entities[0] != entity3)
                throw new Exception("Filters: Failed repeating adding to AB filter");
            if (filterA_B.Entities.Length != 1)
                throw new Exception("Filters: Failed repeating adding to A_B filter");
            
            Debug.Log("Filters: OK");
        }

        private struct A
        {
            public int Value;
        }

        private struct B
        {
        }
    }
}