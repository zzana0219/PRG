using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EventManager;

/// <summary>
/// �÷��̾� ���� ������ ���� ��ũ��Ʈ
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

    public float speed = 5.0f;          // �̵��ӵ�
    public float runningSpeed = 7.5f;   // �޸���ӵ�
    private bool isRunning = false;     // �޸��°�

    public float jumpPower = 1.0f;      // �����Ŀ�
    private float v;                    // �յ� �ӵ�
    private float h;                    // �¿� �ӵ�
    private bool isAttack;              // ������޴°�?
    private bool isSkill;               // ��ų ������ΰ�?
    private bool isJump;                // �����޴°�?
    private bool isDoubleJump;          // �̴� ����
    private bool isAttacked = false;

    private bool isDie = false;

    public int attackDamage = 50;
    public float attackRange;

    // ��ų3�� ���� ����
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

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))    // �̵�
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

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))    // �̵����߱�
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

        if (Input.GetKeyDown(KeyCode.Space)) // ����
        {
            if (_playerState == Status.Attack) return;
            if (_playerState == Status.Skill1) return;
            if (_playerState == Status.Skill2) return;
            if (_playerState == Status.Skill3) return;

            //if (isAttack) return;
            //if (isSkill) return;

            if (isJump)
            {
                Debug.Log($"�ѹ�");
                isDoubleJump = true;
                return;
            }
            SetState(Status.Jump);
            anim.SetTrigger("Jump");
            StartCoroutine(JumpProcess());
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0))    // ����
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

        if (Input.GetKeyDown(KeyCode.Alpha1)) // ��ų 1
        {
            anim.SetTrigger("Skill1");
            isAttack = false;
            isSkill = true;
            SetState(Status.Skill1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // ��ų 2
        {
            anim.SetTrigger("Skill2");
            isAttack = false;
            isSkill = true;
            SetState(Status.Skill2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // ��ų 3
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

    // ���� ��ȯ
    public void SetState(Status ps)
    {
        if (_playerState == ps) return;
        _playerState = ps;
        //print(_playerState);
    }

    // ��� ����
    private void Die()
    {
        if (isDie) return;
        isDie = true;
    }

    // �Ϲ� ����
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

    // �̵� ����
    private void Move(float v, float h)
    {
        if (isAttack) isAttack = false;
        if (isSkill) isSkill = false;

        // �޸��� ��� ��ȯ
        if (Input.GetKey(KeyCode.LeftShift))
        { anim.SetBool("Run", true); isRunning = true; }
        else
        { anim.SetBool("Run", false); isRunning = false; }

        //�ٶ󺸴� ���� �������� �̵�
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

    // ���� ����
    private void Jump(float v, float h)
    {
        if (!isJump) isJump = true;
        Move(v, h);

        if (isUp)
            this.transform.Translate(20 * Time.deltaTime * Vector3.up);
        if (isDown)
            this.transform.Translate(20 * Time.deltaTime * Vector3.down);
    }

    // ���� ���μ���
    public IEnumerator JumpProcess()
    {
        if (isJump) yield break;
        isJump = true;

        isUp = true;
        yield return new WaitUntil(() => this.transform.position.y >= 8);   // ���� ��ġ���� �÷��̾� �ø���
        if (isDoubleJump)
        {
            anim.SetTrigger("JumpSpin");
            yield return new WaitUntil(() => this.transform.position.y >= 16);   // ���� ��ġ���� �÷��̾� �ø���
        }
        isUp = false;
        isDown = true;
        yield return new WaitUntil(() => this.transform.position.y <= 0);   // �� ��ġ���� �÷��̾� ������
        isDown = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);    // Ȥ�� �� ����ó��

        isDoubleJump = false;
        isJump = false;

        // �ش�κ� ���� �� ���� ���߿� ��ġ��
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

    // ���� ����
    private void Attack()
    {
        if (!isAttack)
            isAttack = true;

        weapon.Use();
    }

    // ��ų1, 2 ����
    private void Skill()
    {
        if (!isSkill)
            isSkill = true;
        weapon.Use();
    }

    // ��ų3 ����
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

    // ��ų3 ���μ��� ����
    public void StartSkill3()
    {
        if (skill3Start) return;
        StartCoroutine(Skill3Process());
    }

    // ��ų3 ���μ���
    public IEnumerator Skill3Process()
    {
        skill3Start = true; // �ߺ� ����ó��
        skill3End = false;
        GetComponent<Rigidbody>().useGravity = false;
        yield return new WaitForSeconds(0.2f);
        isUp = true;
        yield return new WaitUntil(() => this.transform.position.y >= 8);   // ���� ��ġ���� �÷��̾� �ø���
        anim.SetTrigger("Skill3Start"); 
        isUp = false;
        yield return new WaitUntil(() => Skill3End == true);    // ������ ���ݱ��� ��ٸ���

        isDown = true;
        yield return new WaitUntil(() => this.transform.position.y <= 0);   // �� ��ġ���� �÷��̾� ������
        isDown = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);    // Ȥ�� �� ����ó��
        anim.SetTrigger("Skill3End");
        skill3Start = false;
        Skill3End = false;
        GetComponent<Rigidbody>().useGravity = true;
    }

    // �ǰ� ����
    private void Damaged()
    {
    }

    // �ǰ� ���μ���
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

    // ���� �޴� Ÿ�̹�?
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == weapon.name) return;
        if (other.GetComponent<Weapon>() == null) return;

        hp -= other.GetComponent<Weapon>().damage;
        other.GetComponent<Weapon>().meleeArea.enabled = false;

        if (hp > 0)
        {
            // ��ų���� ���۾Ƹ� ������ ����
            if (_playerState == Status.Skill1) return;
            if (_playerState == Status.Skill2) return;
            if (_playerState == Status.Skill3) return;
            StartCoroutine(DamagedProcess());
        }
        else
            SetState(Status.Die);
    }

    // ü�� ����
    public void SetHp(float hp, float maxHP)
    {
        this.hp = hp;
        this.maxHp = maxHP;
    }

    public float GetHp() { return this.hp; }
    public float GetMaxHp() { return this.maxHp; }
}
