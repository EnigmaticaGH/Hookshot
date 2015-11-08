using UnityEngine;
using System.Collections;

public class FrogJumpLAnimate : StateMachineBehaviour
{
    private float animTime;

    override public void OnStateUpdate(Animator anim, AnimatorStateInfo animInfo, int layerIndex)
    {
        animTime = animInfo.normalizedTime;
        if (Input.GetAxis("Horizontal") > 0)
        {
            anim.Play("Jump AnimationR", 0, animTime);
        }
    }
}
