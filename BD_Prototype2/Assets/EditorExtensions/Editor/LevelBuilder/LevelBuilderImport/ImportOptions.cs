using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.LevelBuilder
{


public class ImportOptions
{
    VisualElement _chooseExist, _createNew, _palettePreview;
    DropdownField _objType, _importType, _palettes;

    public void Init(VisualElement root)
    {
        _objType = root.Q<DropdownField>("type");
        _objType.choices = new List<string> {"Sprites", "Prefabs"};
        _objType.index = 0;
        _objType.RegisterCallback<ChangeEvent<string>>(
            (ChangeEvent<string> ev) => 
            { 
                UpdatePaletteChoices();
                UpdatePalettePreview();
            } );

        _importType = root.Q<DropdownField>("import");
        _importType.choices = new List<string> {"Existing palette", "New palette"};
        _importType.index = 0;
        _importType.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> ev) => { UpdateOptions(); } );

        _palettes = root.Q<DropdownField>("palettes");
        PaletteHandler.Instance.LoadPalettes();
        UpdatePaletteChoices();
        _palettes.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> ev) => { UpdatePalettePreview(); });

        _chooseExist = root.Q<VisualElement>("exist");
        _createNew   = root.Q<VisualElement>("new");
        UpdateOptions();

        _palettePreview = root.Q<VisualElement>("img");
        UpdatePalettePreview();
    }


    public bool isNewPalette { get { return _importType.value.Contains("New"); } }
    public bool isSpriteType { get { return _objType.value == "Sprites"; } }

    void UpdateOptions()
    {
        _chooseExist.style.display = !isNewPalette ? DisplayStyle.Flex : DisplayStyle.None;
        _createNew.style.display   = isNewPalette  ? DisplayStyle.Flex : DisplayStyle.None;
    }


    public void UpdatePaletteChoices()
    {
        PaletteHandler.Instance.SetCurrentPalettes(isSpriteType, !isSpriteType, false);
        _palettes.choices = PaletteHandler.Instance.currentPalettes;
        _palettes.index   = 0;
    }

    public GameObject selectedPalette
    {
        get {  return PaletteHandler.Instance.paletteList[_palettes.value];  }
    }

    public void UpdatePalettePreview()
    {
        Texture2D paletteImg = AssetPreview.GetAssetPreview(selectedPalette);
        _palettePreview.style.backgroundImage = paletteImg;

        if(paletteImg != null)
            _palettePreview.style.width  = 100 * paletteImg.width / paletteImg.height;
    }
}


}