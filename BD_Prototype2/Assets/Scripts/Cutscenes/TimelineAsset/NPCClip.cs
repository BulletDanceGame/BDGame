using UnityEngine;
using UnityEngine.Playables;
using BulletDance.Animation;


namespace BulletDance.Cutscene
{

public class NPCBehaviour : CharacterBehaviour
{
    public bool showGraphics;
    public bool faceTowardsPlayer;
    public NPCClip.Animation animState;
}

public class NPCClip : CharacterClip
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        SetPlayableBehaviourType(typeof(NPCBehaviour));
        return base.CreatePlayable(graph, owner);
    }

    protected override void SetPlayableBehaviourVariables<TPlayableBehaviour>(TPlayableBehaviour playableBehaviour)
    {
        base.SetPlayableBehaviourVariables<CharacterBehaviour>(playableBehaviour as CharacterBehaviour);
        SetVariables(playableBehaviour as NPCBehaviour);
    }


    public bool showGraphics = true;
    public bool faceTowardsPlayer;

    public enum Animation { 
        Idle   = 0,  Walk = 1, Dash = 2, Attack = 3, 
        Intro  = 30, PhaseDefeat = 31, PhaseChange = 32,
        Defeat = 50 }; //Combining AnimAction and NPCAnimAction
    public Animation animState;

    private void SetVariables(NPCBehaviour NPCBehaviour)
    {
        if(NPCBehaviour == null) return;
        NPCBehaviour.showGraphics = showGraphics;
        NPCBehaviour.faceTowardsPlayer = faceTowardsPlayer;
        NPCBehaviour.animState = animState;
    }
}

}