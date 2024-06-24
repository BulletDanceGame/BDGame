using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

[CustomPropertyDrawer(typeof(Note))]
public class NoteDrawer : PropertyDrawer
{
    public static SerializedProperty copied;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/RasmusTesting/Note.uxml");

        VisualElement root = new VisualElement();
        xml.CloneTree(root);


        property.serializedObject.Update();

        Label title = root.Q<Label>("title");
        title.text = property.FindPropertyRelative("functionName").stringValue;

        Toggle forPlayer = root.Q<Toggle>("forPlayer");
        forPlayer.BindProperty(property.FindPropertyRelative("forPlayer"));

        TextField functionName = root.Q<TextField>("functionName");
        functionName.BindProperty(property.FindPropertyRelative("functionName"));

        ColorField color = root.Q<ColorField>("color");
        color.BindProperty(property.FindPropertyRelative("color"));

        //shooting
        PropertyField bullets = root.Q<PropertyField>("bullets");
        bullets.BindProperty(property.FindPropertyRelative("bullets"));


        //group
        EnumField movelistEnum = root.Q<EnumField>("movelistEnum");
        movelistEnum.BindProperty(property.FindPropertyRelative("movelistsToTrigger"));
        movelistEnum.RegisterCallback((ChangeEvent<Enum> ev) => { DrawGroupVariables(root, property); });


        //to copy
        title.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
        {
            //COPY
            evt.menu.AppendAction("Copy", (x) =>
            {
                copied = property;
            });

            //PASTE
            evt.menu.AppendAction("Paste", (x) =>
            {
                property.serializedObject.Update();

                //color and functionname
                property.boxedValue = copied.boxedValue;
                title.text = property.FindPropertyRelative("functionName").stringValue;

                property.serializedObject.ApplyModifiedProperties();
            });

        }));



        return root;
    }


    public void DrawGroupVariables(VisualElement root, SerializedProperty property)
    {
        int enumValue = property.FindPropertyRelative("movelistsToTrigger").enumValueIndex;
        IntegerField movelistsToRandomize = root.Q<IntegerField>("movelistsToRandomize");
        PropertyField specifiedMovelists = root.Q<PropertyField>("specifiedMovelists");


        if (enumValue == 0)
        {
            movelistsToRandomize.style.display = DisplayStyle.Flex;
            movelistsToRandomize.BindProperty(property.FindPropertyRelative("amountToRandomizeBetween"));
            specifiedMovelists.style.display = DisplayStyle.None;
        }
        else if (enumValue == 1)
        {
            movelistsToRandomize.style.display = DisplayStyle.Flex;
            movelistsToRandomize.BindProperty(property.FindPropertyRelative("amountToRandomizeBetween"));
            specifiedMovelists.style.display = DisplayStyle.Flex;
            specifiedMovelists.BindProperty(property.FindPropertyRelative("specifiedMovelists"));
        }
        else if (enumValue == 2)
        {
            movelistsToRandomize.style.display = DisplayStyle.None;
            specifiedMovelists.style.display = DisplayStyle.Flex;
            specifiedMovelists.BindProperty(property.FindPropertyRelative("specifiedMovelists"));
        }
    }


}






[CustomPropertyDrawer(typeof(Row))]
public class RowDrawer : PropertyDrawer
{
    public static SerializedProperty copiedRow;
    public static SerializedProperty copiedNote;
    public VisualElement thisRoot;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/RasmusTesting/Row.uxml");

        VisualElement root = new VisualElement();
        xml.CloneTree(root);
        thisRoot = root;

        property.serializedObject.Update();

        Label bar = root.Q<Label>("Bar");
        bar.text = property.FindPropertyRelative("barNr").stringValue;


