using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

using BulletDance.Editor;


namespace BulletDance.LevelBuilder
{


public class AddRoomPopUp : PopupWindowContent
{
    public GameObject room;

    //Set the window size
    public override Vector2 GetWindowSize()
    {
        return new Vector2(280, 160);
    }

    public override void OnGUI(Rect rect){}

    private string _popupXMLPath = "Assets/EditorExtensions/Editor/LevelBuilder/LevelBuilderWindow/AddRoomPopUp.uxml";
    TextField     name;
    DropdownField type;
    Vector3Field  gridSize, gridGap;

    public override void OnOpen()
    {
        var root = editorWindow.rootVisualElement;
        var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(_popupXMLPath);
        visualTreeAsset.CloneTree(root);

        name = root.Q<TextField>();

        gridSize = root.Q<Vector3Field>("size");
        gridGap  = root.Q<Vector3Field>("gap");

        GridSettings grid = GridSettings.TileGrid();
        gridSize.value = grid.cellSize;
        gridGap.value = grid.cellGap;

        type = root.Q<DropdownField>();
        type.RegisterCallback((ChangeEvent<string> ev) => { OnTypeChange(ev); });
        type.choices = new List<string>(){"Tile", "GameObject"};
        type.index   = 0;

        var confirm = root.Q<Button>();
        confirm.clicked += OnButtonConfirm;
    }

    void OnTypeChange(ChangeEvent<string> ev)
    {
        GridSettings grid = ev.newValue == "Tile" ? GridSettings.TileGrid() : GridSettings.ObjectGrid();
        gridSize.value = grid.cellSize;
        gridGap.value  = grid.cellGap;
    }

    void OnButtonConfirm()
    {
        LayerType layerType = type.value == "Tile" ? LayerType.Tile : LayerType.GameObject;
        GridSettings grid   = new GridSettings(gridSize.value, gridGap.value);

        LayerManager.Instance.CreateLayer(room, name.value, layerType, grid);
        editorWindow.Close();
    }

    public override void OnClose() {}
}



}