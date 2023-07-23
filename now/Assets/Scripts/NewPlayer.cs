using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;

/// <summary>
/// 플레이어 상태 관리를 위한 스크립트
/// </summary>
public class NewPlayer : MonoBehaviour
{
    [SerializeField] Status _playerState;
    [SerializeField] float hp;
    [SerializeField] float maxHp;

    Animator anim;

    public GameObject hitEffect;
    public GameObject _mainCamera;

    bool isStart = false;

    public float speed = 5.0f;          // 이동속도
    public float runningSpeed = 7.5f;   // 달리기속도
    private bool isRunning = false;     // 달리는가

    public float jumpPower = 1.0f;      // 점프파워
    private float v;                    // 앞뒤 속도
    private float h;                    // 좌우 속도
    private bool isAttack;              // 재공격햇는가?
    private bool isSkill;               // 스킬 사용중인가?
    private bool isJump;                // 점프햇는가?
    private bool isDoubleJump;          // 이단 점프
    private bool isAttacked = false;

    private bool isDie = false;

    public int attackDamage = 50;
    public float attackRange;

    // 스킬3을 위한 변수
    private bool skill3Start = false;
    private bool isUp = false;
    private bool isDown = false;
    private bool skill3End = false;

    [SerializeField] private Weapon weapon = null;
    [SerializeField] private Vector3 MoveDir;
    [SerializeField] private GameObject _camera;

    private string _name;

    public bool IsAttack { get => isAttack; set => isAttack = value; }
    public bool Skill3End { get => skill3End; set => skill3End = value; }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.InGameStart.AddListener(OnGameStart);

        var model = this.transform.Find("Model");

        anim = model.GetComponent<Animator>();
        weapon = model.GetComponentInChildren<Weapon>();
        weapon.damage = attackDamage;

        _playerState = Status.Idle;

