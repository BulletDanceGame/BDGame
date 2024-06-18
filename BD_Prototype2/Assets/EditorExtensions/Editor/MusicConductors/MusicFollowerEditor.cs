using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[CustomEditor(typeof(MusicFollower))]
public class MusicFollowerEditor : Editor
{
    public VisualTreeAsset musicFollowerXML;

    public VisualElement root;

    public int selectedSequence, selectedRow, selectedNote;

    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our inspector UI
        root = new VisualElement();

        // Add a simple label
        root.Add(new Label("This is a Music Follower"));

        // Load from default reference
        musicFollowerXML.CloneTree(root);


        // Return the finished inspector UI
        return Draw();
    }

    VisualElement Draw()
    {
        EnumField followerType = root.Q<EnumField>("followerType");
        SerializedProperty follower = serializedObject.FindProperty("_follower");
        followerType.BindProperty(follower);
        followerType.RegisterCallback((ChangeEvent<Enum> ev) => {
            fuckyou();
        });


        GroupBox movelistGroup = root.Q<GroupBox>("movelistGroup");
        movelistGroup.Clear();

        if (follower.enumValueIndex == 0)
        {
            PropertyField movelist = new PropertyField();
            movelist.BindProperty(serializedObject.FindProperty("_movelist"));
            movelistGroup.Add(movelist);
        }
        else if (follower.enumValueIndex == 1)
        {
            PropertyField movelists = new PropertyField();
            movelists.BindProperty(serializedObject.FindProperty("_movelists"));
            movelistGroup.Add(movelists);
        }

        PropertyField beatActions = root.Q<PropertyField>("beatActions");
        beatActions.BindProperty(serializedObject.FindProperty("_beatActions"));


        Button removeItem = root.Q<Button>("removeSequence");
        removeItem?.RegisterCallback((ClickEvent ev) => { DeleteSequence(); });

        Button addItem = root.Q<Button>("addSequence");
        addItem?.RegisterCallback((ClickEvent ev) => { AddSequence(); });

        DrawSequences();


        return root;

    }

    void fuckyou()
    {
        DrawSequences();
    }

    void DrawSequences()
    {
        GroupBox sheet = root.Q<GroupBox>("sequences");
        sheet.Clear();
        SerializedProperty items = serializedObject.FindProperty("_sequences");
        var enumerator = items.GetEnumerator();
        while (enumerator.MoveNext())
        {
            SerializedProperty foundProperty = enumerator.Current as SerializedProperty;

            PropertyField field = new PropertyField();
            field.BindProperty(foundProperty);
            field.label = foundProperty.name;
            sheet.Add(field);
        }


        root.schedule.Execute(RegisterCallbacks).StartingIn(800 * (items.arraySize + 1));
    }


    bool CheckIfEverythingIsDrawn()
    {
        List<Foldout> sequences = root?.Query<Foldout>("seqMain").ToList();

        //CHECK THAT EVERYTHING IS DRAWN

        //seqeuences
        SerializedProperty s = serializedObject.FindProperty("_sequences");
        int correctSize = s.arraySize;
        if (correctSize == 0) return true;

        if (sequences.Count == correctSize)
        {
            //rows
            List<GroupBox> lastRows = sequences[correctSize - 1]?.Query<GroupBox>("Row").ToList();
            correctSize = s.GetArrayElementAtIndex(correctSize - 1).FindPropertyRelative("sheet").arraySize;
            if (correctSize == 0) return true;

            if (lastRows.Count == correctSize)
            {
                //notes
                List<ColorField> lastNotes = lastRows[correctSize - 1]?.Query<ColorField>("colorShown").ToList();
                correctSize = 8;

                if (lastNotes.Count == correctSize)
                {
                    return true;
                }
            }
        }

        Debug.Log("wait to register callbacks, everything isnt drawn yet");
        root.schedule.Execute(RegisterCallbacks).StartingIn(800);
        return false;
    }


    void RegisterCallbacks()
    {
        //if (!CheckIfEverythingIsDrawn()) { return; } //it will call this again, so it will simply happen later


        List<Foldout> sequences = root.Query<Foldout>("seqMain").ToList();

        foreach (Foldout sequence in sequences)
        {
            sequence.RegisterCallback((ClickEvent ev) => { ClickOnSequence(sequences.IndexOf(sequence), sequence.text); });

            Button addRow = sequence.Q<Button>("addRow");
            addRow?.RegisterCallback((ClickEvent ev) => { AddRow(); });

            Button removeRow = sequence.Q<Button>("removeRow");
            removeRow?.RegisterCallback((ClickEvent ev) => { DeleteRow(); });


        }

        
    }



    void ClickOnSequence(int sequenceIndex, string sequenceName)
    {
        selectedSequence = sequenceIndex;
        root.Q<Label>("currentSequence").text = "Current Sequence: " + sequenceName;
    }


    void DeleteSequence()
    {
        SerializedProperty items = serializedObject.FindProperty("_sequences");

        items.DeleteArrayElementAtIndex(selectedSequence);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        DrawSequences();
    }

    void AddSequence()
    {
        SerializedProperty items = serializedObject.FindProperty("_sequences");

        items.InsertArrayElementAtIndex(items.arraySize);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        DrawSequences();
    }



    void DeleteRow()
    {
        SerializedProperty items = serializedObject.FindProperty("_sequences").
                GetArrayElementAtIndex(selectedSequence).FindPropertyRelative("sheet");

        if (items.arraySize == 0) { return; }

        items.DeleteArrayElementAtIndex(items.arraySize - 1); 
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        DrawSequences();
    }

    void AddRow()
    {
        SerializedProperty sheet = serializedObject.FindProperty("_sequences").
                GetArrayElementAtIndex(selectedSequence).FindPropertyRelative("sheet");

        sheet.InsertArrayElementAtIndex(sheet.arraySize);
        sheet.GetArrayElementAtIndex(sheet.arraySize - 1).FindPropertyRelative("barNr").stringValue = sheet.arraySize.ToString();

        SerializedProperty notes = sheet.GetArrayElementAtIndex(sheet.arraySize - 1).FindPropertyRelative("notes");
        for (int n = 0; n < 8; n++)
        {
            notes.InsertArrayElementAtIndex(n);
            notes.GetArrayElementAtIndex(n).FindPropertyRelative("functionName").stringValue = " - ";
            notes.GetArrayElementAtIndex(n).FindPropertyRelative("color").colorValue = Color.white;
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        DrawSequences();
    }



}
