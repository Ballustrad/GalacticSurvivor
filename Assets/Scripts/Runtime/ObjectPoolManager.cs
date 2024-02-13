using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalacticSurvivor.Game.Runtime
{
    public class ObjectPoolManager : MonoBehaviour
    {
        /// <summary>
        /// Pour utiliser et summon les objets , remplacez les instantiate dans les scripts concernés par :
        /// ObjectPoolManager.SpawnObject(l'objet, la position, la rotation
        /// </summary>
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
        {
            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);

            if (pool == null)
            {
                pool = new PooledObjectInfo()
                {
                    LookupString = objectToSpawn.name,

                };
                ObjectPools.Add(pool);
            }

            GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (spawnableObj == null)
            {
                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            }
            else
            {
                spawnableObj.transform.position = spawnPosition;
                spawnableObj.transform.rotation = spawnRotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }

            return spawnableObj;

        }

        public static void ReturnedObjectToPool(GameObject obj)
        {
            string goName = obj.name.Substring(0, obj.name.Length - 7); //pour ignorer le Clone

            PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);

            if (pool == null)
            {
                Debug.LogWarning("Tryng to release an object that hasnt been pooled" + obj.name);
            }
            else
            {
                obj.SetActive(false);
                pool.InactiveObjects.Add(obj);
            }
        }
    }

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();

    }
}
