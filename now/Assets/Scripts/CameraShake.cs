using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 피격시 화면 흔들림을 위한 스크립트
/// </summary>
public class CameraShake : MonoBehaviour
{
    public float ShakeAmount;       // 카메라 흔드는 힘
    float ShakeTime;                // 카메라 흔들리는 시간
    Vector3 initialPosition;        // 카메라 흔들리는 위치
    public GameObject _player;      // 카메라가 보는 대상
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            VibrateForTime(0.2f);
        }

        initialPosition = _player.transform.position;

        if (ShakeTime > 0)
        {
            this.transform.position = Random.insideUnitSphere * ShakeAmount + initialPosition;
            ShakeTime -= Time.deltaTime;
        }
        else
        {
            this.transform.position = initialPosition;
            ShakeTime = 0.0f;
        }
    }

    public void VibrateForTime(float time)
    {
        ShakeTime = time;
    }
}
