using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour
{
    [System.Serializable]
    public class DroppedGameObject
    {
        [SerializeField] private GameObject droppedItemType;
        [SerializeField] private int randomObjectAmountMin, randomObjectAmountMax;

        public GameObject GetGameObject() { return droppedItemType; }
        public int GetRandom() { return UnityEngine.Random.Range(randomObjectAmountMin, randomObjectAmountMax); }
    }


    [SerializeField] private List<DroppedGameObject> _droppedItems = new List<DroppedGameObject>();
    [SerializeField] private float _dropRadius;


    private void OnDestroy()
    {
        Vector3 dropPosition = transform.position;
        float theta;

        for (int i = 0; i < _droppedItems.Count; i++)
        {
            for (int j = 0; j < _droppedItems[i].GetRandom(); j++)
            {
                //random positioning
                theta = UnityEngine.Random.Range(0f, 10f) * 2 * Mathf.PI;
                dropPosition.y = transform.position.y + _dropRadius * Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * Mathf.Cos(theta);
                dropPosition.x = transform.position.x + _dropRadius * Mathf.Sqrt(UnityEngine.Random.Range(0f, 1f)) * Mathf.Sin(theta);

                GameObject item = Instantiate(_droppedItems[i].GetGameObject(), dropPosition, transform.rotation);
            }
        }
    }


    //math reference: https://stackoverflow.com/questions/5837572/generate-a-random-point-within-a-circle-uniformly
}
