using System;
using UnityEngine;

namespace SemsamECS.Tests
{
    public class TestPools : MonoBehaviour
    {
        private void Start()
        {
            var pools = new Pools(1);

            pools.Create<int>(new SizeSet(1, 1));
            pools.Create<char>(new SizeSet(1, 1));

            if (!pools.Have<int>() || !pools.Have<char>() || pools.Have<byte>())
                throw new Exception("Pools: Failed on checking");
            
            var poolInt = pools.Get<int>();
            var poolChar = pools.Get<char>();

            if (poolInt == null || poolChar == null)
                throw new Exception("Pools: Failed on getting");

            var entity1 = new Entity(1, 0);
            var entity2 = new Entity(2, 0);
            poolInt.Add(entity1, 3);
            poolInt.Add(entity2, 5);
            if (!poolInt.Have(entity1) || !poolInt.Have(entity2))
                throw new Exception("Pools: Failed on checking");
            
            if (poolInt.Get(entity1) != 3 || poolInt.Get(entity2) != 5)
                throw new Exception("Pools: Failed on adding");

            poolInt.Get(entity1) = 4;
            if (poolInt.Get(entity1) != 4)
                throw new Exception("Pools: Failed on reassigning");
            
            poolInt.Remove(entity1);
            if (poolInt.Have(entity1) || !poolInt.Have(entity2))
                throw new Exception("Pools: Failed on removing");
            poolInt.Remove(entity2);
            if (poolInt.Have(entity2))
                throw new Exception("Pools: Failed on removing");
                
            Debug.Log("Pools: OK");
        }
    }
}
