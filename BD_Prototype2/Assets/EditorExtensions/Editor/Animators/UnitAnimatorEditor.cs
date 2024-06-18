using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using BulletDance.Animation;

namespace BulletDance.Editor
{
    [CustomEditor(typeof(UnitAnimator), true)]
    public class UnitAnimatorEditor : UnityEditor.Editor
    {
        public VisualTreeAsset _inspectorXML;
        VisualElement inspector;

        List<string> _excludedProperties = new List<string>() { "m_Script", "_animType", "_animator", "_spriteAnimator", "_layeredAnimator", "_animationName" };

        public override VisualElement CreateInspectorGUI()
        {
            // Root of inspector UI
            inspector = new VisualElement();
            _inspectorXML.CloneTree(inspector);

            var unityAnim   = inspector.Q<ObjectField>("unity");
            unityAnim.BindProperty(serializedObject.FindProperty("_animator"));
            var spriteAnim  = inspector.Q<ObjectField>("sprite");
            spriteAnim.BindProperty(serializedObject.FindProperty("_spriteAnimator"));
            var layeredAnim = inspector.Q<ObjectField>("layered");
            layeredAnim.BindProperty(serializedObject.FindProperty("_layeredAnimator"));
            var _animType   = inspector.Q<DropdownField>();
            _animType.RegisterValueChangedCallback((ev) =>
                {
                    unityAnim.style.display   = DisplayStyle.None;
                    spriteAnim.style.display  = DisplayStyle.None;
                    layeredAnim.style.display = DisplayStyle.None;

                    if(ev.newValue == "Unity")
                        unityAnim.style.display = DisplayStyle.Flex;
                    if(ev.newValue == "Sprite")
                        spriteAnim.style.display = DisplayStyle.Flex;
                    if(ev.newValue == "Layered")
                        layeredAnim.style.display = DisplayStyle.Flex;
                }
            );
            _animType.BindProperty(serializedObject.FindProperty("_animType"));
            _animType.index = 0;

            //Find serialized properties
            SerializedProperty foundProperty = serializedObject.GetIterator();

            // GetIterator returns a root that is above all others
            // so first property is found by stepping into the children
            foundProperty.NextVisible(true);

            // For rest of scan stay at the same level (false)
            do
            {
                //Skip excluded, that one is handled with seperately
                if(_excludedProperties.Contains(foundProperty.name)) continue;

                //Create property field and add it to foldout
                PropertyField property = new PropertyField();
                property.BindProperty(foundProperty);
                property.label = EditorExt.FormatPropertyName(foundProperty.name);

                inspector.Add(property);
            }
            while (foundProperty.NextVisible(false));


            return inspector;
        }
    }

}