using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalacticSurvivor.Game.Runtime
{
    public class ObjectPoolManager : MonoBehaviour
    {
        /// <summary>
        /// Pour utiliser et summon les objets , remplacez les instantiate dans les scripts concernés par :
        /// ObjectPoolManager.SpawnObject(l'objet, la position, la rotation, et le pooltype : ObjectManager.PoolType.Type1)
        /// </summary>
        public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

        private GameObject _objectPoolEmptyHolder;
        private static GameObject _type1Empty;
        private static GameObject _type2Empty;
        public enum PoolType
        {
            Type1,
            Type2,
            None
        }
        public static PoolType PoolingType;

        private void Awake()
        {
            SetUpEmpties();
        }

        private void SetUpEmpties()
        {
            _objectPoolEmptyHolder = new GameObject("Pooled Objects");

            _type1Empty = new GameObject("Type 1");
            _type1Empty.transform.SetParent(_objectPoolEmptyHolder.transform);

            _type2Empty = new GameObject("Type 2");
            _type2Empty.transform.SetParent(_objectPoolEmptyHolder.transform);
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
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
                GameObject parentObject = SetParentObject(poolType);

                spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

                if (parentObject != null)
                {
                    spawnableObj.transform.SetParent(parentObject.transform);
                }
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
        /// <summary>
        /// en dessous c'est la meme chose que l'autre avec moins de parametres, un overload
        /// </summary>
        /// <param name="objectToSpawn"></param>
        /// <param name="parentTransform"></param>
        /// <returns></returns>
        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform)
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


                spawnableObj = Instantiate(objectToSpawn, parentTransform);


            }
            else
            {

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

        private static GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.Type1:
                    return _type1Empty;
                case PoolType.Type2:
                    return _type2Empty;
                case PoolType.None:
                    return null;
                default:
                    return null;
            }
        }
    }

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();

    }
}
