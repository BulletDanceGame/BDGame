using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using BulletDance.Editor;


namespace BulletDance.LevelBuilder
{


public class PaletteHandler
{
    private static PaletteHandler instance = null;
    public static PaletteHandler Instance
    {
        get
        {
            if(instance == null) 
                instance = new PaletteHandler();
            return instance;
        }
    }

    const string nullName = "No Palette available";
    List<string> _nullPalette = new List<string>(){ nullName };
    public Dictionary<string, GameObject> paletteList;
    public void LoadPalettes()
    {
        paletteList = new Dictionary<string, GameObject>();

        //Find palette prefab assets and add them to list
        string[] guids = AssetDatabase.FindAssets("t:GridPalette");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GridPalette paletteAsset = AssetDatabase.LoadAssetAtPath(path, typeof(GridPalette)) as GridPalette;

            if (paletteAsset != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(paletteAsset);
                GameObject palette = AssetDatabase.LoadMainAssetAtPath(assetPath) as GameObject;
                if (palette != null)
                    paletteList.Add(EditorExt.FormatPropertyName(palette.name), palette);
            }
        }

        //Add null palette
        paletteList.Add(nullName, null);

        SortPalettes();
    }

    List<string> _tilePalette, _objectPalete;
    public void SortPalettes()
    {
        _tilePalette  = new List<string>();
        _objectPalete = new List<string>();
        
        foreach(string palette in paletteList.Keys)
        {
            var name = palette.ToLower();
            if(name.Contains("tile"))
                _tilePalette.Add(palette);
            if(name.Contains("gameobject") || name.Contains("game object") || name.Contains("prefab"))
                _objectPalete.Add(palette);
        }

        if(_tilePalette.Count  < 1) _tilePalette  = _nullPalette;
        if(_objectPalete.Count < 1) _objectPalete = _nullPalette;
    }

    public List<string> currentPalettes { get; private set; }
    public void SetCurrentPalettes(bool TileCondition, bool GameObjectCondition, bool nullCondition)
    {
        if(nullCondition) 
        {
            currentPalettes = _nullPalette;
            return;
        }

        currentPalettes = TileCondition       ? _tilePalette  :
                          GameObjectCondition ? _objectPalete :
                          _nullPalette;
    }

}



}