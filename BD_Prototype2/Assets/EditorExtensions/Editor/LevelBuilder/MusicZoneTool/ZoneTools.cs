using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using BDEditor = BulletDance.Editor;

namespace BulletDance.LevelBuilder
{


public class ZoneTools
{
    private VisualElement _panel;
    private Label _log;
    public void Init(VisualElement root)
    {
        _panel = root.Q<VisualElement>("tools");
        _log   = root.Q<Label>("toolMsg");
        _log.text = "";

        var delete = _panel.Q<Button>("DeleteZone");
        delete.clicked += () =>
        {
            _log.text = "Deleting " + ZoneManager.Instance.currentZone.name + " of " + ZoneManager.Instance.currentRoom.name;
            ZoneManager.Instance.DeleteCurrentZone();
        };

        var edit = _panel.Q<Button>("EditZone");
        edit.clicked += EditCollider;

        var retargetAll = root.Q<Button>("RetargetAll");
        retargetAll.clicked += RetargetAll;

        var retargetCurrent = root.Q<Button>("RetargetCurrent");
        retargetCurrent.clicked += RetargetCurrent;
    }

    public void UpdateView()
    {
        _panel.style.display = (ZoneManager.Instance.currentZone == null) ? DisplayStyle.None : DisplayStyle.Flex;
        _log.text = "";
        StopEditing();
    }


    // -- Edit tool -- //
    private bool _isEditing = false;
    private void EditCollider()
    {
        var currentZone = ZoneManager.Instance.currentZone;
        if(currentZone == null) return;

        var collider = currentZone.GetComponent<Collider2D>();
        if (!collider) return;
    
        //Currently editting, click button to stop editting
        if (_isEditing)
        {
            _log.text = "Exit collider edit mode";
            StopEditing();
            return;
        }

        //Click button to start editting
        _log.text = "Entered collider edit mode";

        //Fetching tool type with GetAssemblies, because it is hidden by Unity (fuck you unity I need to make editors)
        string tool = _tools[collider.GetType()];
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies) 
        {
            if (assembly.GetType(tool) != null) 
            {                
                //ToolManager.SetActiveTool(typeof(-----2DTool));  //Does not work bc it's hidden
                ToolManager.SetActiveTool(assembly.GetType(tool));
                _isEditing = true;
            }
        }
    }

    private void StopEditing()
    {
        ToolManager.RestorePreviousPersistentTool();
        _isEditing = false;
    }

    Dictionary<System.Type, string> _tools = new Dictionary<System.Type, string>()
    {
        {typeof(BoxCollider2D),     "UnityEditor.BoxCollider2DTool"},
        {typeof(CapsuleCollider2D), "UnityEditor.CapsuleCollider2DTool"},
        {typeof(CircleCollider2D),  "UnityEditor.CircleCollider2DTool"},
        {typeof(EdgeCollider2D),    "UnityEditor.EdgeCollider2DTool"},
        {typeof(PolygonCollider2D), "UnityEditor.PolygonCollider2DTool"}
    };


    // -- Retarget tools -- //
    public void RetargetAll()
    {
        _log.text = "Retargeting all room's EnemyController\n";

        foreach(var room in ZoneManager.allRooms)
        {
            _log.text += "Retargeting " + room.name +"\n";

            RoomController controller   = null;
            EnvironmentConductor[] zones = null;
            ZoneManager.GetZoneFromRoom(room, out controller, out zones);
            if(controller == null)
            {
                _log.text += "Skipping operation for " + room.name + ": EnemyController is not found" +"\n";
                continue;
            }

            var zoneFolder = ZoneManager.GetZoneFolder(room);
            if(zoneFolder == null)
            {
                _log.text += "Skipping operation for " + room.name + ": MusicConductors folder is not found" +"\n";
                continue;
            }
            
            controller.SetConductors(zones.ToList());
            controller.SetEnemies(new List<GameObject>());
            foreach(var zone in zones)
            {
                Collider2D zoneArea = zone.GetComponentInChildren<Collider2D>();
                controller.AddEnemies(FindEnemiesInArea(room, zoneArea));
            }
        }
    }

    public void RetargetCurrent()
    {
        _log.text = "Retargeting current conductor\n";

        //Shorthand
        var currentZone       = ZoneManager.Instance.currentZone;
        var currentController = ZoneManager.Instance.currentController;

        if(currentZone == null)
        {
            _log.text += "Cannot perform operation:\nNo zone is selected, or \ncurrent gameobject has no EnvironmentConductor component" +"\n";
            return;
        }

        if(currentController == null)
        {
            _log.text += "Cannot perform operation:\nThis gameobject is not part of a room, or \nEnemyController is not found in this room" +"\n";
            return;
        }

        currentController.RemoveNullConductors();
        currentController.RemoveNullEnemies();

        currentController.AddConductor(currentZone);

        Collider2D zoneArea = currentZone.GetComponentInChildren<Collider2D>();
        currentController.AddEnemies(FindEnemiesInArea(ZoneManager.Instance.currentRoom, zoneArea));
    }

    private MusicFollower[] GetRoomEnemies(GameObject room)
    {
        if(room == null) return null;

        //All enemies will probably have music follower
        return room.GetComponentsInChildren<MusicFollower>();
    }

    private List<GameObject> FindEnemiesInArea(GameObject room, Collider2D area)
    {
        List<Collider2D> overlapColliders = new List<Collider2D>();
        area.OverlapCollider(new ContactFilter2D().NoFilter(), overlapColliders);

        List<GameObject> foundEnemies = new List<GameObject>();
        var enemyList = GetRoomEnemies(room);
        foreach(var enemy in enemyList)
        {
            var collider = enemy.GetComponentInChildren<Collider2D>();
            if(collider == null) continue;

            if(overlapColliders.Contains(collider))
                foundEnemies.Add(collider.gameObject);
        }
        return foundEnemies;
    }
}


}