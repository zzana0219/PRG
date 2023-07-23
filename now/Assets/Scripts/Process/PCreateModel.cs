using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 인게임 모델 소환 스크립트
/// </summary>
public class CreateModel
{
    private List<IModel> datas = new();

    public CreateModel()
    {
        EventManager.CreateModel.AddListener(OnCreateModel);
    }

    private void OnCreateModel(IModel model)
    {
        foreach (var data in datas)
            if (data == model) return;
        datas.Add(model);

        GameObject modelGo = (GameObject)GameObject.Instantiate(Resources.Load(model.Route));
        modelGo.name = model.Name;
        modelGo.transform.position = model.Position;

        //_datas.Add(model, new VHuman(modelGo));
        if(model is Human)
            EventManager.RegistVModel.Dispatch((model, new VHuman(modelGo)));

        if(model is Skeleton)
            EventManager.RegistVModel.Dispatch((model, new VSkeleton(modelGo)));
    }
}