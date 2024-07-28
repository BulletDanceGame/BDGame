using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using BulletDance.Animation;
using UnityEngine.InputSystem.Interactions;


namespace BulletDance.Cutscene
{

    //Base classes for animating units/boss/player in cutscene
    //Do not use these directly, they are meant to be added on/overriden

    public class CharacterBehaviour : PlayableBehaviour
    {
        public bool enableUpdate;
        public Direction direction;
        public GameObject faceTowardsTarget;
        public Vector2 moveVelocity;
    }

    public class CharacterClip : PlayableAsset
    {
        protected Type playableBehaviourType = typeof(CharacterBehaviour);

        public bool enableUpdate;
        public Direction direction;
        public GameObject faceTowardsTarget;
        public Vector2 moveVelocity;

        /// <summary>
        /// For derived implementation, set the clip behaviour type before calling base
        ///    SetPlayableBehaviourType(typeof(NameBehaviour));
        ///    return base.CreatePlayable(graph, owner);
        /// </summary>
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            //Very copy pasty bc of local variables and Generic types 

            if(playableBehaviourType == typeof(PlayerBehaviour)) 
            {
                ScriptPlayable<PlayerBehaviour> playable = ScriptPlayable<PlayerBehaviour>.Create(graph);
                PlayerBehaviour playableBehaviour = playable.GetBehaviour();
                SetPlayableBehaviourVariables<PlayerBehaviour>(playableBehaviour);
                return playable;
            }

            else if(playableBehaviourType == typeof(BossBehaviour)) 
            {
                ScriptPlayable<BossBehaviour> playable = ScriptPlayable<BossBehaviour>.Create(graph);
                BossBehaviour playableBehaviour = playable.GetBehaviour();
                SetPlayableBehaviourVariables<BossBehaviour>(playableBehaviour);
                return playable;
            }

            else
            {
                Debug.Log("PlayableBehaviourType does not match, defaulting to base CharacterBehaviour. May cause errors");
                ScriptPlayable<CharacterBehaviour> playable = ScriptPlayable<CharacterBehaviour>.Create(graph);
                CharacterBehaviour playableBehaviour = playable.GetBehaviour();
                SetPlayableBehaviourVariables<CharacterBehaviour>(playableBehaviour);
                return playable;
            }
        }

        protected void SetPlayableBehaviourType(Type type)
        {
            playableBehaviourType = type;
        }

        /// <summary>
        /// For derived implementation, cast playableBehaviour to correct type for correct behaviour
        ///    base.SetPlayableBehaviourVariables[CharacterBehaviour](playableBehaviour as CharacterBehaviour); //Triangle ones breaks the summary lol
        ///    ImplementMethod(playableBehaviour as CorrectType)
        /// </summary>
        protected virtual void SetPlayableBehaviourVariables<TPlayableBehaviour>(TPlayableBehaviour playableBehaviour)
        {
            SetCommonVariables(playableBehaviour as CharacterBehaviour);
        }

