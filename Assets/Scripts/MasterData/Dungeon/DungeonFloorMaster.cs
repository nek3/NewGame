using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダンジョンフロア情報
/// </summary>
[Serializable]
public class DungeonFloorMaster
{
    public enum EGroundType
    {
		/// <summary> 通常床。進行可能 </summary>
        Normal,
		/// <summary> 進行禁止 </summary>
		Block, 
    }

    /// <summary> フロアの横幅 </summary>
    [SerializeField] private int mWidth;
    /// <summary> フロアの縦幅 </summary>
    [SerializeField] private int mHeight;
	/// <summary> 床情報 </summary>
	[SerializeField] private EGroundType[] mGrounds;
	// 配置物情報



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
