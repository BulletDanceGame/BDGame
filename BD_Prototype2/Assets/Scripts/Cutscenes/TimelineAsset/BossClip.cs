using UnityEngine;
using UnityEngine.Playables;
using BulletDance.Animation;


namespace BulletDance.Cutscene
{

public class BossBehaviour : CharacterBehaviour
{
    public bool showGraphics;
    public bool faceTowardsPlayer;
    public int  bossphase = 1;
    public BossClip.Animation animState;
}

public class BossClip : CharacterClip
{
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        SetPlayableBehaviourType(typeof(BossBehaviour));
        return base.CreatePlayable(graph, owner);
    }

    protected override void SetPlayableBehaviourVariables<TPlayableBehaviour>(TPlayableBehaviour playableBehaviour)
    {
        base.SetPlayableBehaviourVariables<CharacterBehaviour>(playableBehaviour as CharacterBehaviour);
        SetVariables(playableBehaviour as BossBehaviour);
    }


    public bool showGraphics = true;
    public bool faceTowardsPlayer;
    public int  bossphase = 1;

    public enum Animation { 
        Idle   = 0,  Walk = 1, Dash = 2, Attack = 3, 
        Intro  = 30, PhaseDefeat = 31, PhaseChange = 32,
        Defeat = 50 }; //Combining AnimAction and BossAnimAction
    public Animation animState;

    private void SetVariables(BossBehaviour bossBehaviour)
    {
        bossBehaviour.showGraphics = showGraphics;
        bossBehaviour.faceTowardsPlayer = faceTowardsPlayer;
        bossBehaviour.bossphase = bossphase;
        bossBehaviour.animState = animState;
    }
}

}