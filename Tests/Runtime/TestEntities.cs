using System;
using UnityEngine;

namespace SemsamECS.Tests
{
    public class TestEntities : MonoBehaviour
    {
        private void Start()
        {
            var entities = new Entities(new SizeSet(1, 1));

            for (var i = 0; i < 10; i++)
                if (entities.Create() != new Entity(i + 1, 0))
                    throw new Exception("Entities: Failed on creation");

            for (var i = 0; i < 10; i++)
                entities.Remove(new Entity(i + 1, 0));
            if (entities.Existing.Length != 0)
                throw new Exception("Entities: Failed on removing");

            for (var i = 0; i < 10; i++)
                if (entities.Create() == new Entity(10, i + 1))
                    entities.Remove(new Entity(10, i + 1));
                else
                    throw new Exception("Entities: Failed on recycling");

            for (var i = 0; i < 10; i++)
                if (entities.Create().Id != 10 - i)
                    throw new Exception("Entities: Failed on order");

            for (var i = 0; i < 10; i++)
                if (!entities.Have(entities.Create()))
                    throw new Exception("Entities: Failed on checking");
            if (entities.Have(new Entity(10, 1)))
                throw new Exception("Entities: Failed on checking");

            Debug.Log("Entities: OK");
        }
    }
}