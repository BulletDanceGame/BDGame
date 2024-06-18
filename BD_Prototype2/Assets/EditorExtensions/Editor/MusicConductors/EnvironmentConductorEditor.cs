using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BulletDance.Editor
{
    [CustomEditor(typeof(EnvironmentConductor), true)]
    public class EnvironmentConductorEditor : UnityEditor.Editor
    {
        private EnvironmentConductorDrawer _zoneDrawer = new EnvironmentConductorDrawer();
        public override VisualElement CreateInspectorGUI()
        {
            _zoneDrawer.Init(null);
            _zoneDrawer.SetZone(serializedObject);
            return _zoneDrawer.Redraw();
        }
    }
}