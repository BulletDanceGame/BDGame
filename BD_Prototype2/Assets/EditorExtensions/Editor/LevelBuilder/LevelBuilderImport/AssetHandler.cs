using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.LevelBuilder
{


public class AssetHandler : PointerManipulator
{
    VisualTreeAsset _itemTempXML;
    DropdownField _objType;
    ListViewH _listView;
    public void Init(VisualElement root, VisualTreeAsset xml)
    {
        _objType = root.Q<DropdownField>("type");
        _objType.RegisterCallback<ChangeEvent<string>>((ChangeEvent<string> ev) => { UpdateListType(); } );

        _listView = root.Q<ListViewH>("objects");
        _listView.makeItem = () => CreateItem();
        _listView.bindItem = (item, index) => BindItem(item, index);
        _listView.itemsSource = isSpriteType ? spriteAssets : prefabAssets;
        target = _listView;

        var add = root.Q<Button>("add");
        add.clicked += AddItem;

        var delete = root.Q<Button>("delete");
        delete.clicked += DeleteItem;

        _itemTempXML = xml;
    }
    

    // -- Clear list -- //
    public void ClearAssets()
    {
        _listView.selectedIndex = -1;
        spriteAssets.Clear();
        prefabAssets.Clear();
        _listView.Rebuild();
    }

    public void RemoveNulls()
    {
        spriteAssets.RemoveAll(item => item == null);
        prefabAssets.RemoveAll(item => item == null);
    }

    // -- Switch import type -- //
    bool isSpriteType { get { return _objType.value == "Sprites"; } }

    public List<Sprite>     spriteAssets { get; private set; } = new List<Sprite>();
    public List<GameObject> prefabAssets { get; private set; } = new List<GameObject>();
    void UpdateListType()
    {
        spriteAssets.RemoveAll(item => item == null);
        prefabAssets.RemoveAll(item => item == null);

        _listView.itemsSource = isSpriteType ? spriteAssets : prefabAssets;
        _listView.Rebuild();
        _listView.selectedIndex = -1;
    }


    // -- Add/remove-- //
    void AddItem()
    {
        if(isSpriteType)
            spriteAssets.Add(null);

        if(!isSpriteType)
            prefabAssets.Add(null);

        _listView.Rebuild();
    }

    void DeleteItem()
    {
        if(_listView.selectedIndex == -1) return;
        if(isSpriteType && _listView.selectedIndex < spriteAssets.Count)
            spriteAssets.RemoveAt(_listView.selectedIndex);

        if(!isSpriteType && _listView.selectedIndex < prefabAssets.Count)
            prefabAssets.RemoveAt(_listView.selectedIndex);

        _listView.Rebuild();
    }


    // -- Drag and drop assets -- //
    protected override void RegisterCallbacksOnTarget()
    {
        _listView.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        _listView.RegisterCallback<DragPerformEvent>(OnDragPerform);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        _listView.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
        _listView.UnregisterCallback<DragPerformEvent>(OnDragPerform);
    }

    void OnDragUpdate(DragUpdatedEvent ev)
    {
        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
    }

    void OnDragPerform(DragPerformEvent ev)
    {
        var objs = DragAndDrop.objectReferences;

        foreach(var obj in objs)
        {
            if(!isSpriteType && (obj.GetType() == typeof(GameObject)))
                prefabAssets.Add(obj as GameObject);

            if(isSpriteType && (obj.GetType() == typeof(Sprite)))
                spriteAssets.Add(obj as Sprite);

            if(isSpriteType && (obj.GetType() == typeof(Texture2D)))
            {
                var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
                foreach(var asset in assets)
                {
                    if(asset.GetType() == typeof(Sprite))
                        spriteAssets.Add(asset as Sprite);
                }
            }

            continue;
        }

        _listView.Rebuild();
    }


    // -- Creating visual elements -- //
    VisualElement CreateItem()
    {
        VisualElement item = new VisualElement();
        _itemTempXML.CloneTree(item); //_itemTempXML is null??
        item.Q<ObjectField>().objectType = isSpriteType ? typeof(Sprite) : typeof(GameObject);
        item.Q<ObjectField>().RegisterValueChangedCallback
        (
            (ev) =>
            {
                if(isSpriteType)
                {
                    if(spriteAssets.Count < 1)
                        spriteAssets.Add(ev.newValue as Sprite);
                    else if(_listView.selectedIndex > -1)
                        spriteAssets[_listView.selectedIndex] = ev.newValue as Sprite;
                }
                else
                {
                    if(prefabAssets.Count < 1)
                        prefabAssets.Add(ev.newValue as GameObject);
                    else if(_listView.selectedIndex > -1)
                        prefabAssets[_listView.selectedIndex] = ev.newValue as GameObject;
                }

                item.Q<VisualElement>("preview").style.backgroundImage = AssetPreview.GetAssetPreview(ev.newValue);
                _listView.RefreshItem(_listView.selectedIndex); //Rebuild is recursive, caused a stack overflow
            }
        );

        return item;
    }

    void BindItem(VisualElement item, int index)
    {
        if(isSpriteType)
        {
            Sprite asset = spriteAssets[index];
            item.Q<ObjectField>().value = asset;
            item.Q<VisualElement>("preview").style.backgroundImage = AssetPreview.GetAssetPreview(asset);
        }
        else
        {
            GameObject asset = prefabAssets[index];
            item.Q<ObjectField>().value = asset;
            item.Q<VisualElement>("preview").style.backgroundImage = AssetPreview.GetAssetPreview(asset);
        }
    }

}


}