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
	/// <summary>
	/// セルタイプ
	/// </summary>
    public enum EGroundType
    {
		/// <summary> 通常床。進行可能 </summary>
        Normal,
		/// <summary> 進行禁止 </summary>
		Block, 
    }

	/// <summary>
	/// 配置物の種類
	/// </summary>
	public enum EStaticObjectType
	{
		/// <summary>　階段　</summary>
		Steps,		
		/// <summary> 宝箱 </summary>
		TreasureBox,
		/// <summary> ボス </summary>
		Boss,
	}

	/// <summary>
	/// 配置物情報
	/// </summary>
	[Serializable]
	public struct StaticObject
	{
		[SerializeField] public int mPosX;
		[SerializeField] public int mPosY;
		
		[SerializeField] public EStaticObjectType mObjectType;
		[SerializeField] public int mObjectParam1;
		[SerializeField] public int mObjectParam2;
	}

    /// <summary> フロアの横幅 </summary>
    [SerializeField] private int mWidth;
    /// <summary> フロアの縦幅 </summary>
    [SerializeField] private int mHeight;
	/// <summary> 床情報 </summary>
	[SerializeField] private EGroundType[] mGrounds;
	// 配置物情報
	[SerializeField] private StaticObject[] mStaticObjects;


	
}
