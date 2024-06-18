using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.GrapicsEditor
{


public class SpritLibImportOptions
{
    public float slotStart = 0;
    public string animGroup = "Anim";
    public bool isNewGroup = true;

    TextField _groupName;
    DropdownField _animGroup;
    string addGroup = "Add new group";
    List<string> _choices 
    {
        get { return _animGroup.choices; }
        set
        {
            List<string> tempChoices = value;
            tempChoices.Add(addGroup);
            _animGroup.choices = tempChoices;
            _animGroup.index = 0;
            isNewGroup = _animGroup.value == addGroup;
            animGroup  = isNewGroup ? _groupName.value : _animGroup.value;
        }
    }

    public SpriteLibraryAsset spriteLibAsset = null;

    public void Init(VisualElement root)
    {
        var slot      = root.Q<VisualElement>("slotstart");
        var intSlider = slot.Q<Slider>();
        var intField  = slot.Q<TextField>();
        intSlider.RegisterValueChangedCallback((ev) => 
        {
            slotStart = ev.newValue;
            intField.value = ev.newValue.ToString();
        });

        intField.RegisterValueChangedCallback((ev) =>
        {
            int slotNum = int.Parse(ev.newValue);
            if(slotNum > 59) slotNum = 59;
            if(slotNum < 0)  slotNum = 0;

            slotStart = slotNum;
            intSlider.value = slotNum;
        });

        var newGroup = root.Q<VisualElement>("newgroup");
        _groupName = newGroup.Q<TextField>();
        _groupName.RegisterValueChangedCallback((ev) =>
        {
            if(isNewGroup) animGroup = ev.newValue;
        });

        _animGroup = root.Q<VisualElement>("animtype").Q<DropdownField>();
        _choices = new List<string>();
        _animGroup.RegisterValueChangedCallback((ev)=>
        {
            isNewGroup = ev.newValue == addGroup;
            newGroup.style.display = isNewGroup ? DisplayStyle.Flex : DisplayStyle.None;     
            animGroup  = isNewGroup ? _groupName.value : ev.newValue;
        });

        var spriteLib = root.Q<ObjectField>("spritelib");
        spriteLib.RegisterValueChangedCallback((ev)=>
        {
            spriteLibAsset = ev.newValue as SpriteLibraryAsset;
            _choices = spriteLibAsset.GetCategoryNames().ToList();
        });
    }

}


}