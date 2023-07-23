using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enemy 상태 관리 스크립트
/// </summary>
public class EnemyFSM : MonoBehaviour
{
    public Status _Status;

    Vector3 originPos;
    public GameObject hitEffect;
    public float moveDistance = 10.0f;  // 최대 움직일 수 있는 거리
    public float findDistance = 10.0f;  // 탐색 가능한 거리
    public float attackDistance = 1.0f; // 공격 가능 거리
    public float moveSpeed = 2.0f;      // 이동 속도
    public int attackDamage = 10;       // 공격 데미지
    public float hp;                    // 현재 체력
    public float maxHp;                 // 최대 체력

    float currentTime = 0;              // 공격 딜레이를 위한 시간
    float attackDelay = 4.0f;           // 공격 딜레이
    bool isDead;

    bool isStart = false;               // 게임 시작부터 움직이게

    public GameObject _player;
    Transform player;
    public Slider hpSlider;
    CapsuleCollider _capsuleCollider;
    Rigidbody _rigid;
    Weapon _weapon;

    Animator anim;
    Quaternion originRot;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _weapon = GetComponentInChildren<Weapon>();
        _weapon.damage = attackDamage;
    }

    void Start()
    {
        EventManager.InGameStart.AddListener(OnGameStart);

        _Status = Status.Idle;
        originPos = transform.position;
        originRot = transform.rotation;

        anim = transform.GetComponentInChildren<Animator>();
    }

    // 게임 시작 전엔 움직이지 않게
    private void OnGameStart(bool obj)
    {
        isStart = obj;
    }

    void Update()
    {
        if (!isStart) return;
        switch (_Status)
        {
            case Status.Idle:
                Idle();
                break;
            case Status.Move:
                Walk();
                break;
            case Status.Attack:
                Attack();
                break;
            case Status.Return:
                Return();
                break;
            case Status.Damaged:
                Damaged();
                break;
            case Status.Die:
                Die();
                break;
        }
    }

    // 일반 상태
    void Idle()
    {
        anim.SetBool("Walk", false);
        if (player == null) return;
        //플레이어가 탐색 범위에 들어오면 이동 상태로 전환
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            anim.SetTrigger("Walk");
            SetState(Status.Move);
        }
    }

    // 이동 상태
    void Walk()
    {
        anim.SetBool("Walk", true);
        if (player == null) return;
        Vector3 e_position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 p_position = new Vector3(player.position.x, 0.0f, player.position.z);

        // 이동 가능 범위보다 멀면 본래 위치로 이동
        if (Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            anim.SetTrigger("Walk");
            SetState(Status.Return);
        }

        // 이동 가능 범위면 공격하러 이동
        else if (Vector3.Distance(e_position, p_position) > attackDistance)
        {
            this.transform.LookAt(p_position);
            Vector3 dir = (player.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        // 공격 범위에 들어오면 공격상태로 전환
        else
        {
            currentTime = 2.5f;

            anim.SetBool("Walk", false);
            anim.SetTrigger("AttackDelay");
            SetState(Status.Attack);
        }
    }

    // 공격 상태
    void Attack()
    {
        if (player == null) return;
        Vector3 e_position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 p_position = new Vector3(player.position.x, 0.0f, player.position.z);

        // 공격 범위 안으로 들어오면 공격
        if (Vector3.Distance(e_position, p_position) < attackDistance)
        {
            this.transform.LookAt(p_position);
            // 공격 딜레이
            currentTime += Time.deltaTime;
            if (currentTime > attackDelay)
            {
                currentTime = 0;
                anim.SetTrigger("Attack");
                _weapon.Use();
            }
        }
        else if(anim.GetCurrentAnimatorStateInfo(0).IsName("AttackDelay"))
        {
            // 아니면 재탐색 혹은 추격
            currentTime = 0;
            anim.ResetTrigger("Attack");
            anim.ResetTrigger("AttackDelay");
            anim.SetBool("Walk", true);
            SetState(Status.Move);
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            currentTime = 0;
            anim.ResetTrigger("Attack");
            anim.ResetTrigger("AttackDelay");
            SetState(Status.Move);
        }
    }

    // 원래 위치로 돌아가기
    void Return()
    {
        anim.SetBool("Walk", true);
        if (Vector3.Distance(transform.position, originPos) > 0.2f)
        {
            this.transform.LookAt(originPos);
            Vector3 dir = (originPos - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = originPos;
            transform.rotation = originRot;
            
            hp = maxHp;
            SetState(Status.Idle);
            anim.SetTrigger("Idle");
        }
    }

    // 공격 당한 상태
    void Damaged()
    {
    }

    // 공격 받았을때
    IEnumerator DamagedProcess()
    {
        anim.ResetTrigger("AttackDelay");
        anim.ResetTrigger("Attack");
        anim.SetBool("Walk", false);
        anim.ResetTrigger("Damaged");
        anim.SetTrigger("Damaged");

        // 피격모션 시간만큼 잠깐 대기한다.
        yield return new WaitForSeconds(0.1f);
        SetState(Status.Idle);
    }

    // 죽음 상태
    void Die()
    {
        // 죽기 위한 상태처리
        if (!isDead)
        {
            // 죽었으니까 사용중인 코루틴이 있으면 중지
            StopAllCoroutines();
            StartCoroutine(DieProcess());
        }
        isDead = true;
    }

    // 죽음 프로세스
    IEnumerator DieProcess()
    {
        //print("사망");
        anim.SetTrigger("Dead");

        // 죽으면 통과할 수 있게
        _capsuleCollider.enabled = false;
        _rigid.useGravity = false;
        if (EventManager.UnRegistVModel.CanDispatch())
            EventManager.UnRegistVModel.Dispatch(this.gameObject);

        // 2초 기다렸다 자기자신 삭제
        yield return new WaitForSeconds(4.0f);
        Destroy(this.gameObject);
    }

    // 공격 받을때
    private void OnTriggerEnter(Collider other)
    {
        // 무기가 아니면 리턴
        if (other.name == _weapon.name) return;
        if (other.GetComponent<Weapon>() == null) return;

        _Status = Status.Damaged;

        hp -= other.GetComponent<Weapon>().damage;
        other.GetComponent<Weapon>().meleeArea.enabled = false;
        Debug.Log($"{other.name} 피격 : {hp}");
        if (hp > 0)
            StartCoroutine(DamagedProcess());
        else
            SetState(Status.Die);
    }

    // 상태 변환
    private void SetState(Status es)
    {
        if (_Status == es) return;
        _Status = es;
    }

    // hp 세팅
    public void SetHp(float hp, float maxHP)
    {
        this.hp = hp;
        this.maxHp = maxHP;
    }

    public float GetHp()
    {
        return this.hp;
    }
    public float GetMaxHp()
    {
        return this.maxHp;
    }

    // 공격할 플레이어 세팅
    public void SetPlayer(GameObject player)
    {
        _player = player;
        this.player = _player.transform;
    }
}
