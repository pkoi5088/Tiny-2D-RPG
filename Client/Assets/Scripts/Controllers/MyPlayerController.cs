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

    Animator _animator;
    bool _movePressed = false;
    public Vector2 Position = Vector2.zero;

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
        UpdateController();
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
            switch (Dir)
            {
                case MoveDir.Left:
                    transform.localScale = new Vector3(1, 1, 1);
                    break;
                case MoveDir.Right:
                    transform.localScale = new Vector3(-1, 1, 1);
                    break;
            }
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
        // 방향 입력
        switch (State)
        {
            case PlayerState.Idle:
            case PlayerState.Walk:
                GetDirInput();
                break;
        }

        // 상태기반 Update
        switch (State)
        {
            case PlayerState.Idle:
                UpdateIdle();
                break;

            case PlayerState.Walk:
                UpdateWalk();
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
        if (_movePressed)
        {
            State = PlayerState.Walk;
            return;
        }
    }

    void UpdateWalk()
    {
        if (!_movePressed)
        {
            State = PlayerState.Idle;
            return;
        }
    }

    void UpdateAttack()
    {

    }

    void UpdateDead()
    {

    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
