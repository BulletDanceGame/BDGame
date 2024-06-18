using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BulletDance.Animation
{


public class BossCritterAnimator : BossAnimator
{
    protected override void WalkStart()
    { 
        _isWalk = true;

        //Play a faster move animation if the movement happens offbeat.

        if(_offbeatDuration > 100f) return; //Not offbeat (no beat or offbeat is smaller than minimum duration)

        float speed = 1f / GetDuration(_offbeatDuration, _animNoteDuration);
        if(speed > 4f) return; //The animation is too fast, don't animate

        _spriteAnimator.SetSpeed(speed);
        _spriteAnimator.Anim(AnimAction.Walk);
    }
}


}
