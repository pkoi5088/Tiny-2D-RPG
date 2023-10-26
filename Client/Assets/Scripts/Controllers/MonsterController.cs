using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{
    MyPlayerController _target = null;
    float _searchRange = 5.0f;
    float _attackRange = 0.8f;

    protected override void Init()
    {
        base.Init();
        _moveSpeed = 2.5f;
    }

    protected override void UpdateController()
    {
        // 방향 설정
        switch (State)
        {
            case CreatureState.Idle:
            case CreatureState.Walk:
            case CreatureState.Jump:
                SetDir();
                SetPlayer();
                break;
        }

        base.UpdateController();
    }

    void SetDir()
    {
        if (_target == null)
            return;

        Vector3 nowPos = _target.Pos - Pos;

        if (nowPos.x < 0.0f)
        {
            Dir = MoveDir.Left;
        }
        else
        {
            Dir = MoveDir.Right;
        }
    }

    void SetPlayer()
    {
        MyPlayerController pc = Managers.Object.GetClosestPlayer(Pos, _searchRange);
        _target = pc;
    }

    protected override void UpdateIdle()
    {
        if (_target == null)
            return;

        if ((_target.Pos - Pos).magnitude < _attackRange)
        {
            State = CreatureState.Attack;
        }
        else
        {
            State = CreatureState.Walk;
        }
    }

    protected override void UpdateWalk()
    {
        if (_target == null)
        {
            State = CreatureState.Idle;
            return;
        }

        // 점프 판정
        if (Managers.Map.CanGo(GetFrontPos()) == false)
        {
            State = CreatureState.Jump;
            return;
        }

        // 공격 판정
        if ((_target.Pos - Pos).magnitude < _attackRange)
        {
            State = CreatureState.Attack;
        }
    }

    Vector3 GetFrontPos()
    {
        Vector3 ret = Pos;

        if (Dir == MoveDir.Left)
        {
            ret.x -= 0.1f;
        }
        else if (Dir == MoveDir.Right)
        {
            ret.x += 0.1f;
        }

        return ret;
    }

    protected override void ApplyPhysic()
    {
        Vector3 nextPosVec = _speedVec;

        // 방향키
        if (_target != null && (State == CreatureState.Walk || State == CreatureState.Jump))
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
}
