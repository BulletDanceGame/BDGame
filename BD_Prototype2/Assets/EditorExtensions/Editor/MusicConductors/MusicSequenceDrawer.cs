using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine;
using BulletDance.Editor;

[CustomPropertyDrawer(typeof(MusicSequence))]
public class MusicSequenceDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        //FUCK WHY UNITY
        var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/MusicConductors/MusicSequenceDrawer.uxml");

        var root = new VisualElement();
        xml.CloneTree(root);

        //Fix bindings
        var main = root.Q<Foldout>("seqMain");

        var name = root.Q<TextField>("name");
        name.BindProperty(property.FindPropertyRelative("name"));
        name.RegisterCallback((InputEvent ev) => { main.text = ev.newData; });

        var song = root.Q<PropertyField>("song");
        song.BindProperty(property.FindPropertyRelative("song"));

        var beatMapActions = root.Q<PropertyField>("beatMapActions");
        beatMapActions.BindProperty(property.FindPropertyRelative("beatMapActions"));

        var beatMapPlayer = root.Q<PropertyField>("beatMapPlayer");
        beatMapPlayer.BindProperty(property.FindPropertyRelative("beatMapPlayer"));

        var bpm = root.Q<IntegerField>("bpm");
        bpm.BindProperty(property.FindPropertyRelative("bpm"));


        var stop = root.Q<Toggle>("stop");
        stop.BindProperty(property.FindPropertyRelative("stoppedEarly"));

        var switchplay = root.Q<Toggle>("switch");
        switchplay.BindProperty(property.FindPropertyRelative("keepPlayingOnSwitch"));

        var replay = root.Q<Toggle>("replay");
        replay.BindProperty(property.FindPropertyRelative("replayInSameSequence"));


        Foldout sheetFoldout = root.Q<Foldout>("sheetFoldout");
        sheetFoldout.RegisterCallback<ChangeEvent<bool>, EventHandler>(DrawRows, new EventHandler(property, sheetFoldout));

        var duration = root.Q<IntegerField>("duration");
        duration.BindProperty(property.FindPropertyRelative("duration"));
        duration.RegisterCallback<ChangeEvent<int>, EventHandler>(ChangedDuration, new EventHandler(property, sheetFoldout));



        Rows(property, sheetFoldout);


        return root;
    }

    public void Rows(SerializedProperty property, Foldout sheetFoldout)
    {

        SerializedProperty items = property.FindPropertyRelative("sheet");

        //Set correct amount based on duration value
        int dur = property.FindPropertyRelative("duration").intValue / 8;
        dur += (property.FindPropertyRelative("duration").intValue % 8.0 == 0) ? 0 : 1;
        if (items.arraySize != dur)
        {
            //add rows
            if (items.arraySize < dur)
            {
                for (int i = items.arraySize; i < dur; i++)
                {
                    items.InsertArrayElementAtIndex(i);
                    items.GetArrayElementAtIndex(i).FindPropertyRelative("barNr").stringValue = items.arraySize.ToString();

                    SerializedProperty notes = items.GetArrayElementAtIndex(i).FindPropertyRelative("notes");
                    notes.ClearArray();
                    for (int n = 0; n < 8; n++)
                    {
                        notes.InsertArrayElementAtIndex(n);
                        notes.GetArrayElementAtIndex(n).FindPropertyRelative("functionName").stringValue = " - ";
                        notes.GetArrayElementAtIndex(n).FindPropertyRelative("color").colorValue = Color.white;
                    }
                }
            }
            //delete rows
            else if (items.arraySize > dur)
            {
                for (int i = items.arraySize - 1; i >= dur; i--)
                {
                    items.DeleteArrayElementAtIndex(i);
                }
            }

            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

        }

    }

    public void DrawRows(ChangeEvent<bool> ev, EventHandler eh)
    {
        if (ev.newValue == true)
        {
            SerializedProperty items = eh.property.FindPropertyRelative("sheet");
            //Draw
            for (int i = 0; i < items.arraySize; i++)
            {
                SerializedProperty foundProperty = items.GetArrayElementAtIndex(i);

                SerializedProperty barNr = foundProperty.FindPropertyRelative("barNr");
                barNr.stringValue = (i + 1).ToString();


                PropertyField field = new PropertyField();
                field.name = "Row";
                field.BindProperty(foundProperty);
                eh.sheetFoldout.Add(field);
            }
        }
        else
        {
            eh.sheetFoldout.Clear();
        }
        
    }



    public class EventHandler
    {
        public SerializedProperty property;
        public Foldout sheetFoldout;

        public EventHandler(SerializedProperty _property, Foldout _sheetFoldout)
        {
            property = _property;
            sheetFoldout = _sheetFoldout;
        }

    }




    public void ChangedDuration(ChangeEvent<int> ev, EventHandler eh)
    {
        eh.property.FindPropertyRelative("duration").intValue = ev.newValue;

        int dur = ev.previousValue / 8;
        dur += (ev.previousValue % 8.0 == 0) ? 0 : 1;
        for (int i = 0; i < dur; i++)
        {
            PropertyField row = eh.sheetFoldout.Q<PropertyField>("Row");
            if (row != null)
                eh.sheetFoldout.Remove(row);
        }

        Rows(eh.property, eh.sheetFoldout);
    }
}
