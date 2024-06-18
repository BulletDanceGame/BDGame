using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.LevelBuilder
{


public class LevelBuilderImportWindow : EditorWindow
{
    // -- Open/Refresh the window -- //
    [MenuItem("Tools/Import to Level Builder")]
    public static void ShowEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<LevelBuilderImportWindow>();
        wnd.titleContent = new GUIContent("Level Builder Importer");

        // Limit size of the window
        wnd.minSize = new Vector2(450, 390);
        wnd.maxSize = new Vector2(1920, 1080);
    }

    void OnProjectChange()
    {
        PaletteHandler.Instance.LoadPalettes();
        _importOptions.UpdatePaletteChoices();
        _importOptions.UpdatePalettePreview();
    }

    // -- Window creation -- //

    public  VisualTreeAsset _windowXML, _itemTempXML;
    private VisualElement  _root;
    private ImportOptions _importOptions = new ImportOptions();
    private AssetHandler  _assetHandler  = new AssetHandler();
    private ImportHandler _importHandler = new ImportHandler();

    public void CreateGUI()
    {
        // Root of UI
        _root = new VisualElement();
        _windowXML.CloneTree(_root);

        _importOptions.Init(_root);

        _assetHandler.Init(_root, _itemTempXML);

        _importHandler.Init(_root, _assetHandler, _importOptions);

        rootVisualElement.Add(_root);
    }

    public void OnDestroy()
    {
        _assetHandler.target.RemoveManipulator(_assetHandler);
    }
}


}