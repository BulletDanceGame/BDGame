using System;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(RoomController))]
public class RoomControllerEditor : Editor
{
    public static SerializedProperty copied;

    public override VisualElement CreateInspectorGUI()
    {
        var xml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorExtensions/Editor/RasmusTesting/RoomController.uxml");

        Debug.Log(xml);

        VisualElement root = new VisualElement();
        xml.CloneTree(root);


        EnumField roomType = root.Q<EnumField>("roomType");
        roomType.BindProperty(serializedObject.FindProperty("_roomType"));
        roomType.RegisterCallback((ChangeEvent<Enum> ev) => {
            DrawRoomTypeVariables(root, serializedObject,
            serializedObject.FindProperty("_roomType").enumValueIndex);
        });


        PropertyField environmentConductors = root.Q<PropertyField>("environmentConductors");
        environmentConductors.BindProperty(serializedObject.FindProperty("_environmentConductors"));


        //killroom
        PropertyField waves = root.Q<PropertyField>("waves");
        waves.BindProperty(serializedObject.FindProperty("_waves"));


        //survival room
        IntegerField durationInBeats = root.Q<IntegerField>("durationInBeats");
        durationInBeats.BindProperty(serializedObject.FindProperty("_durationInBeats"));

        PropertyField initialEnemies = root.Q<PropertyField>("initialEnemies");
        initialEnemies.BindProperty(serializedObject.FindProperty("_initialEnemies"));

        PropertyField spawnableEnemies = root.Q<PropertyField>("spawnableEnemies");
        spawnableEnemies.BindProperty(serializedObject.FindProperty("_spawnableEnemies"));

        Toggle randomizeSpawning = root.Q<Toggle>("randomizeSpawning");
        randomizeSpawning.BindProperty(serializedObject.FindProperty("_randomizeSpawning"));

        IntegerField enemiesKilledBeforeSpawning = root.Q<IntegerField>("enemiesKilledBeforeSpawning");
        enemiesKilledBeforeSpawning.BindProperty(serializedObject.FindProperty("_enemiesKilledBeforeSpawn"));

        IntegerField enemiesSpawnedEachTime = root.Q<IntegerField>("enemiesSpawnedEachTime");
        enemiesSpawnedEachTime.BindProperty(serializedObject.FindProperty("_enemiesSpawnedEachTime"));

        PropertyField timerUIPrefab = root.Q<PropertyField>("timerUIPrefab");
        timerUIPrefab.BindProperty(serializedObject.FindProperty("_timerUIPrefab"));


        //kill and survival room
        PropertyField gatesToLock = root.Q<PropertyField>("gatesToLock");
        gatesToLock.BindProperty(serializedObject.FindProperty("_gatesToLock"));

        PropertyField spawnMarkerPrefab = root.Q<PropertyField>("spawnMarkerPrefab");
        spawnMarkerPrefab.BindProperty(serializedObject.FindProperty("_spawnMarkerPrefab"));

        PropertyField enemySpawnSFX = root.Q<PropertyField>("enemySpawnSFX");
        enemySpawnSFX.BindProperty(serializedObject.FindProperty("_enemySpawnSFX"));

        PropertyField vfxSmokePrefab = root.Q<PropertyField>("vfxSmokePrefab");
        vfxSmokePrefab.BindProperty(serializedObject.FindProperty("_vfxSmokePrefab"));

        //roaming
        PropertyField enemies = root.Q<PropertyField>("enemies");
        enemies.BindProperty(serializedObject.FindProperty("_enemies"));


        //general
        PropertyField startBattleSFX = root.Q<PropertyField>("startBattleSFX");
        startBattleSFX.BindProperty(serializedObject.FindProperty("_startBattleSFX"));
        PropertyField endBattleSFX = root.Q<PropertyField>("endBattleSFX");
        endBattleSFX.BindProperty(serializedObject.FindProperty("_endBattleSFX"));



        return root;
    }

    public void DrawRoomTypeVariables(VisualElement root, SerializedObject serializedObject, int enumIndex)
    {

        GroupBox roamingArea = root.Q<GroupBox>("roamingArea");
        GroupBox killroom = root.Q<GroupBox>("killRoom");
        GroupBox survivalRoom = root.Q<GroupBox>("survivalRoom");
        GroupBox killAndSurvivalRoom = root.Q<GroupBox>("killAndSurvivalRoom");


        roamingArea.style.display = (enumIndex == 0) ? DisplayStyle.Flex : DisplayStyle.None;
        killroom.style.display = (enumIndex == 1) ? DisplayStyle.Flex : DisplayStyle.None;
        survivalRoom.style.display = (enumIndex == 2) ? DisplayStyle.Flex : DisplayStyle.None;
        killAndSurvivalRoom.style.display = (enumIndex == 1 || enumIndex == 2) ? DisplayStyle.Flex : DisplayStyle.None;

    }

}
