using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 관련 스크립트
/// </summary>
public class Weapon : MonoBehaviour
{
    public enum Type
    {
        Melee,  // 근접 공격
        Range,  // 원거리 공격
    };

    public Type type;
    public int damage;
    public float rate;
    public BoxCollider meleeArea;
    public TrailRenderer traileffect;
    public bool isAttack = false;

    public void Use()
    {
        // 근접 무기일 경우
        if (type == Type.Melee)
        {
            StartCoroutine(Swing());
        }
    }

    // 공격시 콜라이더 및 이펙트 사용
    IEnumerator Swing()
    {
        if(isAttack) yield break;
        isAttack = true;
        meleeArea.enabled = true;
        if (traileffect != null)
            traileffect.enabled = true;

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = false;

        if (traileffect != null)
            traileffect.enabled = false;

        isAttack = false;
    }
}
