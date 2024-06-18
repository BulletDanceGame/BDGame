using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using BulletDance.Audio;

namespace BulletDance.Editor
{
    [CustomEditor(typeof(SoundContainer), true)]
    public class SoundContainerEditor : UnityEditor.Editor
    {
        public VisualTreeAsset _inspectorXML, _listGroupTempXML, _listItemTempXML;
        VisualElement inspector;
        TextField groupNameField;

        public override VisualElement CreateInspectorGUI()
        {
            // Root of inspector UI
            inspector = new VisualElement();
            _inspectorXML.CloneTree(inspector);

            BindClassVariables();

            groupNameField = inspector.Q<TextField>("groupName");
            inspector.Q<Button>("addGroup").RegisterCallback<ClickEvent>(OnAddSFXGroup);
            inspector.Q<Button>("removeGroup").RegisterCallback<ClickEvent>(OnRemoveSFXGroup);

            inspector.Q("listUI").Add(BuildListUI());
            inspector.RegisterCallback<ClickEvent>(OnSFXItemDeSelect);

            return inspector;
        }

        void BindClassVariables()
        {
            //Create variable container
            Foldout foldout = new Foldout();
            foldout.text = "Variables";

            //Find serialized properties
            int propertyCount = 0;
            SerializedProperty foundProperty = serializedObject.GetIterator();

            // GetIterator returns a root that is above all others
            // so first property is found by stepping into the children
            foundProperty.NextVisible(true);

            // For rest of scan stay at the same level (false)
            do
            {
                //Skip sfxGroups, that one is handled with BuildListUI
                if(foundProperty.name == "_sfxGroups" || foundProperty.name == "m_Script") continue;
                propertyCount++;

                //Create property field and add it to foldout
                PropertyField property = new PropertyField();
                property.BindProperty(foundProperty);
                property.label = EditorExt.FormatPropertyName(foundProperty.name);

                foldout.Add(property);
            }
            while (foundProperty.NextVisible(false));

            //Add to inspector
            if(propertyCount > 0)  inspector.Q("Variables").Add(foldout);
        }

        void RebuildListUI()
        {
            var listUIroot = inspector.Q("listUI");
            listUIroot.RemoveAt(listUIroot.childCount-1);
            listUIroot.Add(BuildListUI());
        }

        VisualElement BuildListUI()
        {
            VisualElement listUI = new VisualElement();

            //Create list's groups
            var grouplist = serializedObject.FindProperty("_sfxGroups");
            for(int groupIndex = 0; groupIndex < grouplist.arraySize; groupIndex++)
            {
                var currentGroup = grouplist.GetArrayElementAtIndex(groupIndex);
                var groupName = currentGroup.FindPropertyRelative("groupName");

                //Build groupUI by cloning group template
                VisualElement groupUI = new VisualElement();
                _listGroupTempXML.CloneTree(groupUI);

                var foldout  = groupUI.Q<Foldout>("groupTemplate");
                foldout.text = groupName.stringValue;
                foldout.RegisterCallback<ClickEvent>(OnSFXItemDeSelect);
                foldout.RegisterCallback<ClickEvent, EventHandler>(OnSFXGroupSelect, new EventHandler(groupName.stringValue));

                groupUI.Q<GroupBox>("groupItemButtons").text = "Add/Remove selected SFX";
                groupUI.Q<Button>("addItem").RegisterCallback<ClickEvent, EventHandler>(OnAddSFX, new EventHandler(groupUI, groupName.stringValue));
                groupUI.Q<Button>("removeItem").RegisterCallback<ClickEvent, EventHandler>(OnRemoveSFX, new EventHandler(groupUI, groupName.stringValue));

                //Create group's items
                var sfxlist = currentGroup.FindPropertyRelative("sfxList");
                for(int itemIndex = 0; itemIndex < sfxlist.arraySize; itemIndex++)
                {
                    var currentSFX = sfxlist.GetArrayElementAtIndex(itemIndex);
                    CreateSFXItem(groupUI, currentSFX);
                }

                //Parent groupUI to final listUI
                listUI.Add(groupUI);
            }

            return listUI;
        }

        VisualElement CreateSFXItem(VisualElement parentGroup, SerializedProperty currentSFX)
        {
            var sfxName  = currentSFX.FindPropertyRelative("sfxName");
            var sfxEvent = currentSFX.FindPropertyRelative("sfxEvent");

            //Build itemUI by cloning item template
            VisualElement itemUI = new VisualElement();
            _listItemTempXML.CloneTree(itemUI);

            var foldout  = itemUI.Q<Foldout>("itemTemplate");
            foldout.text = sfxName.stringValue;
            foldout.RegisterCallback<ClickEvent, EventHandler>(OnSFXItemSelect, new EventHandler(foldout, sfxName.stringValue));

            var sfx  = SetupPropertyField(itemUI, "itemProperty", sfxEvent);
            var name = SetupPropertyField(itemUI, "itemName",     sfxName);
            name.RegisterCallback<InputEvent, EventHandler>(OnChangeSFXName, new EventHandler(foldout));

            //Parent itemUI to groupUI
            parentGroup.Q<Foldout>("groupTemplate").Add(itemUI);

            return itemUI;
        }

        PropertyField SetupPropertyField(VisualElement parent, string name, SerializedProperty bindProperty)
        {
            var property = parent.Q<PropertyField>(name);
            property.BindProperty(bindProperty);
            property.label = "";
            return property;
        }


        // -- Event handler -- //
        class EventHandler
        {
            public VisualElement UI { get; private set; }
            public string Name { get; private set; }
            public int Index  { get; private set; }

