using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using BulletDance.VFX;


public class VFXManager : MonoBehaviour
{
    private Dictionary<string, List<GameObject>> _vfxDict = new Dictionary<string, List<GameObject>>();
    private List<VFXObject> _updateList = new List<VFXObject>();
    private int idCounter = 0;

    public static VFXManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }


    // -- Helper methods -- //
    private List<GameObject> GetVFXList(string vfxName)
    {
        return _vfxDict.ContainsKey(vfxName) == true ?
                _vfxDict[vfxName] : null;
    }

    private GameObject GetVFXObject(List<GameObject> vfxList)
    {
        foreach(GameObject vfxObject in vfxList)
        {
            if(vfxObject.activeSelf) return vfxObject;
        }

        return null;
    }

    private void AddToUpdateList(GameObject vfx)
    {
        var _vfxObject = vfx.GetComponent<VFXObject>();
        AddToUpdateList(_vfxObject);
    }

    private void AddToUpdateList(VFXObject vfxObject)
    {
        if(vfxObject)
        {
            _updateList.Add(vfxObject);
            vfxObject.id = idCounter;

            idCounter++;
            if(idCounter > 5000) //Reset id in case of interger overflow
                idCounter = 0;
        }
    }

    public void RemoveFromUpdateList(VFXObject vfxObject)
    {
        for(int i = 0; i < _updateList.Count; i++)
        {
            if(_updateList[i].id == vfxObject.id)
            {
                _updateList.RemoveAt(i);
                return;
            }
        }
    }



    // -- Request VFX -- //
    public void RequestVFX(GameObject vfx, Vector3 position, Transform parent = null)
    {
        RequestVFX(vfx, position, Quaternion.identity, parent);
    }


    public void RequestVFX(GameObject vfx, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        Transform  _parent    = parent ? parent : this.transform; //If parent is unspecified, parent to this transform for clean scene
        GameObject _vfxObject = null;
        List<GameObject> _vfxList = GetVFXList(vfx.name);

        //VFX List is non-existent, Instantiate object and add to list & dict
        if(_vfxList == null)
        {
            _vfxObject = Instantiate(vfx, position, rotation, _parent);
            _vfxObject.name = vfx.name;

            _vfxList = new List<GameObject>();
            _vfxList.Add(_vfxObject);
            _vfxDict.Add(vfx.name, _vfxList);
            AddToUpdateList(_vfxObject);
            return;
        }

        //Got list, now get vfxObject
        _vfxObject = GetVFXObject(_vfxList);

        //If an object is available, set the vfxObject properties
        if(_vfxObject)
        {
            _vfxObject.SetActive(true);
            _vfxObject.transform.position = position;
            _vfxObject.transform.rotation = rotation;
            _vfxObject.transform.parent   = _parent;
            AddToUpdateList(_vfxObject);
        }

        //No object available, Instantiate and add to list
        else
        {
            _vfxObject      = Instantiate(vfx, position, rotation, _parent);
            _vfxObject.name = vfx.name;
            _vfxList.Add(_vfxObject);
            AddToUpdateList(_vfxObject);
        }
    }


    // -- Special VFX Request -- //
    public void RequestVFX_SlowMoZoom(Transform focusTransform)
    {
        var slowMoVFX = GetComponentInChildren<SlowMoZoom>();
        if(!slowMoVFX) return;

        slowMoVFX.SetAnimation(focusTransform);
        AddToUpdateList(slowMoVFX as VFXObject);
    }


    // -- Update vfx objects -- //
    void Update()
    {
        if(_updateList.Count < 1) return;

        foreach(VFXObject vfxObject in _updateList)
        {
            vfxObject.UpdateSelf();
        }
    }

}
