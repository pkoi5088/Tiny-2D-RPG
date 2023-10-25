using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);
        Managers.Object.Add(true);
        Managers.Object.Add(false);
    }

    public override void Clear()
    {
        
    }
}
