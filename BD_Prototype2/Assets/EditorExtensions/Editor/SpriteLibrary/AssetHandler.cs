using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.GrapicsEditor
{


public class SpriteLibImportDragDrop : PointerManipulator
{
    ListView _listView;
    VisualElement _dragdropMsg;
    public List<Sprite> spriteList { get; private set; } = new List<Sprite>();
    public void Init(VisualElement root)
    {
        _listView = root.Q<ListView>();
        _listView.makeItem = () => CreateItem();
        _listView.bindItem = (item, index) => BindItem(item, index);
        _listView.itemsSource = spriteList;

        _dragdropMsg = root.Q<VisualElement>("dragdropmsg");
        target = _dragdropMsg;
    }

    // -- Clear list -- //
    public void ClearAssets()
    {
        _listView.selectedIndex = -1;
        spriteList.Clear();
        _listView.Rebuild();

        _dragdropMsg.style.display = DisplayStyle.Flex;
        _listView.style.display = DisplayStyle.None;
        target = _dragdropMsg;
    }

    public void RemoveNulls()
    {
        spriteList.RemoveAll(item => item == null);
    }

    // -- Creating visual elements -- //
    VisualElement CreateItem()
    {
        ObjectField item = new ObjectField();
        item.objectType = typeof(Sprite);
        item.label = "";
        item.RegisterValueChangedCallback
        (
            (ev) =>
            {
                if(spriteList.Count < 1)
                    spriteList.Add(ev.newValue as Sprite);
                else if(_listView.selectedIndex > -1)
                    spriteList[_listView.selectedIndex] = ev.newValue as Sprite;

                _listView.RefreshItem(_listView.selectedIndex); //Rebuild is recursive, caused a stack overflow
            }
        );

        return item;
    }

    void BindItem(VisualElement item, int index)
    {
        Sprite asset = spriteList[index];
        item.Q<ObjectField>().value = asset;
    }

    // -- Drag and drop assets -- //
    protected override void RegisterCallbacksOnTarget()
    {
        _dragdropMsg.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        _dragdropMsg.RegisterCallback<DragPerformEvent>(OnDragPerform);

        _listView.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        _listView.RegisterCallback<DragPerformEvent>(OnDragPerform);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        _dragdropMsg.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
        _dragdropMsg.UnregisterCallback<DragPerformEvent>(OnDragPerform);

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

        if(target == _dragdropMsg)
        {
            _dragdropMsg.style.display = DisplayStyle.None;
            _listView.style.display = DisplayStyle.Flex;
            target = _listView;
        }

        foreach(var obj in objs)
        {
            if(obj.GetType() == typeof(Sprite))
                spriteList.Add(obj as Sprite);

            if(obj.GetType() == typeof(Texture2D))
            {
                var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(obj));
                foreach(var asset in assets)
                {
                    if(asset.GetType() == typeof(Sprite))
                        spriteList.Add(asset as Sprite);
                }
            }

            continue;
        }

        _listView.Rebuild();
    }

}


}