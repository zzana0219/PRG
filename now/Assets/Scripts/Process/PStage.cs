using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지 프로세스 스크립트
/// </summary>
public class PStage : IMProcessor
{
    bool stageStart = false;
    Stage nowStage;
    public bool gameOver = false;

    // 스테이지 구성
    public enum Stage
    {
        One,
        Two,
        Boss,
        Clear,
    }

    public PStage(Stage stage)
    {
        stageStart = true;
        nowStage = stage;
        
        if (stage == Stage.One)
        {
            EventManager.RegistModel.Dispatch(new Human("Player", new Vector3(0, 0, 0), Model.Camp.Player));
            for (int i = 0; i < 5; i++)
                EventManager.RegistModel.Dispatch(new Skeleton($"Enemy{i}", new Vector3(5 * i, 0, 5), Model.Camp.Enemy));
            EventManager.GameStart.Dispatch(true);
        }
    }

    public void Update(List<IModel> models)
    {
        if (!stageStart) return;
        if (CheckStage(models)) return;
        
        nowStage++;
        SettingNextStage();
    }

    // 다음 스테이지 세팅
    private void SettingNextStage()
    {
        switch (nowStage)
        {
            case Stage.One:
                break;
            case Stage.Two:
                for (int i = 0; i < 5; i++)
                    EventManager.RegistModel.Dispatch(new Skeleton($"Enemy{i}", new Vector3(5 * i, 0, 10), Model.Camp.Enemy));
                EventManager.GameStart.Dispatch(true);
                break;
            case Stage.Boss:
                EventManager.RegistModel.Dispatch(new Skeleton($"Boss", new Vector3(15, 0, 20), Model.Camp.Boss, 150));
                EventManager.GameStart.Dispatch(true);
                break;
            case Stage.Clear:
                gameOver = true;
                EventManager.GameEnd.Dispatch(true);
                break;
            default:
                break;
        }
    }

    // 스테이지 내 보스 혹은 적이 없으면 다음 스테이지 시작
    private bool CheckStage(List<IModel> models)
    {
        if (nowStage == Stage.Clear) return true;

        foreach (var item in models)
        {
            if (item is Model model)
                if (model.CampType == Model.Camp.Enemy || model.CampType == Model.Camp.Boss)
                    return true;
        }
        return false;
    }
}
