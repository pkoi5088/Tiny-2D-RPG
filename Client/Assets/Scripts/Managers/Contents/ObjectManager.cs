using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

    public void Add(bool isPlayer = true)
    {
        if (isPlayer)
        {
            if (MyPlayer != null)
                return;

            // 플레이어
            GameObject go = Managers.Resource.Instantiate("Creature/Player");
            go.name = "Player";
            _objects.Add(1, go);

            // TODO: ID 고정값 변경
            MyPlayer = go.GetComponent<MyPlayerController>();
            MyPlayer.ID = 1;
            // 캐릭터 정보 설정
            // TODO?: 플레이어 위치를 Manager에서 직접 설정?
            MyPlayer.transform.position = Vector3.up * 2.5f;
        }
        else
        {
            // 몬스터
            GameObject go = Managers.Resource.Instantiate("Creature/Monster");
            go.name = "Monster";
            _objects.Add(2, go);

            MonsterController mc = go.GetComponent<MonsterController>();
            mc.ID = 2;
            mc.transform.position = (Vector3.up + Vector3.right) * 2.5f;
        }
    }

    public MyPlayerController GetClosestPlayer(Vector3 pos)
    {
        MyPlayerController ret = null;
        float retDist = 0.0f;
        foreach (GameObject obj in _objects.Values)
        {
            MyPlayerController mpc = obj.GetComponent<MyPlayerController>();
            if (mpc == null)
                continue;

            if (ret == null)
            {
                ret = mpc;
                retDist = (mpc.Pos - pos).magnitude;
            }
            else
            {
                float tDist = (mpc.Pos - pos).magnitude;
                if (retDist > tDist)
                {
                    ret = mpc;
                    retDist = tDist;
                }
            }
        }
        return ret;
    }

    public MyPlayerController GetClosestPlayer(Vector3 pos, float range)
    {
        MyPlayerController rmpc = GetClosestPlayer(pos);
        if (rmpc == null)
            return null;

        if ((rmpc.Pos - pos).magnitude > range)
            return null;
        return rmpc;
    }
}
