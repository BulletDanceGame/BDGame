using UnityEngine;
using UnityEngine.Playables;
using BulletDance.Animation;

namespace BulletDance.Cutscene
{

//Binds Serialized objects at runtime && in editor
//Makes saving scenes less harmful
[ExecuteAlways]
public class CutsceneBinder : MonoBehaviour
{
    public static PlayerAnimator PlayerAnimatorInstance { get; private set; } = null;
    public static BossAnimator   BossAnimatorInstance   { get; private set; } = null;
    public static CutsceneEvents CutsceneEventsInstance { get; private set; } = null;

    void Start()
    {
        PlayerAnimatorInstance = GameObject.FindObjectOfType<PlayerAnimator>(true);
        if(PlayerAnimatorInstance == null)
            Debug.Log("Could not find PlayerAnimator, Player cutscene track will not play");

        BossAnimatorInstance = GameObject.FindObjectOfType<BossAnimator>(true);
        if(BossAnimatorInstance == null)
            Debug.Log("Could not find BossAnimator, Boss cutscene track will not play");
        
        CutsceneEventsInstance = GameObject.FindObjectOfType<CutsceneEvents>(true);


        var _cutscenes = GetComponentsInChildren<PlayableDirector>(); 

        foreach(var cutscene in _cutscenes)
        {
            foreach(var playableAssetOutput in cutscene.playableAsset.outputs)
            {
                if(playableAssetOutput.streamName == "Cinemachine Track")
                {
                    cutscene.SetGenericBinding(playableAssetOutput.sourceObject, Camera.main.GetComponent<Cinemachine.CinemachineBrain>());
                }

                ///Example binding
                // if(playableAssetOutput.streamName == "(Name) Track")
                // {
                //     cutscene.SetGenericBinding(playableAssetOutput.sourceObject, ObjectToBind);
                // }
            }
        }
    }
}

}