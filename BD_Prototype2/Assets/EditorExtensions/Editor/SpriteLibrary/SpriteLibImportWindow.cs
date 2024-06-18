using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;

using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace BulletDance.GrapicsEditor
{


public class SpriteLibImportWindow : EditorWindow
{
    // -- Open/Refresh the window -- //
    [MenuItem("Tools/Graphics/Import to Sprite Library")]
    public static void ShowEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        EditorWindow wnd = GetWindow<SpriteLibImportWindow>();
        wnd.titleContent = new GUIContent("Sprite Library Importer");

        // Limit size of the window
        wnd.minSize = new Vector2(640, 480);
        wnd.maxSize = new Vector2(1920, 1080);
    }

    // -- Window creation -- //

    public  VisualTreeAsset _windowXML;
    public  SpriteLibraryAsset _genericAsset;
    private VisualElement  _root;
    private Label _log;
    private SpritLibImportOptions    _importOptions = new SpritLibImportOptions();
    private SpriteLibImportDragDrop  _assetHandler  = new SpriteLibImportDragDrop();

    public void CreateGUI()
    {
        // Root of UI
        _root = new VisualElement();
        _root.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
        _windowXML.CloneTree(_root);

        _importOptions.Init(_root);
        _assetHandler.Init(_root);

        _log = _root.Q<Label>("logmsg");

        var confirm = _root.Q<Button>("import");
        confirm.clicked += ImportAssets;

        rootVisualElement.Add(_root);
    }

    public void OnDestroy()
    {
        _assetHandler.target.RemoveManipulator(_assetHandler);
    }

    void ImportAssets()
    {
        if(_importOptions.spriteLibAsset == null)
        {
            _log.text = "Sprite Library Asset is currently null, cannot import sprites";
            return;
        }

        int i = 0;

        //Create a new sprite library asset from cache and copy the referenced asset
        var asset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();
        List<string> catergories = _importOptions.spriteLibAsset.GetCategoryNames().ToList();
        foreach(string catergory in catergories)
        {
            List<string> labels = _importOptions.spriteLibAsset.GetCategoryLabelNames(catergory).ToList();
            foreach(string label in labels)
            {
                //If the label and catergory matches from import options
                //then instead of copying the old sprites, use the imported sprite
                if(catergory == _importOptions.animGroup && label == ((int)(_importOptions.slotStart + i)).ToString())
                {
                    if(i < _assetHandler.spriteList.Count)
                    {
                        asset.AddCategoryLabel(_assetHandler.spriteList[i], catergory, label);
                        i++;
                        continue;
                    }
                }

                //else copy the sprite
                asset.AddCategoryLabel(_importOptions.spriteLibAsset.GetSprite(catergory, label), catergory, label);
            }
        }

        //Save the asset baby
        SpriteLibraryAssetHelper.SaveAsSpriteLibrarySourceAsset(asset, AssetDatabase.GetAssetPath(_importOptions.spriteLibAsset), _genericAsset);
        AssetDatabase.Refresh();
        _assetHandler.ClearAssets();

        _log.text = "Successfully imported sprites";
    }

}


}