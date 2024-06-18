using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.EditorTools;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using BDEditor = BulletDance.Editor;

namespace BulletDance.LevelBuilder
{


public class ZoneInfo
{
    private VisualElement _panel;
    private Label _log, _selectLog;
    private BDEditor.EnvironmentConductorDrawer _zoneDrawer = new BDEditor.EnvironmentConductorDrawer();

    public void Init(VisualElement root)
    {
        _panel = root.Q<VisualElement>("ZoneInfo");
        _log   = root.Q<Label>("layerMsg");
        _selectLog = root.Q<Label>("groupMsg");

        _zoneDrawer.Init(_panel, "ZoneInfo");

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        var currentZone = ZoneManager.Instance.currentZone;
        _log.text = (currentZone == null) ?
                "Cannot find the MusicConductor component\nSelect a zone from the Room list to view" :
                "Current zone: " + currentZone.name + " of " + ZoneManager.Instance.currentRoom.name;
        
        _panel.style.display = (currentZone == null) ? DisplayStyle.None : DisplayStyle.Flex;

        if(currentZone != null)
        {
            _zoneDrawer.SetZone(currentZone);
            _zoneDrawer.Redraw();
        }
    }

}


}