        private void SetCommonVariables(CharacterBehaviour playableBehaviour)
        {
            playableBehaviour.enableUpdate = enableUpdate;
            playableBehaviour.direction    = direction;
            playableBehaviour.faceTowardsTarget = faceTowardsTarget;
            playableBehaviour.moveVelocity = moveVelocity;
        }
    }

    public class CharacterTrackMixer : PlayableBehaviour
    {
        // -- Check if the bindings and types are correct -- //
        protected bool isBindingCorrect = false;
        protected UnitAnimator characterAnim = null;
        protected virtual void ValidatePlayableBinding()
        {
            characterAnim = null;
            isBindingCorrect = characterAnim != null;
        }

        protected bool isBehaviourTypeCorrect = false;
        protected Type playableBehaviourType = typeof(CharacterBehaviour);
        protected void SetPlayableBehaviourType(Type type)
        {
            playableBehaviourType = type;
            if(playableBehaviourType == typeof(CharacterBehaviour)) 
                Debug.Log("Failed to set the correct PlayableBehaviour type, cannot assign playable behaviour variables");
        
            isBehaviourTypeCorrect = playableBehaviourType != typeof(CharacterBehaviour);
        }

        /// <summary>
        /// For derived implementation, set the clip behaviour type before calling base
        ///    SetPlayableBehaviourType(typeof(NameBehaviour));
        ///    base.ProcessFrame(playable, info, playerData);
        /// </summary>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ValidatePlayableBinding();

            if (!isBindingCorrect) return;

            if(!isBehaviourTypeCorrect) return;

            //Get variables set in inspector
            GetInspectorData(playable);

            //Set & Display
            SetAndDisplay(playable, info);
        }


        private void GetInspectorData(Playable playable)
        {
            for (int i = 0; i < playable.GetInputCount(); i++)
            {
                if (playable.GetInputWeight(i) > 0f)
                {
                    //Very copy pasty bc of local variables and Generic types 

                    if(playableBehaviourType == typeof(PlayerBehaviour)) 
                    {
                        ScriptPlayable<PlayerBehaviour> inputPlayable = (ScriptPlayable<PlayerBehaviour>)playable.GetInput(i);
                        PlayerBehaviour input = inputPlayable.GetBehaviour();
                        SetVariablesFromInspector<PlayerBehaviour>(input);
                    }

                    else if(playableBehaviourType == typeof(BossBehaviour)) 
                    {
                        ScriptPlayable<BossBehaviour> inputPlayable = (ScriptPlayable<BossBehaviour>)playable.GetInput(i);
                        BossBehaviour input = inputPlayable.GetBehaviour();
                        SetVariablesFromInspector<BossBehaviour>(input);
                    }
                }
            }
        }

        /// <summary>
        /// For derived implementation, cast playableBehaviour to correct type for correct behaviour
        ///    base.SetVariablesFromInspector[CharacterBehaviour](input as CharacterBehaviour); //Triangle ones breaks the summary lol
        ///    ImplementMethod(input as CorrectType)
        /// </summary>
        protected virtual void SetVariablesFromInspector<TTPlayableBehaviour>(TTPlayableBehaviour input)
        {
            SetCommonVariables(input as CharacterBehaviour);
        }

        protected bool enableUpdate  = true;
        protected bool requestUpdate = true;
        private void SetCommonVariables(CharacterBehaviour input)
        {
            bool canUpdate = input.enableUpdate;
            requestUpdate = (canUpdate != enableUpdate);
            if(requestUpdate) enableUpdate = canUpdate;

            direction = input.direction;
            faceTowardsTarget = input.faceTowardsTarget;
            moveVelocity = input.moveVelocity;
        }


        /// <summary>
        /// For derived implementation, implement your stuff before calling base
        ///    /* Code implementation */
        ///    base.SetAndDisplay(playable);
        /// </summary>
        protected virtual void SetAndDisplay(Playable playable, FrameData frameData)
        {
            if(requestUpdate)
                characterAnim?.EnableAnimUpdate(enableUpdate);
        
            Move();
            SetDirection();
            Animate();

            //Idk what how time parameter is used, the Unity code doc and source code won't explain
            //So enjoy the characters hyperventilating in edit mode
            if(EditorCheck.inEditMode)
            {
                if(frameData.deltaTime <= 0)
                    characterAnim?.UpdateInEditor((float)playable.GetTime());

                else
                    characterAnim?.UpdateInEditor(frameData.deltaTime);
            }
        }

        protected Direction  direction = Direction.Front;
        protected GameObject faceTowardsTarget = null;
        protected Vector2    moveVelocity = Vector2.zero;
        protected virtual void SetDirection()
        {
            if(faceTowardsTarget != null)
                characterAnim?.FaceTowards(faceTowardsTarget);
            else
                characterAnim?.FaceTowards(direction);
        }

        protected virtual void Animate()
        {
            characterAnim?.AnimState(0);
        }

        protected Rigidbody2D rbd = null;
        protected virtual void Move()
        {
            if(rbd == null)
                rbd = characterAnim.transform.parent.GetComponentInChildren<Rigidbody2D>();

            if(rbd != null)
                rbd.velocity = moveVelocity;
        }
    }

}