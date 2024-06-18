using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.Tilemaps;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using BDEditor = BulletDance.Editor;

namespace BulletDance.LevelBuilder
{


public class BrushInfo
{
    const string tileBrush   = "TileBrush";
    const string prefabBrush = "PrefabBrush";
    public GridBrushBase currentBrush { get; private set; } = null;

    ScrollView _panel;
    Label _message;
    public void Init(VisualElement root)
    {
        LoadBrushes();
        SetDefaultBrush();

        _panel   = root.Q<ScrollView>("BrushInfo");
        _message = root.Q<Label>("brushMsg");
    }


    // -- Brush info -- //
    public void CreateInfoView()
    {
        UpdateInfoView();
    }

    public void UpdateInfoView()
    {
        _panel.Clear();

        //Message
        _message.text = "Current brush: " + BDEditor.EditorExt.FormatPropertyName(currentBrush.name);

        //Find serialized properties
        int propertyCount = 0;
        SerializedObject   brushRef = new SerializedObject(currentBrush);
        SerializedProperty foundProperty = brushRef.GetIterator();

        // GetIterator returns a root that is above all others
        // so first property is found by stepping into the children
        foundProperty.NextVisible(true);

        // For rest of scan stay at the same level (false)
        do
        {
            //Skip properties
            if(foundProperty.name == "m_Script") continue;
            propertyCount++;

            //Create property field and add it to panel
            PropertyField property = new PropertyField();
            property.BindProperty(foundProperty);
            property.label = BDEditor.EditorExt.FormatPropertyName(foundProperty.name);

            _panel.Add(property);
        }
        while (foundProperty.NextVisible(false));
    }


    // -- Load Brush -- //
    Dictionary<string, GridBrushBase> _brushList = new Dictionary<string, GridBrushBase>();
    void LoadBrushes()
    {
        string[] guids = AssetDatabase.FindAssets("t:GridBrushBase");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var brush = AssetDatabase.LoadAssetAtPath(path, typeof(GridBrushBase)) as GridBrushBase;
            if (brush != null)
                _brushList.Add(brush.name, brush);
        }
    }


    // -- Brush selection -- //
    public void SetDefaultBrush()
    {
        currentBrush = _brushList[tileBrush];
    }

    public void SwitchBrush()
    {
        var layer = LayerManager.Instance.currentLayer;

        if(!layer.isValid) return;

        string brush = "";

        switch(layer.layerProperties.layerType)
        {
            case LayerType.Tile:
                brush = tileBrush;   break;

            case LayerType.GameObject:
                brush = prefabBrush; break;

            default:
                brush = tileBrush; break;
        }

        currentBrush = _brushList[brush];
        GridPaintingState.gridBrush = currentBrush;

        UpdateInfoView();
    }
}



}