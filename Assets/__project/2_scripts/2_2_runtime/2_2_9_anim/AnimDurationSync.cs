using UnityEngine;

public class AnimDurationSync : StateMachineBehaviour
{
    public string ParamTargetDuration = "TargetDuration";
    public string ParamSpeedMultiplier = "SpeedMultiplier";
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float targetDuration = animator.GetFloat(ParamTargetDuration);
        
        if (targetDuration > 0.001f)
        {
            float Speed = stateInfo.length / targetDuration;
            
            animator.SetFloat(ParamSpeedMultiplier, Speed);
        }
        else
        {
            // 만약 목표 시간이 0 이하로 잘못 들어왔다면 기본 속도(1)로 처리
            animator.SetFloat(ParamSpeedMultiplier, 1f);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
