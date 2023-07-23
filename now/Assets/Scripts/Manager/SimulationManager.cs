using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;

/// <summary>
/// 시뮬레이션 매니저
/// </summary>
public class SimulationManager
{
    private readonly List<IMProcessor> _mProcessors = new();
    private readonly List<IModel> _models = new();

    CreateModel createModel;
    ViewManager viewManager;
    bool gameStart = false;

    public SimulationManager()
    {
        createModel = new();
        viewManager = new();
        EventManager.GameStart.AddListener(OnGameStart);
        EventManager.RegistModel.AddListener(OnRegistModel);
        EventManager.UnRegistModel.AddListener(OnUnRegistModel);
        AddProcessor();
    }

    // 각 프로세서 추가
    void AddProcessor()
    {
        //_mProcessors.Add(new PMove());
        _mProcessors.Add(new PHPController());
        _mProcessors.Add(new PStage(PStage.Stage.One));
        //_mProcessors.Add(new PPlayerController());
    }

    // 모델 추가
    private void OnRegistModel(IModel obj)
    {
        _models.Add(obj);
        obj.OnInit();
        EventManager.CreateModel.Dispatch(obj);
    }

    // 모델 삭제
    private void OnUnRegistModel(IModel obj)
    {
        _models.Remove(obj);
    }

    public void Update()
    {
        if (!gameStart) return;

        foreach (var processor in _mProcessors)
            processor.Update(_models);
        viewManager.Update();
    }

    // 게임 시작시
    public void OnGameStart(bool obj)
    {
        gameStart = obj;
        viewManager.GameStart(obj);
    }
}
