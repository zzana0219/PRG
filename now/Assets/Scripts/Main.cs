using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 메인 스크립트
/// </summary>
public class Main : MonoBehaviour
{
    SimulationManager simulationManager;
    ViewManager viewManager;

    CancellationTokenSource cts;

    int targetFrame = 5;
    float targetMultiply = 1;
    float targetDeltaTime => 1.0f / targetFrame;

    private void Awake()
    {
        simulationManager = new();
    }

    private void Start()
    {
        GameStart();
    }

    // 게임 플레이
    public void GameStart()
    {
        //if (cts != null)
        //{
        //    Debug.LogWarning("Game이 이미 실행 중입니다.");
        //    return;
        //}
        //cts = new CancellationTokenSource();
        Debug.Log("GameStart");
        StartCoroutine(UpdateGame());
        //cts.Token.Register(() =>
        //{
        //    Debug.Log("CancellationToken Quit");
        //});
        //GameInit(cts.Token).ContinueWith(t => { Debug.Log("GameDone"); });
    }

    /*
    async Task GameInit(CancellationToken cancellationToken)
    {
        await Task.Delay(1, cancellationToken: cancellationToken);

        //await Task.Factory.StartNew(async () =>
        //{
        //    await GamePlayAsync(cancellationToken);
        //});
    }
    */

    // 게임 업데이트 프로세스
    IEnumerator UpdateGame()
    {
        while (true)
        {
            yield return null;
            simulationManager.Update();
        }
    }

    //async Task GamePlayAsync(CancellationToken cancellationToken)
    //{
    //    while (!cancellationToken.IsCancellationRequested)
    //    {
    //        Debug.Log("천천히");
    //        // 여기서부터 다시 찾기
    //        simulationManager.Update();
    //        viewManager.Update();
    //        await Task.Delay((int)/*(targetDeltaTime / targetMultiply) **/ 1000, cancellationToken: cancellationToken);
    //    }
    //}
}