        SerializedProperty notes = property.FindPropertyRelative("notes");
        for (int i = 0; i < 8; i++)
        {
            SerializedProperty note = notes.GetArrayElementAtIndex(i);

            VisualElement visualPlayerNote = root.Q<VisualElement>("PlayerNote" + (i + 1).ToString());
            visualPlayerNote.style.backgroundColor = (note.FindPropertyRelative("forPlayer").boolValue) ? Color.green : Color.red;

            VisualElement visualNote = root.Q<VisualElement>("Note" + (i + 1).ToString());
            visualNote.style.backgroundColor = note.FindPropertyRelative("color").colorValue;
            visualNote.RegisterCallback<ClickEvent, EventHandler>(ClickOnNote,
                new EventHandler(note, root, property.FindPropertyRelative("barNr").stringValue, (i+1).ToString(),visualNote, visualPlayerNote));



            visualNote.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction("Copy", (x) => {

                    copiedNote = note;

                });
                evt.menu.AppendAction("Paste", (x) =>
                {
                    SerializedProperty s = note;
                    s.serializedObject.Update();

                    s.boxedValue = copiedNote.boxedValue;
                    visualNote.style.backgroundColor = copiedNote.FindPropertyRelative("color").colorValue;
                    visualPlayerNote.style.backgroundColor = (note.FindPropertyRelative("forPlayer").boolValue) ? Color.green : Color.red;

                    s.serializedObject.ApplyModifiedProperties();
                });

            }));


        }





        bar.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
        {
            evt.menu.AppendAction("Copy", (x) => {

                copiedRow = property;

            });
            evt.menu.AppendAction("Paste", (x) =>
            {
                property.serializedObject.Update();

                //actual notes
                for (int i = 0; i < 8; i++)
                {
                    property.FindPropertyRelative("notes").GetArrayElementAtIndex(i).boxedValue = 
                                copiedRow.FindPropertyRelative("notes").GetArrayElementAtIndex(i).boxedValue;
                    property.serializedObject.ApplyModifiedProperties();
                }


                SerializedProperty notes = property.FindPropertyRelative("notes");

                for (int i = 1; i < 9; i++)
                {
                    string n = "Note" + i.ToString();
                    VisualElement note = root.Q<VisualElement>(n);
                    note.style.backgroundColor = notes.GetArrayElementAtIndex(i-1).FindPropertyRelative("color").colorValue;

                    VisualElement playerNote = root.Q<VisualElement>("Player" + n);
                    playerNote.style.backgroundColor = (notes.GetArrayElementAtIndex(i - 1).FindPropertyRelative("forPlayer").boolValue) ? Color.green : Color.red;
                }

                property.serializedObject.ApplyModifiedProperties();
            });

        }));





        return root;
    }



    public class EventHandler
    {
        public SerializedProperty note;
        public VisualElement root;
        public string rowNr, noteNr;

        public EventHandler(SerializedProperty _note, VisualElement _root, string _rowNr, string _noteNr, VisualElement _visualNote, VisualElement _visualPlayerNote)
        {
            note = _note;
            root = _root;
            rowNr = _rowNr;
            noteNr = _noteNr;
            visualNote = _visualNote;
            visualPlayerNote = _visualPlayerNote;
        }


        public VisualElement visualNote;
        public VisualElement visualPlayerNote;
        public VisualElement v;

        public EventHandler(VisualElement _n)
        {
            v = _n;
        }


    }




    public void ClickOnNote(ClickEvent ev, EventHandler eh)
    {
        GroupBox noteInfo = eh.root.parent.parent.parent.Q<GroupBox>("currentNote");

        Label noteLabel = noteInfo.Q<Label>("noteName");
        noteLabel.text = "Current: Bar " + eh.rowNr + ", Note " + eh.noteNr;

        PropertyField noteField = noteInfo.Q<PropertyField>("infoNote");
        noteField.BindProperty(eh.note);


        ColorField color = noteField.Q<ColorField>("color");
        color.BindProperty(eh.note.FindPropertyRelative("color"));

        color.RegisterCallback<ChangeEvent<Color>, EventHandler>(ChangedNoteColor,
            new EventHandler(eh.visualNote));


        Toggle forPlayer = noteInfo.Q<Toggle>("forPlayer");
        forPlayer.BindProperty(eh.note.FindPropertyRelative("forPlayer"));
        forPlayer.RegisterCallback<ChangeEvent<bool>, EventHandler>(ToggleForPlayer, new EventHandler(eh.visualPlayerNote));

    }


    public void ChangedNoteColor(ChangeEvent<Color> ev, EventHandler eh)
    {
        eh.v.style.backgroundColor = ev.newValue;
    }

    public void ToggleForPlayer(ChangeEvent<bool> ev, EventHandler eh)
    {
        eh.v.style.backgroundColor = (ev.newValue) ? Color.green : Color.red;

    }
}




[CustomPropertyDrawer(typeof(FollowSequence))]
public class FollowSequenceDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualTreeAsset xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/RasmusTesting/FollowSequence.uxml");

        VisualElement root = new VisualElement();
        xml.CloneTree(root);

        //Fix bindings
        Foldout main = root.Q<Foldout>("seqMain");

        TextField name = root.Q<TextField>("name");
        name.BindProperty(property.FindPropertyRelative("name"));
        name.RegisterCallback<InputEvent>((InputEvent ev) => { main.text = ev.newData; });


        Toggle stop = root.Q<Toggle>("followSpecificSong");
        stop.BindProperty(property.FindPropertyRelative("followSpecificSong"));


        PropertyField song = root.Q<PropertyField>("songToFollow");
        song.BindProperty(property.FindPropertyRelative("songToFollow"));



        // PropertyField map = root.Q<PropertyField>("beatMap");
        // map.BindProperty(property.FindPropertyRelative("beatMap"));


        Foldout sheetFoldout = root.Q<Foldout>("sheetFoldout");
        sheetFoldout.RegisterCallback<ChangeEvent<bool>, EventHandler>(DrawRows, new EventHandler(property, sheetFoldout));

        IntegerField duration = root.Q<IntegerField>("duration");
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







