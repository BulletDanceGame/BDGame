using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.LevelBuilder
{


public class LevelBuilderWindow : EditorWindow
{
    // -- Open/Refresh the window -- //
    [MenuItem("Tools/Level Builder")]
    public static void ShowEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<LevelBuilderWindow>();
        wnd.titleContent = new GUIContent("Level Builder");

        // Limit size of the window
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 1080);
    }

    void OnSelectionChange()
    {
        LayerManager.Instance.UpdateObjectSelection();
    }

    void OnHierarchyChange()
    {
        _roomList.UpdateRoomList();
    }

    void OnProjectChange()
    {
        PaletteHandler.Instance.LoadPalettes();
        _palette.UpdateDropdown();
    }

    // -- Window creation -- //

    public VisualTreeAsset _windowXML, _layerListTempXML;
    private VisualElement  _root;

    private LayerManager _layerManager = new LayerManager();
    private RoomList  _roomList  = new RoomList();
    private LayerInfo _layerInfo = new LayerInfo();
    private BrushInfo _brushInfo = new BrushInfo();
    private TilePalette _palette = new TilePalette();

    public void CreateGUI()
    {
        // Root of UI
        _root = new VisualElement();
        _windowXML.CloneTree(_root);

        _layerManager.Init();

        // Room List
        _roomList.Init(_root, _layerListTempXML);

        // Room Info
        _layerInfo.Init(_root);

        // Tile Pallete
        _palette.Init(_root);

        // Brush Settings
        _brushInfo.Init(_root);
        _brushInfo.CreateInfoView();


        //Event subscription
        _layerManager.OnChangeLayer += _layerInfo.UpdateGridInfo;
        _layerManager.OnChangeLayer += _brushInfo.SwitchBrush;
        _layerManager.OnChangeLayer += _palette.UpdateDropdown;

        rootVisualElement.Add(_root);
    }

    public void OnDestroy()
    {
        _layerManager.OnChangeLayer -= _layerInfo.UpdateGridInfo;
        _layerManager.OnChangeLayer -= _brushInfo.SwitchBrush;
        _layerManager.OnChangeLayer -= _palette.UpdateDropdown;
    }
}


}