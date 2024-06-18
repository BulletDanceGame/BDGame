using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using BulletDance.Editor;


namespace BulletDance.LevelBuilder
{

public class ZoneManager
{
    public static ZoneManager Instance { get; private set; }

    public ZoneManager() { Instance = this; }

    public static GameObject[] allRooms { get { return LevelBuilder.GetRooms(); } }

    public GameObject currentRoom { get; private set; } = null;
    public RoomController currentController { get; private set; } = null;
    public EnvironmentConductor currentZone  { get; private set; } = null;

    public static EnvironmentConductor[] AllZones(GameObject room)
    {
        GameObject zoneFolder = GetZoneFolder(room);
        if(zoneFolder != null)
            return zoneFolder.GetComponentsInChildren<EnvironmentConductor>();
        else
            return null;
    }

    static readonly string[] zoneFolderNames = {"MusicConductors", "EnvironmentConductors"};
    public static GameObject GetZoneFolder(GameObject room)
    {
        foreach(Transform child in room.transform)
        {
            if(child.name == zoneFolderNames[0] || child.name == zoneFolderNames[1]) 
                return child.gameObject;
        }

        return null;
    }


    public void Init()
    {
        SetCurrentZone(Selection.activeGameObject);
    }

    void SetCurrentZone(GameObject gameObject)
    {
        currentRoom = LevelBuilder.GetRoomFromChild(gameObject);
        currentZone = gameObject?.GetComponent<EnvironmentConductor>();
        currentController = currentRoom?.GetComponentInChildren<RoomController>();
    }

    public static void GetZoneFromRoom(GameObject room, out RoomController controller, out EnvironmentConductor[] zones)
    {
        controller = room.GetComponentInChildren<RoomController>();
        zones      = room.GetComponentsInChildren<EnvironmentConductor>();
    }


    // -- Zone Selection -- //
    public event Action OnChangeZone;
    public void UpdateObjectSelection()
    {
        GameObject selectedObject = Selection.activeGameObject;
        SetCurrentZone(selectedObject);
        OnChangeZone?.Invoke();
    }

    // -- Zone Creation -- //
    public event Action OnAddZone;
    public void CreateZone(GameObject room)
    {
        var zoneObject = EditorMenuLib.InstantiatePrefab(prefabContainer => prefabContainer.musicConductorPrefab);

        var zonefolder = GetZoneFolder(room);
        if(zonefolder == null)
        {
            zonefolder = new GameObject(zoneFolderNames[0]);
            zonefolder.transform.parent = room.transform;
        }
        zoneObject.transform.parent = zonefolder.transform;

        Bounds bounds = BulletDance.Maths.BoundBox.GetRenderedBounds(room.transform);
        zoneObject.transform.position = bounds.center;

        SetCurrentZone(zoneObject);

        OnAddZone?.Invoke();
    }

    // -- Zone Deletion -- //
    public event Action OnDeleteZone;
    public void DeleteCurrentZone()
    {
        GameObject.DestroyImmediate(currentZone.gameObject);
        currentController.RemoveNullConductors();

        Selection.activeGameObject = GetZoneFolder(currentRoom);
        UpdateObjectSelection();

        OnDeleteZone?.Invoke();
    }
}

}