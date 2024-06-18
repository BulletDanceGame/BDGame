using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using BulletDance.Editor;


namespace BulletDance.LevelBuilder
{

public class Layer
{
    public string name { get; private set; }
    public GameObject gameObject { get; private set; }
    public LayerProperties layerProperties { get; private set; }

    public Layer(GameObject _gameObject, LayerProperties _layerProperties)
    {
        layerProperties = _layerProperties;
        gameObject      = _gameObject;

        if(gameObject != null)
        {
            name = gameObject.name;
            name =  name.Contains("Grid") ?
                    name.Split("Grid")[0].Trim() : name;
        }
    }

    public bool isValid { get { return gameObject != null && layerProperties != null; } }
}

public class LayerManager
{
    public static LayerManager Instance { get; private set; }
    public LayerManager() { Instance = this; }

    public static GameObject[] allRooms { get { return LevelBuilder.GetRooms(); } }

    public GameObject currentRoom  { get; private set; } = null;
    public Layer      currentLayer { get; private set; } = null;


    private Layer GetLayer(GameObject gameObject)
    {
        return new Layer(gameObject, gameObject?.GetComponent<LayerProperties>());
    }

    public static Layer[] AllLayers(GameObject room)
    {
        var list = new List<Layer>();

        foreach(Transform child in room.transform)
        {
            var layerPropertiesList = child.GetComponentsInChildren<LayerProperties>();
            if(layerPropertiesList.Length > 0)
            {
                foreach(var layerProperties in layerPropertiesList)
                {
                    list.Add(new Layer(layerProperties.gameObject, layerProperties));
                }
            }
        }

        return list.ToArray();
    }


    public static GameObject GetRoom(Layer layer)
    {
        if(!layer.isValid) return null;
        return LevelBuilder.GetRoomFromChild(layer.gameObject);
    }


    public void Init()
    {
        SetCurrentLayer(Selection.activeGameObject);
    }

    void SetCurrentLayer(GameObject gameObject)
    {
        currentRoom  = gameObject ? null : LevelBuilder.GetRoomFromChild(gameObject);
        currentLayer = GetLayer(gameObject);
    }


    // -- Layer Selection -- //
    public event Action OnChangeLayer;
    public void UpdateObjectSelection()
    {
        GameObject selectedObject = Selection.activeGameObject;
        SetCurrentLayer(selectedObject);
        OnChangeLayer?.Invoke();
    }

    // -- Layer Creation -- //
    public event Action OnAddLayer;
    public void CreateLayer(GameObject room, string name, LayerType _layerType, GridSettings _defaultGrid = null)
    {
        var layerObject  = EditorMenuLib.InstantiateGameObject(prefabContainer => prefabContainer.roomLayerTemplate);
        layerObject.name = name;
        layerObject.transform.parent = room.transform;

        var property = layerObject.GetComponent<LayerProperties>();
        property.SetProperties(_layerType, _defaultGrid);
        property.ResetGrid();

        if(_layerType == LayerType.GameObject)
        {
            GameObject.DestroyImmediate(layerObject.GetComponent<UnityEngine.Tilemaps.TilemapRenderer>());
            GameObject.DestroyImmediate(layerObject.GetComponent<UnityEngine.Tilemaps.Tilemap>());
        }

        currentLayer = GetLayer(layerObject);

        OnAddLayer?.Invoke();
    }

    // -- Layer Deletion -- //
    public event Action OnDeleteLayer;
    public void DeleteCurrentLayer()
    {
        GameObject.DestroyImmediate(currentLayer.gameObject);

        Selection.activeGameObject = currentRoom;
        UpdateObjectSelection();

        OnDeleteLayer?.Invoke();
    }
}

}