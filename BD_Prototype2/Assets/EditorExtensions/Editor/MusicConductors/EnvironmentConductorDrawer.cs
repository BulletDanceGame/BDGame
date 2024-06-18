using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;
using System.Drawing.Printing;

namespace BulletDance.Editor
{
    [System.Serializable]
    public class EnvironmentConductorDrawer
    {
        SerializedObject _zoneRef = null;
        public void SetZone(SerializedObject zone)
        {
            _zoneRef = zone;
        }

        public void SetZone(EnvironmentConductor zone)
        {
            _zoneRef = new SerializedObject(zone);
        }


        private VisualElement _root, _panel, _varPanel, _groupPanel;
        public VisualElement Init(VisualElement root, string rootName = "")
        {
            if(root != null)
                 _root = root.Q<VisualElement>(rootName);
            else _root = new VisualElement();

            _root.Clear();

            var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/MusicConductors/MusicConductorEditor.uxml");
            xml.CloneTree(_root);

            _panel    = _root.Q<VisualElement>("musicCondRoot");
            _varPanel = _panel.Q<VisualElement>("variables");
            _groupPanel = _panel.Q<VisualElement>("groups");

            return _panel;
        }

        public VisualElement Redraw()
        {
            DrawVariables();
            DrawGroups();
            DrawButtons();
            _panel.schedule.Execute(RegisterCallbacks).StartingIn(800 * (groupList.Count*2 + 1));
            return _panel;
        }


        void DrawVariables()
        {
            _varPanel.Clear();

            SerializedProperty foundProperty = _zoneRef.GetIterator();
            foundProperty.NextVisible(true);

            do
            {
                //Skip sfxGroups, that one is handled separately
                if(foundProperty.name == "_sequenceGroups" || foundProperty.name == "m_Script") continue;

                PropertyField property = new PropertyField();
                property.BindProperty(foundProperty);
                property.label = EditorExt.FormatPropertyName(foundProperty.name);

                _varPanel.Add(property);
            }
            while (foundProperty.NextVisible(false));
        }


        List<PropertyField> groupList = new List<PropertyField>();

        void DrawGroups()
        {
            _groupPanel.Clear();

            groupList = new List<PropertyField>();
            SerializedProperty groups = _zoneRef.FindProperty("_sequenceGroups");

            var enumerator = groups.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SerializedProperty foundProperty = enumerator.Current as SerializedProperty;

                PropertyField property = new PropertyField();
                property.BindProperty(foundProperty);
                groupList.Add(property);
                _groupPanel.Add(property);

            } 

        }




        int selectedGroup, selectedItem, selectedRow, selectedNote;
        void DrawButtons()
        {
            var addGroup = _panel.Q<Button>("addGroup");
            addGroup.RegisterCallback<ClickEvent>(
                (ClickEvent ev) => 
                {
                    var groups = _zoneRef.FindProperty("_sequenceGroups");
                    groups.InsertArrayElementAtIndex(groups.arraySize);
                    _zoneRef.ApplyModifiedProperties();
                    _zoneRef.Update();
                    Redraw();
                }
            );

            var removeGroup = _panel.Q<Button>("removeGroup");
            removeGroup.RegisterCallback<ClickEvent>(
                (ClickEvent ev) => 
                {
                    var groups = _zoneRef.FindProperty("_sequenceGroups");
                    groups.DeleteArrayElementAtIndex(selectedGroup);
                    _zoneRef.ApplyModifiedProperties();
                    _zoneRef.Update();
                    Redraw();
                }
            );



        }


        bool CheckIfEverythingIsDrawn()
        {
            //CHECK THAT EVERYTHING IS DRAWN

            if (groupList[^1].Q<Foldout>() != null)
            {
                //seqeuences

                List<Foldout> lastSequences = groupList[^1].Q<Foldout>().Query<Foldout>("seqMain").ToList();

                SerializedProperty groups = _zoneRef.FindProperty("_sequenceGroups");
                SerializedProperty sequences = groups.GetArrayElementAtIndex(groups.arraySize - 1).FindPropertyRelative("sequences");
                int correctSize = sequences.arraySize;
                if (correctSize == 0) return true;

                if (lastSequences.Count == correctSize)
                {
                    //rows
                    Debug.Log(lastSequences[correctSize - 1].name);
                    List<GroupBox> lastRows = lastSequences[correctSize - 1]?.Query<GroupBox>("Row").ToList();
                    correctSize = sequences.GetArrayElementAtIndex(correctSize - 1).FindPropertyRelative("sheet").arraySize;
                    if (correctSize == 0) return true;

                    if (lastRows.Count == correctSize)
                    {
                        //notes
                        List<ColorField> lastNotes = lastRows[correctSize - 1].Query<ColorField>("colorShown").ToList();
                        correctSize = 8;

                        if (lastNotes.Count == correctSize)
                        {
                            return true;
                        }
                    }
                }

            }

            Debug.Log("wait to register callbacks, everything isnt drawn yet");
            _panel.schedule.Execute(RegisterCallbacks).StartingIn(1500);
            return false;
        }

