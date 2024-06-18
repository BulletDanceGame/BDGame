using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BulletDance.Editor
{

    [CustomPropertyDrawer(typeof(EnvironmentConductor.SequenceGroup))]
    public class SequenceGroupDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            //FUCK WHY UNITY
            var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/MusicConductors/SequenceGroupDrawer.uxml");

            var root = new VisualElement();
            xml.CloneTree(root);

            //Fix bindings
            var main = root.Q<Foldout>("main");

            var name = root.Q<TextField>("name");
            name.BindProperty(property.FindPropertyRelative("name"));
            name.RegisterCallback<InputEvent>((InputEvent ev) => { main.text = ev.newData; });

            var type = root.Q<DropdownField>("type");
            type.BindProperty(property.FindPropertyRelative("type"));

            var firstSeq = root.Q<TextField>("firstSeq");
            firstSeq.BindProperty(property.FindPropertyRelative("sequencePlayedFirst"));

            var neverFirst = root.Q<Toggle>("neverFirst");
            neverFirst.BindProperty(property.FindPropertyRelative("neverPlayFirstSequenceAgain"));

            var msg = root.Q<Label>("seqMsg");

            var addItem = root.Q<Button>("addItem");

            var removeItem = root.Q<Button>("removeItem");

            //Sequences
            SerializedProperty items = property.FindPropertyRelative("sequences");

            for (int i = 0; i < items.arraySize; i++)
            {
                SerializedProperty foundProperty = items.GetArrayElementAtIndex(i);

                PropertyField field = new PropertyField();
                field.BindProperty(foundProperty);
                field.label = EditorExt.FormatPropertyName(foundProperty.name);
                main.Add(field);
            }




            return root;
        }
    }

}
