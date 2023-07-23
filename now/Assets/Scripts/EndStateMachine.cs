using System;
using UnityEditor;
using UnityEngine;

public class EndStateMachine : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        if (stateInfo.IsTag("AttackEnd"))
        {
            animator.applyRootMotion = false;

            // 공격이 끝나면 Idle로 돌아감 코드 수정 중
            if (animator.transform.parent.GetComponent<NewPlayer>() != null)
            {
                var player = animator.transform.parent.GetComponent<NewPlayer>();
                //player.SetState(Status.Idle);
            }
        }

        if (stateInfo.IsTag("Skill3PointEnd"))
        {
            animator.applyRootMotion = false;
            //animator.gameObject
            Debug.Log("Skill3PointEnd");

            // 공격이 끝나면 Idle로 돌아감
            if (animator.transform.parent.GetComponent<NewPlayer>() != null)
            {
                var player = animator.transform.parent.GetComponent<NewPlayer>();
                player.SetState(Status.Idle);
            }
        }
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (animator.transform.parent.GetComponent<NewPlayer>() == null) return;
        var player = animator.transform.parent.GetComponent<NewPlayer>();
        CheckStatus(stateInfo, player);

        if (stateInfo.IsTag("Skill3End"))
            player.Skill3End = true;
    }

    // 애니메이션 상태에 따라 상태 변환
    private void CheckStatus(AnimatorStateInfo stateInfo, NewPlayer player)
    {
        if (stateInfo.IsTag(Status.Idle.ToString()))
        {
            player.SetState(Status.Idle);
        }

        if (stateInfo.IsTag(Status.Attack.ToString()))
        {
            player.SetState(Status.Attack);
        }

        if (stateInfo.IsTag(Status.Skill1.ToString()))
        {
            player.SetState(Status.Skill1);
        }

        if (stateInfo.IsTag(Status.Skill2.ToString()))
        {
            player.SetState(Status.Skill2);
        }

        if (stateInfo.IsTag(Status.Skill3.ToString()))
        {
            player.SetState(Status.Skill3);
            player.StartSkill3();
        }
    }
}