        void RegisterCallbacks()
        {
            //if (!CheckIfEverythingIsDrawn()) { return; } //motherfucker can only query AFTER GUI is created

            var groupMsg = _panel.Q<Label>("groupMsg");

            foreach(var child in groupList)
            {
                var foldout = child.Q<Foldout>();
                foldout?.RegisterCallback<ClickEvent>(
                    (ClickEvent ev) => 
                    {
                        selectedGroup = groupList.IndexOf(child);
                        groupMsg.text = "Current group: " + child.Q<Foldout>().text;
                    }
                );

                var removeItem = child.Q<Button>("removeItem");
                removeItem?.RegisterCallback<ClickEvent>((ClickEvent ev) => { DeleteSequence(); });

                var addItem = child.Q<Button>("addItem");
                addItem?.RegisterCallback<ClickEvent>((ClickEvent ev) => { AddSequence(); });



                var seqMsg = child.Q<Label>("seqMsg");
                List<Foldout> sequences = foldout?.Query<Foldout>("seqMain").ToList();
                if (sequences == null) continue;
                foreach(Foldout sequenceFold in sequences)
                {
                    //if (sequences.IndexOf(sequenceFold) == 0) continue;
                    sequenceFold.RegisterCallback<ClickEvent, EventHandler>(ClickedOn, //this doesn't work, for some reason??? It works as Editor but not here???
                        new EventHandler(sequences.IndexOf(sequenceFold), groupList.IndexOf(child), sequenceFold, seqMsg)); //Update: it works now???????????
                    //CallbackMethod(element)

                }
            }
        }


        class EventHandler
        {
            public int itemIndex, groupIndex, noteIndex, rowIndex;
            public string id;
            public Foldout foldout;
            public Label message;

            public EventHandler(int _itemIndex, int _groupIndex, Foldout _foldout, Label _label)
            {
                itemIndex = _itemIndex;
                groupIndex = _groupIndex;
                foldout = _foldout;
                message = _label;
            }

            public GroupBox note;

            public EventHandler(int _noteIndex, int _rowIndex, int _itemIndex, int _groupIndex, Foldout _foldout, Label _label, GroupBox _note)
            {
                noteIndex = _noteIndex;
                rowIndex = _rowIndex;
                itemIndex = _itemIndex;
                groupIndex = _groupIndex;
                foldout = _foldout;
                message = _label;
                note = _note;
                id = _groupIndex.ToString() + _itemIndex.ToString() + _rowIndex.ToString() + _noteIndex.ToString();
            }
        }

        void ClickedOn(ClickEvent ev, EventHandler eventHandler)
        {
            selectedItem  = eventHandler.itemIndex;
            selectedGroup = eventHandler.groupIndex;
            eventHandler.message.text = "Current sequence: " + eventHandler.foldout.text;
        }




        void DeleteSequence()
        {
            SerializedProperty items = _zoneRef.FindProperty("_sequenceGroups").GetArrayElementAtIndex(selectedGroup).FindPropertyRelative("sequences");

            items.DeleteArrayElementAtIndex(selectedItem);
            _zoneRef.ApplyModifiedProperties();
            _zoneRef.Update();
            Redraw();
        }

        void AddSequence()
        {
            SerializedProperty items = _zoneRef.FindProperty("_sequenceGroups").GetArrayElementAtIndex(selectedGroup).FindPropertyRelative("sequences");
            items.InsertArrayElementAtIndex(selectedItem);
            _zoneRef.ApplyModifiedProperties(); 
            _zoneRef.Update();
            Redraw();
        }







    }

}