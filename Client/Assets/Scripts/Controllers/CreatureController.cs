using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;
using static PlayerObj;

public class CreatureController : MonoBehaviour
{
    public enum CreatureState
    {
        Idle,
        Walk,
        Jump,
        Attack,
        Dead,
    }

    public enum MoveDir
    {
        Left,
        Right,
    }

    protected float _moveSpeed = 5.0f; // 이동속도
    protected float _ag = 0.4f; // 중력 가속도 (질량은 1.0f로 가정)
    protected float _jumpSpeed = 17.0f; // 점프력 (질량은 1.0f로 가정)

    public int ID { get; set; }
    protected Animator _animator;
    protected Coroutine _coroutine = null;

    public Vector3 Pos
    {
        get { return new Vector3(transform.position.x, transform.position.y, 0); }
        set { transform.position = value; }
    }
    protected Vector3 _speedVec = new Vector3(0, 0, 0); // 속도

    CreatureState _state = CreatureState.Idle;
    public CreatureState State
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

    protected MoveDir _dir = MoveDir.Left;
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

    protected virtual void Init()
    {
        _animator = Util.FindChild(gameObject, "UnitRoot", false).GetComponent<Animator>();
        State = CreatureState.Idle;
        Dir = MoveDir.Right;
    }

    protected void UpdateAnimation()
    {
        if (_animator == null)
            return;

        if (State == CreatureState.Idle)
        {
            _animator.Play("Idle");
        }
        else if (State == CreatureState.Walk)
        {
            _animator.Play("Walk");
        }
        else if (State == CreatureState.Jump)
        {
            _animator.Play("Jump");
        }
        else if (State == CreatureState.Attack)
        {
            _animator.Play("Attack");
        }
        else if (State == CreatureState.Dead)
        {
            _animator.Play("Dead");
        }
    }

    void Update()
    {
        // Update 실행
        UpdateController();
    }

    protected virtual void UpdateController()
    {
        // 상태기반 Update
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;

            case CreatureState.Walk:
                UpdateWalk();
                break;

            case CreatureState.Jump:
                UpdateJump();
                break;

            case CreatureState.Attack:
                UpdateAttack();
                break;

            case CreatureState.Dead:
                UpdateDead();
                break;
        }

        //이동 적용
        ApplyMove();

        // 물리 적용
        ApplyPhysic();
    }

    protected virtual void UpdateIdle()
    {

    }

    protected virtual void UpdateWalk()
    {

    }

    protected virtual void UpdateJump()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine("CoStartJump");
        }
    }

    protected virtual void UpdateAttack()
    {
        if (_coroutine == null)
        {
            _coroutine = StartCoroutine("CoStartAttack");
        }
    }

    protected virtual void UpdateDead()
    {

    }

    protected virtual void ApplyMove()
    {
        Vector3 nextPos = transform.position;

        // X 축
        nextPos += Time.deltaTime * new Vector3(_speedVec.x, 0, 0);
        if (!Managers.Map.CanGo(nextPos))
        {
            nextPos = Managers.Map.GetCorrectionPos(transform.position, nextPos);
        }
        transform.position = nextPos;

        // Y 축
        nextPos += Time.deltaTime * new Vector3(0, _speedVec.y, 0);
        if (!Managers.Map.CanGo(nextPos))
        {
            nextPos = Managers.Map.GetCorrectionPos(transform.position, nextPos);
        }

        transform.position = nextPos;
    }

    protected virtual void ApplyPhysic()
    {
        Vector3 nextPosVec = _speedVec;

        // 중력 적용
        nextPosVec += Vector3.down * _ag;
        if (Managers.Map.IsStand(transform.position))
        {
            nextPosVec.y = 0;
        }

        _speedVec = nextPosVec;
    }

    protected IEnumerator CoStartAttack()
    {
        // 대기 시간
        yield return new WaitForSeconds(0.45f);
        State = CreatureState.Idle;
        _coroutine = null;
    }

    protected IEnumerator CoStartJump()
    {
        _speedVec += Vector3.up * _jumpSpeed;
        // 대기 시간
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (Managers.Map.IsStand(transform.position))
            {
                State = CreatureState.Idle;
                _coroutine = null;
                yield break;
            }
        }
    }
}
