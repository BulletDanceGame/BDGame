using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.UIElements;


[CustomEditor(typeof(MainMenuConductor))]
public class MainMenuConductorEditor : Editor
{
    public static SerializedProperty copied;

    public VisualElement root;

    public int selectedSequence, selectedRow, selectedNote;

    public override VisualElement CreateInspectorGUI()
    {
        var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/MusicConductors/MainMenuConductor.uxml");

        root = new VisualElement();
        xml.CloneTree(root);




        // Return the finished inspector UI
        return Draw();
    }

    VisualElement Draw()
    {


        Button removeItem = root.Q<Button>("removeSequence");
        removeItem?.RegisterCallback((ClickEvent ev) => { DeleteSequence(); });

        Button addItem = root.Q<Button>("addSequence");
        addItem?.RegisterCallback((ClickEvent ev) => { Debug.Log("log"); AddSequence(); });

        DrawSequences();


        return root;

    }



    void DrawSequences()
    {
        GroupBox sheet = root.Q<GroupBox>("menuSequences");
        sheet.Clear();
        SerializedProperty items = serializedObject.FindProperty("_menuSequences");
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
        SerializedProperty s = serializedObject.FindProperty("_menuSequences");
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
        if (!CheckIfEverythingIsDrawn()) { return; } //it will call this again, so it will simply happen later


        List<Foldout> sequences = root.Query<Foldout>("seqMain").ToList();

        foreach (Foldout sequence in sequences)
        {
            sequence.RegisterCallback((ClickEvent ev) => { ClickOnSequence(sequences.IndexOf(sequence), sequence.text); });

            Button addRow = sequence.Q<Button>("addRow");
            addRow?.RegisterCallback((ClickEvent ev) => { AddRow(); });

            Button removeRow = sequence.Q<Button>("removeRow");
            removeRow?.RegisterCallback((ClickEvent ev) => { DeleteRow(); });


            //ROWS
            GroupBox noteInfo = sequence.Q<GroupBox>("currentNote");
            List<GroupBox> rows = sequence?.Query<GroupBox>("Row").ToList();

            foreach (GroupBox row in rows)
            {
                //NOTES
                List<ColorField> colors = row?.Query<ColorField>("colorShown").ToList();

                foreach (ColorField color in colors)
                {
                    Label note = color.Q<Label>();

                    note.RegisterCallback((ClickEvent ev) => {
                        ClickOnNote(
                        sequences.IndexOf(sequence), sequence.text, rows.IndexOf(row), colors.IndexOf(color), sequence);
                    });

                }
            }
        }


    }





    void ClickOnSequence(int sequenceIndex, string sequenceName)
    {
        selectedSequence = sequenceIndex;
        root.Q<Label>("currentSequence").text = "Current Sequence: " + sequenceName;

    }

    void ClickOnNote(int sequenceIndex, string sequenceName, int rowIndex, int noteIndex, Foldout sequenceFold)
    {
        selectedSequence = sequenceIndex;
        root.Q<Label>("currentSequence").text = "Current Sequence: " + sequenceName;
        selectedRow = rowIndex;
        selectedNote = noteIndex;


        //show note info

        SerializedProperty note = serializedObject.FindProperty("_menuSequences").GetArrayElementAtIndex(selectedSequence).
            FindPropertyRelative("sheet").GetArrayElementAtIndex(selectedRow).
            FindPropertyRelative("notes").GetArrayElementAtIndex(selectedNote);


        GroupBox noteInfo = sequenceFold.Q<GroupBox>("currentNote");
        noteInfo.Q<Label>("noteName").text = "Current: Bar " + (selectedRow + 1).ToString() + ", Note " + (selectedNote + 1).ToString();

        PropertyField noteField = noteInfo.Q<PropertyField>("note");
        noteField.BindProperty(note);


        noteField.Q<PropertyField>("bullets").RemoveFromHierarchy();
        noteField.Q<EnumField>("movelistEnum").RemoveFromHierarchy();
        noteField.Q<IntegerField>("movelistsToRandomize").RemoveFromHierarchy();
        noteField.Q<PropertyField>("specifiedMovelists").RemoveFromHierarchy();



        //movelists outside
        //PropertyField movelists = sequenceFold.Q<PropertyField>("movelists");
        //movelists.BindProperty(note.FindPropertyRelative("noteData").FindPropertyRelative("movelists"));

    }


    void DeleteSequence()
    {
        Debug.Log("de");
        SerializedProperty items = serializedObject.FindProperty("_menuSequences");

        items.DeleteArrayElementAtIndex(selectedSequence);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        DrawSequences();
    }

    void AddSequence()
    {
        Debug.Log("add");
        SerializedProperty items = serializedObject.FindProperty("_menuSequences");

        items.InsertArrayElementAtIndex(items.arraySize);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        DrawSequences();
    }



    void DeleteRow()
    {
        SerializedProperty items = serializedObject.FindProperty("_menuSequences").
                GetArrayElementAtIndex(selectedSequence).FindPropertyRelative("sheet");

        if (items.arraySize == 0) { return; }

        items.DeleteArrayElementAtIndex(items.arraySize - 1);
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        DrawSequences();
    }

    void AddRow()
    {
        SerializedProperty items = serializedObject.FindProperty("_menuSequences").
                GetArrayElementAtIndex(selectedSequence).FindPropertyRelative("sheet");

        items.InsertArrayElementAtIndex(items.arraySize);
        items.GetArrayElementAtIndex(items.arraySize - 1).FindPropertyRelative("barNr").stringValue = items.arraySize.ToString();

        SerializedProperty notes = items.GetArrayElementAtIndex(items.arraySize - 1).FindPropertyRelative("notes");
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