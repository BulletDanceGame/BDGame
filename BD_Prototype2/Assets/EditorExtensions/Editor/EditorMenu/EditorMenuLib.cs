using UnityEngine;
using UnityEditor;
using System;

namespace BulletDance.Editor
{
    // Taken from here : https://manuel-rauber.com/2022/05/23/instantiate-your-own-prefabs-via-gameobject-menu/
    public class EditorMenuLib
    {
        private const string _prefabContainerPath = "Assets/EditorExtensions/EditorPrefabContainer.asset";
        private static EditorPrefabContainer LocatePrefabContainer() => 
            AssetDatabase.LoadAssetAtPath<EditorPrefabContainer>(_prefabContainerPath);

        private const int MenuPriority = -50;

        [MenuItem("GameObject/LevelBuilder/Room", priority = MenuPriority)]
        private static void CreateRoom()
        {
            var instance = SafeInstantiate(prefabContainer => prefabContainer.roomTemplate);

            //Name the room by the room count in the scene
            var foundRooms = GameObject.FindGameObjectsWithTag("Room");
            instance.name = "Room " + (foundRooms.Length);
        }


        public static GameObject InstantiateGameObject(Func<EditorPrefabContainer, GameObject> itemSelector)
        {
            return SafeInstantiate(itemSelector);
        }

        public static GameObject InstantiatePrefab(Func<EditorPrefabContainer, GameObject> itemSelector)
        {
            return SafeInstantiatePrefab(itemSelector);
        }


        private static GameObject SafeInstantiate(Func<EditorPrefabContainer, GameObject> itemSelector)
        {
            var prefabContainer = LocatePrefabContainer();

            if (!prefabContainer)
            {
                Debug.LogWarning($"PrefabContainerer not found at path {_prefabContainerPath}");
                return null;
            }

            var item = itemSelector(prefabContainer);
            var instance  = GameObject.Instantiate(item, Selection.activeTransform) as GameObject;

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;

            return instance;
        }

        private static GameObject SafeInstantiatePrefab(Func<EditorPrefabContainer, GameObject> itemSelector)
        {
            var prefabContainer = LocatePrefabContainer();

            if (!prefabContainer)
            {
                Debug.LogWarning($"PrefabContainerer not found at path {_prefabContainerPath}");
                return null;
            }

            var item = itemSelector(prefabContainer);
            var instance = PrefabUtility.InstantiatePrefab(item) as GameObject;

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;

            return instance;
        }
    }
}