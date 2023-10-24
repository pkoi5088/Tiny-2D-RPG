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
        if (MyPlayer != null)
            return;

        if (isPlayer)
        {
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
        }
    }
}
