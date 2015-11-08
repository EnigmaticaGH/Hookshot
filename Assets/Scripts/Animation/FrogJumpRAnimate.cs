using UnityEngine;
using System.Collections;

public class FrogJumpRAnimate : StateMachineBehaviour
{
    private float animTime;

    override public void OnStateUpdate(Animator anim, AnimatorStateInfo animInfo, int layerIndex)
    {
        animTime = animInfo.normalizedTime;
        if (Input.GetAxis("Horizontal") < 0)
        {
            anim.Play("Jump AnimationL", 0, animTime);
        }
    }
}
