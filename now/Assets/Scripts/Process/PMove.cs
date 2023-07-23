using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;

/// <summary>
/// 이동 프로세스 (현재 사용 X)
/// </summary>
public class PMove : IMProcessor
{
    public PMove()
    {
        //EventManager.Move.AddListener(OnMove);
    }

    private void OnMove(Model model)
    {
        switch (model.Status)
        {
            case Status.Idle:
                //model.
                break;
            case Status.Move:
                break;
            case Status.Attack:
                break;
            case Status.Skill1:
                break;
            case Status.Skill2:
                break;
            case Status.Skill3:
                break;
            case Status.Damaged:
                break;
            case Status.Jump:
                break;
            case Status.Die:
                break;
            default:
                break;
        }
    }

    public void Update(List<IModel> models)
    {
        foreach (var model in models) 
        {
            if (model is Model movable)
                OnMove(movable);
        }
    }
}
