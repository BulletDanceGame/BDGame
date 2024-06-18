using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

using BulletDance.Editor;


namespace BulletDance.LevelBuilder
{


public class RoomList
{
    private VisualElement _panel;
    private Button _addRoom, _deleteLayer;
    private VisualTreeAsset _listTemplate;

    public void Init(VisualElement root, VisualTreeAsset listTemplate)
    {
        _listTemplate = listTemplate;

        _addRoom = root.Q<Button>("AddRoom");
        _addRoom.clicked += AddRoom;

        _deleteLayer = root.Q<Button>("deleteLayer");
        _deleteLayer.clicked += LayerManager.Instance.DeleteCurrentLayer;

        _panel = root.Q<VisualElement>("RoomList");
        UpdateRoomList();
    }

    public void UpdateRoomList()
    {
        _panel.Clear();

        var allRooms = LevelBuilder.GetRooms();
        foreach(var room in LayerManager.allRooms)
        {
            var tempPanel = new VisualElement();
            _listTemplate.CloneTree(tempPanel);
            CreateLayerList(room, tempPanel.Q<ListView>());

            var foldout   = tempPanel.Q<Foldout>();
            foldout.text  = room.name;
            //foldout.SetValueWithoutNotify(false); //Collapse foldout

            var button = tempPanel.Q<Button>();
            button.clicked += () =>
            {
                var popup  = new AddRoomPopUp();
                popup.room = room;
                UnityEditor.PopupWindow.Show(button.worldBound, popup);
            };

            _panel.Add(tempPanel);
        }
    }

    ListView CreateLayerList(GameObject room, ListView listView)
    {
        var layers = LayerManager.AllLayers(room);

        //Make later Query possible
        listView.name = room.name;
        listView.selectedIndex   = -1; //Not selected
        listView.fixedItemHeight = 20;

        //Create layer items and binds them to layers (GameObject)
        listView.makeItem = () => new Label();
        listView.bindItem = (item, index) => { (item as Label).text = layers[index].name; };
        listView.itemsSource = layers;

        // React to the user's selection
        listView.onSelectionChange += OnLayerSelectionChange;

        return listView;
    }


    // -- Layer selection -- //
    void OnLayerSelectionChange(IEnumerable<object> selectedItems)
    {
        Layer layer = selectedItems.First() as Layer;

        // Select GameObject in the scene
        Selection.activeObject = layer.gameObject;
        LayerManager.Instance.UpdateObjectSelection();
    }


    void AddRoom()
    {
        var roomObject = EditorMenuLib.InstantiateGameObject(prefabContainer => prefabContainer.roomTemplate);

        var currentRoom = LayerManager.Instance.currentRoom;
        roomObject.transform.parent = GameObject.Find("Rooms")?.transform;

        //Name the room by the room count in the scene
        var foundRooms = GameObject.FindGameObjectsWithTag("Room");
        roomObject.name = "Room " + (foundRooms.Length);

        Selection.activeGameObject = roomObject;
        LayerManager.Instance.UpdateObjectSelection();
    }
}



}