using UnityEngine;
using UnityEditor;
using System;

namespace BulletDance.Editor
{
    // ---------------- IMPORTANT ---------------- //
    // --  Create the ScriptableObject OUTSIDE of the Editor folder!! -- //

    public class EditorPrefabContainer : ScriptableObject
    {
        #if UNITY_EDITOR

        public GameObject roomTemplate;
        public GameObject roomLayerTemplate;
        public GameObject musicConductorPrefab;

        #endif
    }
}