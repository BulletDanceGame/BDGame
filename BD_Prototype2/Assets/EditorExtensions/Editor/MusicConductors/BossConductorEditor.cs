using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BulletDance.Editor
{
    [CustomEditor(typeof(BossConductor), true)]
    public class BossConductorEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            SerializedProperty foundProperty = serializedObject.GetIterator();
            foundProperty.NextVisible(true);

            do
            {
                //Skip sfxGroups, that one is handled separately
                if(foundProperty.name == "m_Script") continue;

                PropertyField property = new PropertyField();
                property.BindProperty(foundProperty);
                property.label = EditorExt.FormatPropertyName(foundProperty.name);

                root.Add(property);
            }
            while (foundProperty.NextVisible(false));

            return root;
        }
    }
}