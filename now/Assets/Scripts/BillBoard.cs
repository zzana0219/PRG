using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 적 체력 UI를 위한 빌보드 스크립트
/// </summary>
public class BillBoard : MonoBehaviour
{
    public Transform target;

    public float hp;
    public float maxHp;
    Slider slider;

    private void Start()
    {
        slider = this.transform.Find("Slider").GetComponent<Slider>();
    }

    // 체력 세팅
    public void SetHp(float hp, float maxHp)
    {
        if (target == null)
        {
            if (GameObject.Find("Player") != null)
                target = GameObject.Find("Player").transform;
            return;
        }
        transform.forward = target.forward;

        if (slider == null) return;
        slider.value = hp / maxHp;
        this.hp = hp;
        this.maxHp = maxHp;
    }
}