            public EventHandler(){}
            public EventHandler(VisualElement ui) { SetValue(ui, "", -1); }
            public EventHandler(string name) { SetValue(null, name, -1); }
            public EventHandler(VisualElement ui, string name) { SetValue(ui, name, -1); }

            public void SetValue(VisualElement ui, string name = "", int index = -1)
            {
                UI = ui;
                Index = index;
                Name = name;
            }
            public void SetUI(VisualElement ui) { UI = ui; }
            public void SetName(string name) { Name = name; }
            public void SetIndex(int index) { Index = index; }
        }
    
        EventHandler currentHandler = new EventHandler();
        int  counter = 0, parentcount = 3;

        int FindSFXGroupIndex(string searchName)
        {
            int index = -1;

            var grouplist = serializedObject.FindProperty("_sfxGroups");
            for(int groupIndex = 0; groupIndex < grouplist.arraySize; groupIndex++)
            {
                var groupName = grouplist.GetArrayElementAtIndex(groupIndex).FindPropertyRelative("groupName");
                if(groupName.stringValue == searchName)
                {
                    index = groupIndex;
                    break;
                }
            }

            return index;
        }

        int FindSFXIndexInGroup(string searchName, int groupIndex)
        {
            int index = -1;

            var grouplist = serializedObject.FindProperty("_sfxGroups").GetArrayElementAtIndex(groupIndex);
            var sfxlist   = grouplist.FindPropertyRelative("sfxList");
            for(int itemIndex = 0; itemIndex < sfxlist.arraySize; itemIndex++)
            {
                var sfxName = sfxlist.GetArrayElementAtIndex(itemIndex).FindPropertyRelative("sfxName");
                if(sfxName.stringValue == searchName)
                {
                    index = itemIndex;
                    break;
                }    
            }

            return index;
        }

        void OnSFXGroupSelect(ClickEvent ev, EventHandler eventHandler)
        {
            groupNameField.value = eventHandler.Name;
        }

        void OnSFXItemSelect(ClickEvent ev, EventHandler eventHandler)
        {
            currentHandler.SetUI(eventHandler.UI);
            currentHandler.SetName(eventHandler.Name);
            counter = parentcount;
        }

        void OnSFXItemDeSelect(ClickEvent ev)
        {
            if(counter > 0)
            {
                counter--;
                return;
            }

            currentHandler.SetUI(null);
        }

        void OnAddSFX(ClickEvent ev, EventHandler eventHandler)
        {
            //Get the group that trggered the event, then get the sfx list of that group
            int groupIndex = FindSFXGroupIndex(eventHandler.Name);
            var sfxGroup = serializedObject.FindProperty("_sfxGroups").GetArrayElementAtIndex(groupIndex);
            var sfxList  = sfxGroup.FindPropertyRelative("sfxList");

            //Create new SFX() at the end of the group
            sfxList.InsertArrayElementAtIndex(sfxList.arraySize);
            var sfx = sfxList.GetArrayElementAtIndex(sfxList.arraySize - 1);
            sfx.FindPropertyRelative("sfxName").stringValue = "New SFX";
            serializedObject.ApplyModifiedProperties(); //Actually apply the change (bruh)
            
            //Repaint doesnt fucking work
            RebuildListUI();

            //Unset currentHandler to prevent errors
            currentHandler.SetValue(null, "", -1);
        }

        void OnRemoveSFX(ClickEvent ev, EventHandler eventHandler)
        {
            //If item not selected, escape
            if(currentHandler.UI == null) return;

            //Get the group that trggered the event, then get the sfx list of that group
            int groupIndex = FindSFXGroupIndex(eventHandler.Name);
            var sfxGroup = serializedObject.FindProperty("_sfxGroups").GetArrayElementAtIndex(groupIndex);
            var sfxList  = sfxGroup.FindPropertyRelative("sfxList");
            if(sfxList.arraySize <= 0) return; //Empty list, escape

            //Remove current sfx item from list
            var sfxIndex = FindSFXIndexInGroup(currentHandler.Name, groupIndex);
            sfxList.DeleteArrayElementAtIndex(sfxIndex);
            serializedObject.ApplyModifiedProperties(); //Actually apply the change (bruh)

            //Repaint doesnt fucking work
            RebuildListUI();

            //Unset currentHandler to prevent errors
            currentHandler.SetValue(null, "", -1);
        }

        void OnChangeSFXName(InputEvent ev, EventHandler eventHandler)
        {
            eventHandler.UI.Q<Label>().text = ev.newData;
            serializedObject.ApplyModifiedProperties(); //Actually apply the change (bruh)
        }

        void OnAddSFXGroup(ClickEvent ev)
        {            
            var groupList = serializedObject.FindProperty("_sfxGroups");
            groupList.InsertArrayElementAtIndex(groupList.arraySize);
            var sfxGroup = groupList.GetArrayElementAtIndex(groupList.arraySize - 1);
            sfxGroup.FindPropertyRelative("groupName").stringValue = groupNameField.value;
            serializedObject.ApplyModifiedProperties(); //Actually apply the change (bruh)

            //Repaint doesnt fucking work
            RebuildListUI();
        }

        void OnRemoveSFXGroup(ClickEvent ev)
        {
            int groupIndex = FindSFXGroupIndex(groupNameField.value);
            if(groupIndex < 0) return; //No group found

            var groupList = serializedObject.FindProperty("_sfxGroups");
            groupList.DeleteArrayElementAtIndex(groupIndex);
            serializedObject.ApplyModifiedProperties(); //Actually apply the change (bruh)

            //Repaint doesnt fucking work
            RebuildListUI();
        }
    }
}