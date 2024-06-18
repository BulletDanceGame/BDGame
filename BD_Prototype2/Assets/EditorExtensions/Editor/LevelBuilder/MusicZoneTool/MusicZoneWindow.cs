using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.LevelBuilder
{


public class MusicZoneWindow : EditorWindow
{
    // -- Open/Refresh the window -- //
    [MenuItem("Tools/Music Zone Tool")]
    public static void ShowEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<MusicZoneWindow>();
        wnd.titleContent = new GUIContent("Music Zone");

        // Limit size of the window
        wnd.minSize = new Vector2(450, 200);
        wnd.maxSize = new Vector2(1920, 1080);
    }

    void OnSelectionChange()
    {
        _zoneManager.UpdateObjectSelection();
    }


    // -- Window creation -- //

    public VisualTreeAsset _windowXML, _zoneListTemplateXML;
    private VisualElement  _root;

    private ZoneManager _zoneManager = new ZoneManager();
    private ZoneList  _zoneList  = new ZoneList();
    private ZoneInfo  _zoneInfo  = new ZoneInfo();
    private ZoneTools _zoneTools = new ZoneTools();

    public void CreateGUI()
    {
        // Root of UI
        _root = new VisualElement();
        _windowXML.CloneTree(_root);

        //Zone manager
        _zoneManager.Init();

        //Zone List
        _zoneList.Init(_root, _zoneListTemplateXML);
        _zoneList.CreateRoomList();

        //Zone Info
        _zoneInfo.Init(_root);

        //Zone Tools
        _zoneTools.Init(_root);

        rootVisualElement.Add(_root);

        _zoneManager.OnChangeZone += _zoneInfo.UpdateInfo;
        _zoneManager.OnChangeZone += _zoneTools.UpdateView;
        _zoneManager.OnAddZone    += _zoneList.UpdateRoomList;
        _zoneManager.OnAddZone    += _zoneTools.RetargetCurrent;
        _zoneManager.OnDeleteZone += _zoneList.UpdateRoomList;

    }

    public void OnDestroy()
    {
        _zoneManager.OnChangeZone -= _zoneInfo.UpdateInfo;
        _zoneManager.OnChangeZone -= _zoneTools.UpdateView;
        _zoneManager.OnAddZone    -= _zoneList.UpdateRoomList;
        _zoneManager.OnAddZone    -= _zoneTools.RetargetCurrent;
        _zoneManager.OnDeleteZone -= _zoneList.UpdateRoomList;
    }
}

}