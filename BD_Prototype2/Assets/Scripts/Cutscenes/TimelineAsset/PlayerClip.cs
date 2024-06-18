using UnityEngine;
using UnityEngine.Playables;
using BulletDance.Animation;


namespace BulletDance.Cutscene
{

    public class PlayerBehaviour : CharacterBehaviour
    {
        public bool faceTowardsBoss;
        public PlayerClip.Animation animState;
    }

    public class PlayerClip : CharacterClip
    {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            SetPlayableBehaviourType(typeof(PlayerBehaviour));
            return base.CreatePlayable(graph, owner);
        }

        protected override void SetPlayableBehaviourVariables<TPlayableBehaviour>(TPlayableBehaviour playableBehaviour)
        {
            base.SetPlayableBehaviourVariables<CharacterBehaviour>(playableBehaviour as CharacterBehaviour);
            SetVariables(playableBehaviour as PlayerBehaviour);
        }


        public bool faceTowardsBoss;

        public enum Animation { 
                Idle = 0, Walk = 1, Dash = 2, Attack = 3, 
                WakeUp = 10,
                SoulFormTransform = 30, SoulFormDetransform = 31,
                Defeat = 50 }; //Combining AnimAction and PlayerAnimAction
    
         public Animation animState;

            private void SetVariables(PlayerBehaviour playerBehaviour)
            {
                playerBehaviour.faceTowardsBoss = faceTowardsBoss;
                playerBehaviour.animState       = animState;
            }
    }

}