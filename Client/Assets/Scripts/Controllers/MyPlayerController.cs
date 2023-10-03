using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class MyPlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Jump,
        Attack,
        Dead,
    }

    public enum MoveDir
    {
        Up,
        Down,
        Left,
        Right,
    }

    // TODO: speed ���� �������� �� ���ֱ�
    float _moveSpeed = 5.0f; // �̵��ӵ�
    float _ag = 0.4f; // �߷� ���ӵ� (������ 1.0f�� ����)
    float _jumpSpeed = 17.0f; // ������ (������ 1.0f�� ����)

    Animator _animator;
    bool _movePressed = false;
    Coroutine _coroutine = null;

    Vector3 _speedVec = new Vector3(0, 0, 0); // �ӵ�
    PlayerState _state = PlayerState.Idle;
    public PlayerState State
    {
        get { return _state; }
        set
        {
            if (_state == value)
                return;

            _state = value;
            UpdateAnimation();
        }
    }
    MoveDir _dir = MoveDir.Left;
    public MoveDir Dir
    {
        get { return _dir; }
        set
        {
            if (_dir == value)
                return;

            _dir = value;
            if (value == MoveDir.Left)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
            UpdateAnimation();
        }
    }

    void Start()
    {
        Init();
    }

    void Init()
    {
        _animator = Util.FindChild(gameObject, "UnitRoot", false).GetComponent<Animator>();
        State = PlayerState.Idle;
        Dir = MoveDir.Right;
    }

    void Update()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(transform.position);
            Debug.Log(Managers.Map.FindClosePos(transform.position));
            Debug.Log(Managers.Map.SceneToArr(Managers.Map.FindClosePos(transform.position)));
        }

        // ���±�� Update ����
        UpdateController();
        
        //�̵� ����
        ApplyMove();

        // ���� ����
        ApplyPhysic();
    }

    void UpdateAnimation()
    {
        if (_animator == null)
            return;

        if (State == PlayerState.Idle)
        {
            _animator.Play("Idle");
        }
        else if (State == PlayerState.Walk)
        {
            _animator.Play("Walk");
        }
        else if (State == PlayerState.Jump)
        {
            _animator.Play("Jump");
        }
        else if (State == PlayerState.Attack)
        {
            _animator.Play("Attack");
        }
        else if (State == PlayerState.Dead)
        {
            _animator.Play("Dead");
        }
    }

    void UpdateController()
    {
        // ���� �Է�
        switch (State)
        {
            case PlayerState.Idle:
            case PlayerState.Walk:
            case PlayerState.Jump:
                GetDirInput();
                break;
        }

        // ���±�� Update
        switch (State)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;

            case PlayerState.Walk:
                UpdateWalk();
                break;

            case PlayerState.Jump:
                UpdateJump();
                break;

            case PlayerState.Attack:
                UpdateAttack();
                break;

            case PlayerState.Dead:
                UpdateDead();
                break;
        }
    }

    void GetDirInput()
    {
        _movePressed = true;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            _movePressed = false;
        }
    }

    void UpdateIdle()
    {
        // ���� ���� ����
        if (Input.GetKey(KeyCode.A))
        {
            State = PlayerState.Attack;
            return;
        }

        // ���� ����
        if (Input.GetKey(KeyCode.Space))
        {
            State = PlayerState.Jump;
            return;
        }

        // �̵� ����
        if (_movePressed)
        {
            State = PlayerState.Walk;
            return;
        }
    }

    void UpdateWalk()
    {
        // ���� ���� ����
        if (Input.GetKey(KeyCode.A))
        {
            State = PlayerState.Attack;
            return;
        }

        // ���� ����
        if (Input.GetKey(KeyCode.Space))
        {
            State = PlayerState.Jump;
            return;
        }

        // �̵� ����
        if (!_movePressed)
        {
            State = PlayerState.Idle;
            return;
        }
    }

    void UpdateJump()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine("CoStartJump");
        }
    }

    void ApplyMove()
    {
        Vector3 nextPos = transform.position;

        // X ��
        nextPos += Time.deltaTime * new Vector3(_speedVec.x, 0, 0);
        if (!Managers.Map.CanGo(nextPos))
        {
            nextPos = Managers.Map.GetCorrectionPos(transform.position, nextPos);
        }
        transform.position = nextPos;

        // Y ��
        nextPos += Time.deltaTime * new Vector3(0, _speedVec.y, 0);
        if (!Managers.Map.CanGo(nextPos))
        {
            nextPos = Managers.Map.GetCorrectionPos(transform.position, nextPos);
        }

        transform.position = nextPos;
    }

    // _speedVec�� ��ȭ
    void ApplyPhysic()
    {
        Vector3 nextPosVec = _speedVec;

        // ����Ű
        if (_movePressed && (State == PlayerState.Walk || State == PlayerState.Jump))
        {
            if (Dir == MoveDir.Left)
            {
                nextPosVec += Vector3.left * _moveSpeed;
                nextPosVec.x = Mathf.Max(nextPosVec.x, -_moveSpeed);
            }
            else if (Dir == MoveDir.Right)
            {
                nextPosVec += Vector3.right * _moveSpeed;
                nextPosVec.x = Mathf.Min(nextPosVec.x, _moveSpeed);
            }
        }
        else
        {
            nextPosVec.x = 0;
        }

        // �߷� ����
        nextPosVec += Vector3.down * _ag;
        if (Managers.Map.IsStand(transform.position))
        {
            nextPosVec.y = 0;
        }

        _speedVec = nextPosVec;
    }

    void UpdateAttack()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine("CoStartAttack");
        }
    }

    void UpdateDead()
    {

    }

    IEnumerator CoStartAttack()
    {
        // ��� �ð�
        yield return new WaitForSeconds(0.45f);
        State = PlayerState.Idle;
        _coroutine = null;
    }

    IEnumerator CoStartJump()
    {
        _speedVec += Vector3.up * _jumpSpeed;
        // ��� �ð�
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (Managers.Map.IsStand(transform.position))
            {
                State = PlayerState.Idle;
                _coroutine = null;
                yield break;
            }
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
