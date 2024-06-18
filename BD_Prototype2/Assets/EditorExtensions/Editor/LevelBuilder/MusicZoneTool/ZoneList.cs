using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

using BDEditor = BulletDance.Editor;


namespace BulletDance.LevelBuilder
{


public class ZoneList
{
    private ScrollView _panel;
    private VisualTreeAsset _listTemplate;

    public void Init(VisualElement root, VisualTreeAsset zoneListTemplate)
    {
        _panel = root.Q<ScrollView>("RoomList");
        _listTemplate = zoneListTemplate;
    }

    public void CreateRoomList()
    {
        UpdateRoomList();
    }


    public void UpdateRoomList()
    {
        _panel.Clear();

        foreach(var room in ZoneManager.allRooms)
        {
            var tempPanel = new VisualElement();
            _listTemplate.CloneTree(tempPanel);
            CreateZoneList(room, tempPanel.Q<ListView>());

            var foldout   = tempPanel.Q<Foldout>();
            foldout.text  = room.name;
            //foldout.SetValueWithoutNotify(false); //Collapse foldout

            var button = tempPanel.Q<Button>();
            button.clicked += () => { ZoneManager.Instance.CreateZone(room); };

            _panel.Add(tempPanel);
        }
    }

    void CreateZoneList(GameObject room, ListView listView)
    {
        var conductors = ZoneManager.AllZones(room);

        //Make later Query possible
        listView.name = room.name;
        listView.selectedIndex   = -1; //Not selected
        listView.fixedItemHeight = 20;

        //Create conductors items and binds them to conductors (GameObject)
        listView.makeItem = () => new Label();
        listView.bindItem = (item, index) => { (item as Label).text = conductors[index].name; };
        listView.itemsSource = conductors;

        // React to the user's selection
        listView.onSelectionChange += OnZoneSelectionChange;
    }

    void OnZoneSelectionChange(IEnumerable<object> selectedItems)
    {
        EnvironmentConductor conductors = selectedItems.First() as EnvironmentConductor;

        // Select GameObject in the scene
        Selection.activeGameObject = conductors.gameObject;
        ZoneManager.Instance.UpdateObjectSelection();
    }
}



}