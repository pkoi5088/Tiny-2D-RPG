using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;
using static PlayerObj;

public class MyPlayerController : CreatureController
{
    bool _movePressed = false;

    protected override void UpdateController()
    {
        // TEST
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(transform.position);
            Debug.Log(Managers.Map.FindClosePos(transform.position));
            Debug.Log(Managers.Map.SceneToArr(Managers.Map.FindClosePos(transform.position)));
        }

        // 방향 입력
        switch (State)
        {
            case CreatureState.Idle:
            case CreatureState.Walk:
            case CreatureState.Jump:
                GetDirInput();
                break;
        }

        // 공격 판정
        switch (State)
        {
            case CreatureState.Idle:
            case CreatureState.Walk:
                GetAttackInput();
                break;
        }

        // 점프 판정
        switch (State)
        {
            case CreatureState.Idle:
            case CreatureState.Walk:
                GetJumpInput();
                break;
        }

        base.UpdateController();
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

    void GetAttackInput()
    {
        // 공격 판정 먼저
        if (Input.GetKey(KeyCode.A))
        {
            State = CreatureState.Attack;
            return;
        }
    }

    void GetJumpInput()
    {
        // 점프 판정
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Jump;
            return;
        }
    }

    protected override void UpdateIdle()
    {
        // 이동 판정
        if (_movePressed)
        {
            State = CreatureState.Walk;
            return;
        }
    }

    protected override void UpdateWalk()
    {
        // 이동 판정
        if (!_movePressed)
        {
            State = CreatureState.Idle;
            return;
        }
    }

    protected override void ApplyPhysic()
    {
        Vector3 nextPosVec = _speedVec;

        // 방향키
        if (_movePressed && (State == CreatureState.Walk || State == CreatureState.Jump))
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
        _speedVec = nextPosVec;

        base.ApplyPhysic();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }
}
