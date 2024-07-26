using System.Collections.Generic;
using UnityEngine;

public class FireBag : MonoBehaviour
{

    [SerializeField]
    private GameObject afterImagePrefab;

    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    public static FireBag Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GrowPool(100);
    }

    private void GrowPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        //print("ahhhhhhhhhhhhhhhhhhhhhh"); //These debug logs are wierd :P maybe write smth more descriptive??
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject GetFromPool()
    {
        if (availableObjects.Count == 0)
        {
            GrowPool(10);
        }

        GameObject instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
