using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEditor.Tilemaps;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.LevelBuilder
{


public class ImportHandler
{
    ImportOptions _options = null;
    AssetHandler  _assetHandler = null;
    TextField _paletteName;
    Button _folderPath;

    public void Init(VisualElement root, AssetHandler assetHandler, ImportOptions options)
    {
        _options = options;
        _assetHandler = assetHandler;

        _paletteName = root.Q<TextField>("name");
        _folderPath  = root.Q<Button>("folder");
        _folderPath.clicked += () => 
        { 
            _path = EditorUtility.SaveFolderPanel("Choose folder", Application.dataPath + defaultPath, "");
            _folderPath.text = "Assets" + _path.Split("/Assets")[1];
        };

        var confirm = root.Q<Button>("confirm");
        confirm.clicked += ImportAssets;
    }

    string defaultPath = "/Prefabs/Level";
    string _path = "";
    string _name  = "";
    GameObject CreateNewPalette()
    {
        if(_path == "") _path = Application.dataPath + defaultPath;

        //Name safeguard
        _name = _paletteName.value;
        string search = _name.ToLower();
        if(_options.isSpriteType)
        {
            if(!search.Contains("tile")) 
                _name += " Tile Palette";
        }
        else
        {
            if(!search.Contains("prefab") || !search.Contains("gameobject") || !search.Contains("game object")) 
                _name += " Prefab Palette";
        }

        //Create Palette and load it
        GridPaletteUtility.CreateNewPalette(_path, _name, GridLayout.CellLayout.Rectangle,
                                            GridPalette.CellSizing.Manual, new Vector3(4, 4, 0), 
                                            GridLayout.CellSwizzle.XYZ);

        AssetDatabase.Refresh(); //Reload database before loading the file
        
        return AssetDatabase.LoadAssetAtPath<GameObject>(FileUtil.GetProjectRelativePath(_path + "/" + _name + ".prefab"));
    }

    void ImportAssets()
    {
        _assetHandler.RemoveNulls();

        if(_options.isSpriteType && _assetHandler.spriteAssets.Count > 0)
            ImportSprites();
        if(!_options.isSpriteType && _assetHandler.prefabAssets.Count > 0)
            ImportPrefabs();

        _path = "";
        _assetHandler.ClearAssets();
    }

    void ImportSprites()
    {
        GameObject palette  = _options.isNewPalette ? CreateNewPalette() : _options.selectedPalette;
        GameObject tempCopy = PrefabUtility.InstantiatePrefab(palette) as GameObject;
        Tilemap    tilemap  = tempCopy.GetComponentInChildren<Tilemap>();

        //Make Tiles
        foreach(var sprite in _assetHandler.spriteAssets)
        {
            TileBase tile = GenerateTile(sprite);
            Vector3Int position = new Vector3Int(0, 0, 0);

            //Keep shifting prefab position until it doesn't overlap with other sister objects
            bool overlap = true;
            int column = 10, i = 0;
            while(overlap)
            {
                overlap = tilemap.GetSprite(position) != null;
                if(!overlap) break;

                i = i > column ? 1 : i + 1;
                position += i < column ? new Vector3Int(1, 0, 0) : new Vector3Int(0, 1, 0);
            }

            tilemap.SetTile(position, tile);
        }

        //Destroy instance copy & open prefab
        PrefabUtility.ApplyObjectOverride(tilemap, AssetDatabase.GetAssetPath(palette), InteractionMode.AutomatedAction);
        PrefabUtility.SavePrefabAsset(palette);
        GameObject.DestroyImmediate(tempCopy);
        PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(palette));
    }

    TileBase GenerateTile(Sprite sprite)
    {
        if(_path == "") _path = Application.dataPath + defaultPath;
        if(!_options.isNewPalette)
        {
            _path = AssetDatabase.GetAssetPath(_options.selectedPalette);
            _path = _path.Replace(_options.selectedPalette.name, "");
            _path = _path.Replace("/.prefab", "");
        }

        TileBase tile = TileUtility.DefaultTile(sprite);
        AssetDatabase.CreateAsset(tile, _path + "/" + sprite.name + ".asset");
        return tile;
    }



    void ImportPrefabs()
    {
        GameObject palette  = _options.isNewPalette ? CreateNewPalette() : _options.selectedPalette;
        GameObject tempCopy = PrefabUtility.InstantiatePrefab(palette) as GameObject;
        Transform  parent   = tempCopy.transform.Find("Layer1");
        Grid       grid     = tempCopy.GetComponentInChildren<Grid>();

        //Cache sister object's bounds
        List<Bounds> sisterBounds = new List<Bounds>();
        foreach(Transform child in parent)
        {
            sisterBounds.Add(EncapsulatedPrefabBounds(child));
        }

        //Adding prefab into palette copy
        foreach(var prefab in _assetHandler.prefabAssets)
        {
            //Add & attach prefab to palette copy
            GameObject prefabCopy = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            prefabCopy.transform.parent = parent;

            Bounds prefabBounds = EncapsulatedPrefabBounds(prefabCopy.transform);
            prefabBounds.center = new Vector2(grid.cellSize.x / 2, grid.cellSize.y / 2);

            //Keep shifting prefab position until it doesn't overlap with other sister objects
            bool overlap = true;
            int column = 10, i = 0;
            while(overlap)
            {
                overlap = isPrefabOverlapOthers(prefabBounds, sisterBounds);
                if(!overlap) break;

                i = i > column ? 1 : i + 1;
                prefabBounds.center += i < column ? new Vector3(grid.cellSize.x, 0, 0) : new Vector3(0, grid.cellSize.y, 0);
                prefabBounds.center = new Vector2(i < column ?  prefabBounds.center.x : grid.cellSize.x / 2, prefabBounds.center.y);
            }
            prefabCopy.transform.position = prefabBounds.center;

            //Operation done, adding to sister bounds
            sisterBounds.Add(prefabBounds);
        }

        //Apply prefab addition to prefab
        foreach(var added in PrefabUtility.GetAddedGameObjects(tempCopy))
        {
            added.Apply(AssetDatabase.GetAssetPath(palette));
        }

        //Save prefab, destroy instance copy & open prefab
        PrefabUtility.SavePrefabAsset(palette);
        GameObject.DestroyImmediate(tempCopy);
        PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(palette));
    }

    Bounds EncapsulatedPrefabBounds(Transform parent)
    {
        Renderer[]   renderers = parent.GetComponentsInChildren<Renderer>();
        Collider2D[] colliders = parent.GetComponentsInChildren<Collider2D>();

        Bounds bounds = new Bounds();

        if (renderers.Length > 0)
            bounds = renderers[0].bounds;
        else if(colliders.Length > 0)
            bounds = colliders[0].bounds;

        foreach(var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        foreach(var collider in colliders)
        {
            bounds.Encapsulate(collider.bounds);
        }

        return bounds;
    }

    bool isPrefabOverlapOthers(Bounds thisBounds, List<Bounds> otherBounds)
    {
        foreach(var bound in otherBounds)
        {
            if(thisBounds.Intersects(bound))
                return true;
        }

        return false;
    }

}


}