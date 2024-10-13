using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField] private GameObject _afterImagePrefab;

    private Queue<AfterImage> _availableObject = new Queue<AfterImage>();

    public static AfterImagePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        GrowPool();
        DetachFromParent();
    }

    private void GrowPool()
    {
        for(int i=0;i <10;i++)
        {
            var instanceToAdd=Instantiate(_afterImagePrefab).GetComponent<AfterImage>();
            instanceToAdd.transform.SetParent(transform);  
            AddIntoPool(instanceToAdd);           
        }
    }

    public void AddIntoPool(AfterImage instance)
    {
        instance.Disable();
        _availableObject.Enqueue(instance);
    }

    public AfterImage GetFromPool(Transform target,SpriteRenderer renderer, Vector3 offset, Color color,
                                    bool customColor = false, bool useBlockMaterial = false)
    {
        if(_availableObject.Count == 0)
        {
            GrowPool();
        }


        var instance=_availableObject.Dequeue();

        instance.SetTargetTransform(target);
        instance.SetPositionOffset(offset);
        instance.SetTargetRenderer(renderer);
        instance.SetBlockMaterial(useBlockMaterial);
        instance.SetColor(customColor, color);

        instance.Enable();
        return instance;
    }

    public void DetachFromParent()
    {
        transform.parent = null;
    }

}
