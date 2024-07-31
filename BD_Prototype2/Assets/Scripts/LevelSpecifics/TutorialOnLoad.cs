using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletDance.Animation;
using UnityEngine.Timeline;
using UnityEngine.Playables;


namespace BulletDance.Cutscene
{
    public class TutorialOnLoad : MonoBehaviour
    {

        public GameObject cutscene1trigger;
        public GameObject cutscene2trigger;

        [Tooltip("For debugging, should be false in build")]
        public bool dontMovePlayerOnStart;
        // Start is called before the first frame update
        void Start()
        {
            cutscene1trigger.SetActive(true);
            cutscene1trigger.SetActive(true);
            if (dontMovePlayerOnStart == false)
            {
                Invoke("PlayerPos", 1.5f);
            }
        }

        private void PlayerPos()
        {
            

            Vector3 Checkpointposition;

            Checkpointposition.x = SaveSystem.Instance.GetData().currentCheckpointX;
            Checkpointposition.y = SaveSystem.Instance.GetData().currentCheckpointY;
            Checkpointposition.z = SaveSystem.Instance.GetData().currentCheckpointZ;


            UnitManager.Instance.GetPlayer().transform.position = Checkpointposition;
        }

        private void Update()
        {
            PlayerAnimator player = CutsceneBinder.PlayerAnimatorInstance;

            if (SaveSystem.Instance.GetData().hasBat)
            {
                PlayerAnimator.PlayerSpriteSet spriteSet = PlayerAnimator.PlayerSpriteSet.Default;
                player.SetSpriteSet(spriteSet);
            }
            else
            {
                PlayerAnimator.PlayerSpriteSet spriteSet = PlayerAnimator.PlayerSpriteSet.NoBat;
                player.SetSpriteSet(spriteSet);

            }
            if (cutscene1trigger)
            {
                if (SaveSystem.Instance.GetData().hasplayed1stcutscene)
                {
                    cutscene1trigger.SetActive(false);
                }
            }
            if (cutscene2trigger)
            {
                if (SaveSystem.Instance.GetData().hasplayed2stcutscene)
                {
                    cutscene2trigger.SetActive(false);
                }
            }


        }
    }

}
