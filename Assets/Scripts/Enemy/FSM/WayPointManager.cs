using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    private static WayPointManager _instance;

    /*属性封装*/
    public static WayPointManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public List<int> usingIndex = new List<int>();  //每个敌人分配用到的路线
    public List<int> rawIndex = new List<int>();    //辅助list。将路线打乱分配
    private void Awake()
    {
        _instance = this;   //初始化
        int tempcount = rawIndex.Count;
        for (int i = 0; i < tempcount; i++)
        {
            int tempindex = Random.Range(0, rawIndex.Count);
            usingIndex.Add(rawIndex[tempindex]);
            rawIndex.RemoveAt(tempindex);
        }

    }
}
