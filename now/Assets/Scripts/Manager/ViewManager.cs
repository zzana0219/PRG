using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static EventManager;

/// <summary>
/// UI 및 뷰 매니저 스크립트
/// </summary>
public class ViewManager
{
    public static ViewManager Instance { private set; get; }
    public GameObject UIObject { get => uiObject; set => uiObject = value; }
    public Image PlayerHp { get => playerHp; set => playerHp = value; }
    public Slider BossHp { get => bossHp; set => bossHp = value; }
    public Image PlayerMp { get => playerMp; set => playerMp = value; }
    public List<Image> EnemyHp { get => enemyHp; set => enemyHp = value; }
    public GameObject BossObj { get => bossObj; set => bossObj = value; }

    private readonly Dictionary<IModel, VModel> models = new();
    private GameObject uiObject;
    private Transform enemyHpObject;
    private GameObject playerObj;
    private Image playerHp;
    private Image playerMp;
    private Slider bossHp;
    private GameObject bossObj;


    private List<Image> enemyHp;

    public ViewManager()
    {
        if (Instance == null)
            Instance = this;
        else
            return;

        EventManager.RegistVModel.AddListener(OnRegistModel);
        EventManager.UnRegistVModel.AddListener(OnUnRegistModel);
        EventManager.GameEnd.AddListener(OnGameEnd);

        uiObject = GameObject.Instantiate(Resources.Load<GameObject>("UI/Game UI"));
        playerHp = uiObject.transform.Find("InGamePanel").Find("Status").Find("Status Bar").Find("HpBar").Find("Fill").GetComponent<Image>();
        playerMp = uiObject.transform.Find("InGamePanel").Find("Status").Find("Status Bar").Find("MpBar").Find("Fill").GetComponent<Image>();
        enemyHpObject = uiObject.transform.Find("InGamePanel").Find("EnemyHp");
        bossObj = uiObject.transform.Find("InGamePanel").Find("Boss").gameObject;
        BossHp = uiObject.transform.Find("InGamePanel").Find("Boss").Find("BossHpBar").GetComponent<Slider>();
    }

    // 모델 소환시 각 Camp에 따라 필요한 정보 추가
    private void OnRegistModel((IModel, VModel) obj)
    {
        models.Add(obj.Item1, obj.Item2);

        if (obj.Item1 is not Model model) return;

        // 플레이어일 경우
        if (model.CampType == Model.Camp.Player)
        {
            playerObj = obj.Item2.Object;
            playerObj.AddComponent<NewPlayer>().SetHp(model.HP, model.MaxHP);
        }

        // 적 혹은 보스일 경우
        if (model.CampType == Model.Camp.Enemy || model.CampType == Model.Camp.Boss)
        {
            obj.Item2.Object.AddComponent<EnemyFSM>();
            if (obj.Item2.Object.transform.Find("EnemyHpBar") != null)
            {
                var bar = obj.Item2.Object.transform.Find("EnemyHpBar");
                bar.gameObject.SetActive(true);
                bar.GetComponent<BillBoard>().enabled = true;
                obj.Item2.billBoard = bar.GetComponent<BillBoard>();
                obj.Item2.Object.GetComponent<EnemyFSM>().SetHp(model.HP, model.MaxHP);
            }
            if (model.CampType == Model.Camp.Boss)
            {
                bossObj.SetActive(true);
            }
        }
        CheckPlayer();
    }

    // 적 진영에 플레이어 적용 -> 추후 변경 필요
    private void CheckPlayer()
    {
        if (playerObj == null) return;

        foreach (var item in models)
        {
            if (item.Key is not Model model) continue;
            if (model.CampType == Model.Camp.Ally) continue;
            if (model.CampType == Model.Camp.Player) continue;
            if (model.CampType == Model.Camp.neutral) continue;

            item.Value.Object.GetComponent<EnemyFSM>().SetPlayer(playerObj);
        }
    }

    private void OnUnRegistModel(GameObject obj)
    {
        foreach (var item in models)
        {
            if (item.Value.Object == obj)
            {
                CheckModelUI();
                models.Remove(item.Key);
                if (EventManager.UnRegistModel.CanDispatch())
                    EventManager.UnRegistModel.Dispatch(item.Key);
                break;
            }
        }
    }

    private void OnGameEnd(bool obj)
    {
        var stage = uiObject.transform.Find("InGamePanel").Find("Stage");
        var text = stage.Find("Text").GetComponent<Text>();
        stage.gameObject.SetActive(true);

        if (obj)
        {
            text.text = $"You Win";
        }
        else
        {
            text.text = $"You Lose";
        }
    }

    public void Update()
    {
        CheckModelUI();
    }

    private void CheckModelUI()
    {
        foreach (var item in models)
        {
            if (item.Key is Model model)
            {
                if (model.CampType == Model.Camp.Player)
                {
                    var hp = item.Value.Object.GetComponent<NewPlayer>().GetHp();
                    model.HP = hp;
                }
                if (model.CampType == Model.Camp.Enemy || model.CampType == Model.Camp.Boss)
                {
                    var hp = item.Value.Object.GetComponent<EnemyFSM>().GetHp();
                    var maxHp = item.Value.Object.GetComponent<EnemyFSM>().GetMaxHp();
                    item.Value.billBoard.SetHp(hp, maxHp);
                    model.HP = hp;
                }
            }
        }
    }

    // 게임 시작
    public async void GameStart(bool isStart)
    {
        if (!isStart) return;
        await GameReady();
    }

    // 게임 준비
    async Task GameReady()
    {
        var stage = uiObject.transform.Find("InGamePanel").Find("Stage");
        var text = stage.Find("Text").GetComponent<Text>();
        stage.gameObject.SetActive(true);
        text.text = $"Ready\n3";
        await Task.Delay(1000);
        text.text = $"Ready\n2";
        await Task.Delay(1000);
        text.text = $"Ready\n1";
        await Task.Delay(1000);
        text.text = $"Start";
        await Task.Delay(1000);
        stage.gameObject.SetActive(false);
        EventManager.InGameStart.Dispatch(true);
    }
}
