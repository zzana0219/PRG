using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;

/// <summary>
/// �ùķ��̼� �Ŵ���
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

    // �� ���μ��� �߰�
    void AddProcessor()
    {
        //_mProcessors.Add(new PMove());
        _mProcessors.Add(new PHPController());
        _mProcessors.Add(new PStage(PStage.Stage.One));
        //_mProcessors.Add(new PPlayerController());
    }

    // �� �߰�
    private void OnRegistModel(IModel obj)
    {
        _models.Add(obj);
        obj.OnInit();
        EventManager.CreateModel.Dispatch(obj);
    }

    // �� ����
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

    // ���� ���۽�
    public void OnGameStart(bool obj)
    {
        gameStart = obj;
        viewManager.GameStart(obj);
    }
}
