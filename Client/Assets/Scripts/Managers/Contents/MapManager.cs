using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    // MinX <= X < MaxX가 실제 필드 좌표
    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    public int SizeX { get { return MaxX - MinX; } }
    public int SizeY { get { return MaxY - MinY; } }

    bool[,] _collision;

    public bool CanGo(Vector3 Pos)
    {
        if (Pos.x < MinX || Pos.x >= MaxX) return false;
        if (Pos.y < MinY || Pos.y >= MaxY) return false;

        Vector2Int pos = FindClosePos(Pos);

        // 좌표 변환
        pos = SceneToArr(pos);

        return !_collision[pos.y, pos.x];
    }

    public bool IsStand(Vector3 Pos)
    {
        if (Pos.x < MinX || Pos.x >= MaxX) return false;
        if (Pos.y < MinY || Pos.y >= MaxY) return false;

        Vector3 standingPos = new Vector3(Pos.x, Pos.y - 0.01f, 0);

        return !CanGo(standingPos);
    }

    public Vector3 GetCorrectionPos(Vector3 from, Vector3 to)
    {
        Vector3 ret = from;
        while (true)
        {
            Vector3 mid = (from + to) / 2;
            if ((mid - ret).magnitude < 0.01f)
            {
                return ret;
            }

            if (CanGo(mid))
            {
                ret = mid;
                from = mid;
            }
            else
            {
                to = mid;
            }
        }
    }

    public void LoadMap(int mapId)
    {
        DestroyMap();

        string mapName = "Map" + mapId.ToString("000");
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        GameObject collision = Util.FindChild(go, "Collision", true);
        if (collision != null)
            collision.SetActive(false);

        // 충돌 영역 불러오기
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());

        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount - 1, xCount - 1];

        for (int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            if (y == 0) continue;
            for (int x = 0; x < xCount; x++)
            {
                if (x == xCount - 1) continue;
                _collision[y - 1, x] = (line[x] == '1' ? true : false);
            }
        }

        // 배경 설정하기
        SpriteRenderer background = Util.FindChild<SpriteRenderer>(go, "Background1", true);
        if (background == null)
            return;
        Debug.Log(background.sprite.bounds.size.x);
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if (map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }

    public Vector2Int FindClosePos(Vector3 pos)
    {
        Vector2Int ret = new Vector2Int(0, 0);

        ret.x = (int)Mathf.Floor(pos.x);
        ret.y = (int)Mathf.Floor(pos.y);

        return ret;
    }

    public Vector2Int SceneToArr(Vector2Int pos)
    {
        Vector2Int ret = pos;

        ret.x = MaxX + 1 + ret.x;
        ret.y = MaxY - 1 - ret.y;

        return ret;
    }

    public Vector2Int ArrToScene(Vector2Int pos)
    {
        Vector2Int ret = pos;

        ret.x = ret.x - (MaxX + 1);
        ret.y = MaxY - 1 - ret.y;

        return ret;
    }
}
