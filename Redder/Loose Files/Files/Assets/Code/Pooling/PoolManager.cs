using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

    static PoolManager _instance;

    public static PoolManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PoolManager>();
            }
            return _instance;
        }
    }

	public void CreatePool(GameObject prefab, int poolSize) //Function to create new pools.
    {
        int poolKey = prefab.GetInstanceID(); //Get instance ID from object I want to pool and use it as key for the pool, so I can find it easily.

        if(!poolDictionary.ContainsKey(poolKey)) //Check if there isn't already a pool for the object.
        {
            GameObject poolHolder = new GameObject(prefab.name + " (Pool)"); //Create parent object to place pooled objects under, so the hierarchy doesn't get messy.

            poolHolder.transform.parent = transform; //Set Poolmanager's object as parent for the pool's parent.

            poolDictionary.Add(poolKey, new Queue<ObjectInstance>()); //Create a new pool in the pool dictionary.

            for(int i = 0; i < poolSize; i++) //Repeat for [poolSize] times.
            {
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject); //Instantiate a new object to add to the pool.
                poolDictionary[poolKey].Enqueue(newObject); //Enque the instantiated object.
                newObject.SetParent(poolHolder.transform); //Set the parent for the instantiated object.
            }
        }
        else
        {
            Debug.LogWarning("Error: The pool you tried to create already exists. Type: " + prefab.name); //Debug error if pool already exist.
        }
    }

    public void ExpandPool(GameObject prefab, int poolSize) //Function to expand already existing pools.
    {
        int poolKey = prefab.GetInstanceID(); //Get poolkey from the object.

        if (poolDictionary.ContainsKey(poolKey)) //Check if the dictionary has a pool for the requested object.
        {
            if (poolDictionary[poolKey].Count < poolSize) //Check if the pool isn't already bigger than the requested size. 
            {
                GameObject poolHolder = GameObject.Find(prefab.name + " (Pool)"); //Find the poolholder for the requested pool.

                int extraPoolSize = poolSize - poolDictionary[poolKey].Count; //Get the difference between the requested size and the current size.

                for (int i = 0; i < extraPoolSize; i++) //Repeat for [extraPoolSize] times.
                {
                    ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject); //Instantiate a new object to add to the pool.
                    poolDictionary[poolKey].Enqueue(newObject); //Enque the instantiated object.
                    newObject.SetParent(poolHolder.transform); //Set the parent for the instantiated object.
                }
            }
        }
        else
        {
            Debug.LogError("Error: The pool you tried to expand does not exist. Type: " + prefab.name); //Debug error if the pool doesn't exist.
        }
    }

    public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation, bool requiredInActive) //Function that assigns an object to reuse from its pool. This way I dont have to instatiate any objects in runtime.
    {
        int poolKey = prefab.GetInstanceID(); //Get poolkey from the object.
        if (poolDictionary.ContainsKey(poolKey)) //Check if there is a pool for the requested object.
        {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue(); //Dequeue the first object in the pool's queue.
            poolDictionary[poolKey].Enqueue(objectToReuse); //Requeue the recently dequeued object so it's at the back of the queue.

            if (requiredInActive) //Check if the function that called ReuseObject() needs an inactive object. For example this could be used for planes. An airship cannot use an already used plane. 
            {
                for (int i = 1; i < poolDictionary[poolKey].Count; i++) //Cycle through all objects in the object's queue.
                {
                    if (!objectToReuse.gameObject.activeSelf) //Check if current cycle's object is inactive.
                    {
                        break; //Break if true.
                    }
                    objectToReuse = poolDictionary[poolKey].Dequeue(); //Dequeue the next object in the pool's queue.
                    poolDictionary[poolKey].Enqueue(objectToReuse); //Requeue the recently dequeued object so it's at the back of the queue.
                }
            }

            objectToReuse.Reuse(position, rotation); //Reuse the object. Assign it's new location and rotation.
        }
        else
        {
            Debug.LogError("Error: The pool you tried to access does not exist. Type: " + prefab.name); //Debug an error if the requested object to reuse doesn't have a pool.
        }
    }

    public void ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation, bool requiredInActive, GameObject obj) //Function that assigns an object to reuse from its pool with an extra overload. This way I dont have to instatiate any objects in runtime.
    {
        int poolKey = prefab.GetInstanceID(); //Get poolkey from the object.
        if (poolDictionary.ContainsKey(poolKey)) //Check if there is a pool for the requested object.
        {
            ObjectInstance objectToReuse = poolDictionary[poolKey].Dequeue(); //Dequeue the first object in the pool's queue.
            poolDictionary[poolKey].Enqueue(objectToReuse); //Requeue the recently dequeued object so it's at the back of the queue.

            if (requiredInActive) //Check if the function that called ReuseObject() needs an inactive object. For example this could be used for planes. An airship cannot use an already used plane. 
            {
                for (int i = 1; i < poolDictionary[poolKey].Count; i++) //Cycle through all objects in the object's queue.
                {
                    if (!objectToReuse.gameObject.activeSelf) //Check if current cycle's object is inactive.
                    {
                        break; //Break if true.
                    }
                    objectToReuse = poolDictionary[poolKey].Dequeue(); //Dequeue the next object in the pool's queue.
                    poolDictionary[poolKey].Enqueue(objectToReuse); //Requeue the recently dequeued object so it's at the back of the queue.
                }
            }

            objectToReuse.Reuse(position, rotation, obj); //Reuse the object. Assign it's new location and rotation.
        }
        else
        {
            Debug.LogError("Error: The pool you tried to access does not exist. Type: " + prefab.name); //Debug an error if the requested object to reuse doesn't have a pool.
        }
    }

    public class ObjectInstance
    {
        public GameObject gameObject;
        public Transform transform;

        bool hasPoolObjectComponent;
        PoolObject poolObjectScript;

        public ObjectInstance(GameObject objectInstance) //Constructor for the class
        {
            gameObject = objectInstance;
            transform = gameObject.transform;

            gameObject.SetActive(false);

            if(gameObject.GetComponent<PoolObject>()) //Check if the object is a poolObject
            {
                hasPoolObjectComponent = true;
                poolObjectScript = gameObject.GetComponent<PoolObject>(); //Assigns the object's PoolObject script.
            }
        }

        public void Reuse(Vector3 position, Quaternion rotation) //Function that assigns the new position and rotation of the object.
        {
            gameObject.SetActive(true); //Reactivates the object.
            transform.position = position; //Applies the new position.
            transform.rotation = rotation; //Applies the new rotation.

            if (hasPoolObjectComponent) //Check if the object is a poolObject.
            {
                poolObjectScript.OnObjectReuse(); //Update the poolObject's OnObjectReuse() function.
            }
        }

        public void Reuse(Vector3 position, Quaternion rotation, GameObject obj) //Function that assigns the new position and rotation of the object with an extra overload.
        {
            gameObject.SetActive(true); //Reactivates the object.
            transform.position = position; //Applies the new position.
            transform.rotation = rotation; //Applies the new rotation.

            if (hasPoolObjectComponent) //Check if the object is a poolObject.
            {
                poolObjectScript.OnObjectReuse(obj); //Update the poolObject's OnObjectReuse() function.
            }
        }

        public void SetParent(Transform parent) 
        {
            transform.parent = parent; //Sets the object's parent.
        }
    }
}
