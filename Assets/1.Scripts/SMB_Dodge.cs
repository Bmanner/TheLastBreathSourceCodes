using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_Dodge : StateMachineBehaviour {

    PlayerMovement playerMoveCon;
    PlayerAttackController playerAttackCon;

    void Awake()
    {
        var player = GameObject.FindWithTag("Player");
        playerMoveCon = player.GetComponent<PlayerMovement>();
        playerAttackCon = player.GetComponent<PlayerAttackController>();
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        Debug.Log("닷지 애니메이션 스테이트 진입");
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    if(stateInfo.normalizedTime >= 0.87f)
        {
            //Debug.Log(stateInfo.normalizedTime);
            animator.applyRootMotion = false;
            playerMoveCon.IsDodging = false;
            playerAttackCon.enabled = true;
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    //animator.applyRootMotion = false;
        playerMoveCon.DodgeIsCool = false;
        //playerAttackCon.enabled = true;
    }

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
