using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine;
using UnityEditor.Tilemaps;

using UnityEditor.UIElements;
using UnityEngine.UIElements;

using BDEditor = BulletDance.Editor;

namespace BulletDance.LevelBuilder
{


public class TilePalette
{
    DropdownField _dropdown;
    public void Init(VisualElement root)
    {
        _dropdown = root.Q<DropdownField>();
        _dropdown.RegisterValueChangedCallback((evt) => { SelectPallete(); });

        PaletteHandler.Instance.LoadPalettes();
        UpdateDropdown();
    }


    LayerType layerType;
    bool TileCondition, GameObjectCondition;
    public void UpdateDropdown()
    {
        var layer = LayerManager.Instance.currentLayer;
        if(layer.isValid)
        {
            layerType = layer.layerProperties.layerType;
            TileCondition  = (layerType == LayerType.Tile);
            GameObjectCondition = (layerType == LayerType.GameObject);
        }

        PaletteHandler.Instance.SetCurrentPalettes(TileCondition, GameObjectCondition, !layer.isValid);
        _dropdown.choices = PaletteHandler.Instance.currentPalettes;
        _dropdown.index   = 0;
    }

    //Palette selection
    void SelectPallete()
    {
        GridPaintingState.palette = PaletteHandler.Instance.paletteList[_dropdown.value];
    }
}



}