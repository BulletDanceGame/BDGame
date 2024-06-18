using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.LevelBuilder
{


public class LayerInfo
{
    // -- Grid Info view -- //
    private Label _message;
    private VisualElement _gridInfo, _deleteLayer;
    private SerializedObject _gridRef;

    public void Init(VisualElement root)
    {
        _message     = root.Q<Label>("layerMsg");
        _gridInfo    = root.Q<VisualElement>("gridInfo");
        _deleteLayer = root.Q<Button>("deleteLayer");

        var resetButton = _gridInfo.Q<Button>("resetGrid");

        UpdateGridInfo();
    }

    public void UpdateGridInfo()
    {
        var layer = LayerManager.Instance.currentLayer;

        _gridInfo.style.display    = !layer.isValid ? DisplayStyle.None : DisplayStyle.Flex;
        _deleteLayer.style.display = !layer.isValid ? DisplayStyle.None : DisplayStyle.Flex;

        //Escape method there is no layer
        if(layer.gameObject == null)
        {
            CreateMessage("null", layer);
            return;
        }

        if(layer.layerProperties == null)
        {
            CreateMessage("noGrid", layer);
            return;
        }

        //Rebind grid
        BindGrind(layer.layerProperties.grid);
        CreateMessage("GridExist", layer);
    }

    void CreateMessage(string messageCond, Layer layer)
    {
        string message = "";
        switch(messageCond)
        {
            case "null":
                message = "Nothing is selected right now\nSelect a layer from the Room list to view"; 
                break;

            case "noGrid":
                message = layer.name + " does not have a LayerProperties component\nSelect a layer from the rooms to view";
                break;
            
            default:
                message = "Current layer: " + layer.name + " of " + LayerManager.GetRoom(layer).name;
                break;
        }

        _message.text = message;
    }

    void BindGrind(Grid grid)
    {
        _gridRef = new SerializedObject(grid);
        _gridInfo.Q<PropertyField>("cellSize").BindProperty(_gridRef.FindProperty("m_CellSize"));
        _gridInfo.Q<PropertyField>("cellGap").BindProperty(_gridRef.FindProperty("m_CellGap"));
    }

    void BindButton()
    {
    }
}


}