        isAttack = false;
        _camera = GameObject.Find("Main Camera");
        _camera.transform.SetParent(transform);
    }

    private void OnGameStart(bool obj)
    {
        isStart = obj;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStart) return;
        if(isDie) return;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SetState(Status.Damaged);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))    // 이동
        {
            if (_playerState == Status.Attack) return;
            if (_playerState == Status.Skill1) return;
            if (_playerState == Status.Skill2) return; 
            if (_playerState == Status.Skill3) return;
            if (_playerState == Status.Jump) return;

            //if (isAttack) return;
            //if (isSkill) return;
            //if (isJump) return;
            anim.SetBool("Walk", true);
            SetState(Status.Move);
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))    // 이동멈추기
        {
            if (_playerState == Status.Attack) return;
            if (_playerState == Status.Skill1) return;
            if (_playerState == Status.Skill2) return;
            if (_playerState == Status.Skill3) return;
            if (_playerState == Status.Jump) return;

            //if (isAttack) return;
            //if (isSkill) return;
            //if (isJump) return;
            if (isRunning) isRunning = false;
            SetState(Status.Idle);
        }

        if (Input.GetKeyDown(KeyCode.Space)) // 점프
        {
            if (_playerState == Status.Attack) return;
            if (_playerState == Status.Skill1) return;
            if (_playerState == Status.Skill2) return;
            if (_playerState == Status.Skill3) return;

            //if (isAttack) return;
            //if (isSkill) return;

            if (isJump)
            {
                Debug.Log($"한번");
                isDoubleJump = true;
                return;
            }
            SetState(Status.Jump);
            anim.SetTrigger("Jump");
            StartCoroutine(JumpProcess());
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))    // 공격
        {
            if (_playerState == Status.Skill1) return;
            if (_playerState == Status.Skill2) return;
            if (_playerState == Status.Skill3) return;

            //if (isSkill) return;
            isAttack = true;

            if (_playerState == Status.Jump) { anim.SetTrigger("JumpAttack"); return; }
            //if (isJump) { anim.SetTrigger("JumpAttack"); return; }
            anim.SetTrigger("Attack");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) // 스킬 1
        {
            anim.SetTrigger("Skill1");
            isAttack = false;
            isSkill = true;
            SetState(Status.Skill1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // 스킬 2
        {
            anim.SetTrigger("Skill2");
            isAttack = false;
            isSkill = true;
            SetState(Status.Skill2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // 스킬 3
        {
            anim.SetTrigger("Skill3");
            isAttack = false;
            isSkill = true;
            SetState(Status.Skill3);
        }

        //if (_playerState == Status.Move || _playerState == Status.Jump)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");
        }

        switch (_playerState)
        {
            case Status.Idle:
                Idle(h);
                break;
            case Status.Move:
                Move(v, h);
                break;
            case Status.Attack:
                Attack();
                break;
            case Status.Skill1:
            case Status.Skill2:
                Skill();
                break;
            case Status.Skill3:
                Skill3();
                break;
            case Status.Damaged:
                Damaged();
                break;
            case Status.Jump:
                Jump(v, h);
                break;
            case Status.Die:
                Die();
                break;
        }
    }

    // 상태 변환
    public void SetState(Status ps)
    {
        if (_playerState == ps) return;
        _playerState = ps;
        //print(_playerState);
    }

    // 사망 상태
    private void Die()
    {
        if (isDie) return;
        isDie = true;
    }

    // 일반 상태
    private void Idle(float h)
    {
        this.transform.Rotate(90 * h * Time.deltaTime * Vector3.up);

        if (isAttack) isAttack = false;
        if (isSkill) isSkill = false;
        if (isJump) isJump = false;
        if (skill3Start) skill3Start = false;
        if (isAttacked) isAttacked = false;
        anim.SetBool("Walk", false);
        anim.SetBool("Run", false);
    }

    // 이동 상태
    private void Move(float v, float h)
    {
        if (isAttack) isAttack = false;
        if (isSkill) isSkill = false;

        // 달리기 경우 변환
        if (Input.GetKey(KeyCode.LeftShift))
        { anim.SetBool("Run", true); isRunning = true; }
        else
        { anim.SetBool("Run", false); isRunning = false; }

        //바라보는 시점 방향으로 이동
        if (isRunning)
        {
            this.transform.Translate(runningSpeed * Time.deltaTime * v * Vector3.forward);
            this.transform.Rotate(90 * h * Time.deltaTime * Vector3.up);
        }
        else
        {
            this.transform.Translate(speed * Time.deltaTime * v * Vector3.forward);
            this.transform.Rotate(90 * h * Time.deltaTime * Vector3.up);
        }
    }

    // 점프 상태
    private void Jump(float v, float h)
    {
        if (!isJump) isJump = true;
        Move(v, h);

        if (isUp)
            this.transform.Translate(20 * Time.deltaTime * Vector3.up);
        if (isDown)
            this.transform.Translate(20 * Time.deltaTime * Vector3.down);
    }

    // 점프 프로세스
    public IEnumerator JumpProcess()
    {
        if (isJump) yield break;
        isJump = true;

        isUp = true;
        yield return new WaitUntil(() => this.transform.position.y >= 8);   // 공중 위치까지 플레이어 올리기
        if (isDoubleJump)
        {
            anim.SetTrigger("JumpSpin");
            yield return new WaitUntil(() => this.transform.position.y >= 16);   // 공중 위치까지 플레이어 올리기
        }
        isUp = false;
        isDown = true;
        yield return new WaitUntil(() => this.transform.position.y <= 0);   // 땅 위치까지 플레이어 내리기
        isDown = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);    // 혹시 모를 예외처리

        isDoubleJump = false;
        isJump = false;

        // 해당부분 버그 좀 있음 나중에 고치기
        if (v <= 0.5f && v >= -0.5f)
        {
            anim.SetTrigger("JumpToIdle");
        }
        else
        {
            if (isRunning)
            {
                anim.SetTrigger("JumpToRun");
                anim.SetBool("Walk", true);
                anim.SetBool("Run", true);
            }
            else
            {
                anim.SetTrigger("JumpToMove");
                anim.SetBool("Walk", true);
            }
            SetState(Status.Move);
        }
    }

    // 공격 상태
    private void Attack()
    {
        if (!isAttack)
            isAttack = true;

        weapon.Use();
    }

    // 스킬1, 2 상태
    private void Skill()
    {
        if (!isSkill)
            isSkill = true;
        weapon.Use();
    }

    // 스킬3 상태
    private void Skill3()
    {
        if (!isSkill)
            isSkill = true;

        if (isUp)
            this.transform.Translate(20 * Time.deltaTime * Vector3.up);
        if (isDown)
            this.transform.Translate(20 * Time.deltaTime * Vector3.down);
        weapon.Use();
    }

    // 스킬3 프로세스 시작
    public void StartSkill3()
    {
        if (skill3Start) return;
        StartCoroutine(Skill3Process());
    }

    // 스킬3 프로세스
    public IEnumerator Skill3Process()
    {
        skill3Start = true; // 중복 예외처리
        skill3End = false;
        GetComponent<Rigidbody>().useGravity = false;
        yield return new WaitForSeconds(0.2f);
        isUp = true;
        yield return new WaitUntil(() => this.transform.position.y >= 8);   // 공중 위치까지 플레이어 올리기
        anim.SetTrigger("Skill3Start"); 
        isUp = false;
        yield return new WaitUntil(() => Skill3End == true);    // 마지막 공격까지 기다리기

        isDown = true;
        yield return new WaitUntil(() => this.transform.position.y <= 0);   // 땅 위치까지 플레이어 내리기
        isDown = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);    // 혹시 모를 예외처리
        anim.SetTrigger("Skill3End");
        skill3Start = false;
        Skill3End = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    // 피격 상태
    private void Damaged()
    {
    }

    // 피격 프로세스
    IEnumerator DamagedProcess()
    {
        if (hp >= 0)
        {
            anim.SetTrigger("Attacked");
            yield return new WaitForSeconds(0.5f);
            isAttacked = false;
            SetState(Status.Idle);
        }
        else
        {
            anim.SetTrigger("Die");
            SetState(Status.Die);
            EventManager.GameEnd.Dispatch(false);
        }
    }

    // 공격 받는 타이밍?
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == weapon.name) return;
        if (other.GetComponent<Weapon>() == null) return;

        hp -= other.GetComponent<Weapon>().damage;
        other.GetComponent<Weapon>().meleeArea.enabled = false;

        if (hp > 0)
        {
            // 스킬류는 슈퍼아머 판정을 위해
            if (_playerState == Status.Skill1) return;
            if (_playerState == Status.Skill2) return;
            if (_playerState == Status.Skill3) return;
            StartCoroutine(DamagedProcess());
        }
        else
            SetState(Status.Die);
    }

    // 체력 세팅
    public void SetHp(float hp, float maxHP)
    {
        this.hp = hp;
        this.maxHp = maxHP;
    }

    public float GetHp() { return this.hp; }
    public float GetMaxHp() { return this.maxHp; }
}
