using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// ダンジョン進捗情報
/// </summary>
[Serializable]
public class DungeonProgress
{
	/// <summary> 階層情報 </summary>
	[SerializeField]	private List<FloorProgress>	mFloores;
	/// <summary> 現在の階 </summary>
	[SerializeField]	private int					mNowFloor;

	//-------------------------------------------------------------------------
	//	public メソッド
	//-------------------------------------------------------------------------
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public DungeonProgress(Dungeon masterData)
	{
		mFloores = new List<FloorProgress>(masterData.FloorCount);
		for (int i = 0 ; i < masterData.FloorCount ; i++) {
			mFloores.Add(new FloorProgress(masterData.GetFloor(i)));
		}
		mNowFloor = 0;
	}

	/// <summary>
	/// 現在進行中の階設定
	/// </summary>
	public void SetFloor(int num)
	{
		Debug.Assert(0 <= num && num < mFloores.Count);
		mNowFloor = num;
	}

	/// <summary>
	/// 指定位置に入ったことにする
	/// </summary>
	public void SetEnter(Vector2<int> pos)
	{
		mFloores[mNowFloor].GetCell(pos).Status |= CellProgress.EStatus.Enter;
	}

	//-------------------------------------------------------------------------
	//	外部クラス
	//-------------------------------------------------------------------------
	/// <summary>
	/// フロア状態
	/// </summary>
	[Serializable]
	public class FloorProgress
	{
		/// <summary> フロアサイズ </summary>
		[SerializeField]	Vector2<int>				mSize;
		/// <summary> フロアのセル情報 </summary>
		[SerializeField]	private CellProgress []		mCells;
		/// <summary> 開けた宝箱インデックスリスト </summary>
		[SerializeField]	private List<int>			mOpenTreasureBoxes;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FloorProgress(Dungeon.Floor floorMaster)
		{
			mSize = new Vector2<int>();
			mSize.x = floorMaster.Width;
			mSize.y = floorMaster.Height;
			mCells	= new CellProgress[mSize.x * mSize.y];
			for (int i = 0 ; i < mCells.Length ; i++) {
				mCells[i] = new CellProgress();
			}
		}

		/// <summary>
		/// Cell取得
		/// </summary>
		public CellProgress GetCell(Vector2<int> pos)
		{
			return mCells[pos.y * mSize.x + pos.x];
		}
	}// class FloorProgress

	/// <summary>
	/// セル状態
	/// </summary>
	[Serializable]
	public class CellProgress
	{
		[Flags]
		public enum EStatus {
			Empty				= 0,
			/// <summary> このセルが見えた </summary>
			Show				= 1,
			/// <summary> このセルに入った </summary>
			Enter				= 1 << 1,
			/// <summary> 宝箱を開けた </summary>
			OpenTreasureBox		= 1 << 2,

		}

		[SerializeField]	private	EStatus		mStatus;

		/// <summary> 状態 </summary>
		public EStatus Status {
			get { return mStatus; } 
			set { mStatus = value; }
		}
	}// class CellProgress
}

/// <summary>
/// ダンジョンセーブデータ
/// </summary>
[Serializable]
public class DungeonSaveData
{
	private const string SaveKey	= "GuMasDgn";

	private static DungeonSaveData	msInstance;

	[SerializeField]	Dictionary<string, DungeonProgress>		mDungeonProgressList;

	/// <summary> コンストラクタ </summary>
	private DungeonSaveData()	{}


	/// <summary>
	/// セーブデータからクラス生成
	/// </summary>
	public static void Load()
	{
		string data = SaveData.GetSaveData(SaveKey);
		if (string.IsNullOrEmpty(data)) {
			// 新規
			msInstance = new DungeonSaveData();
		} else {
			msInstance = JsonUtility.FromJson<DungeonSaveData>(data);
		}
	}

	/// <summary>
	/// セーブデータ取得
	/// </summary>
	public static DungeonProgress GetData(string name)
	{
		if (msInstance == null) {
			return null;
		}

		DungeonProgress result = null;
		if (msInstance.mDungeonProgressList.TryGetValue(name, out result)) {
			return result;
		}
		return null;
	}

	/// <summary>
	/// 新規データ作成
	/// </summary>
	public static DungeonProgress CreateData(string name, Dungeon dungeonMaster)
	{
		Debug.Assert(msInstance != null);
		Debug.Assert(!msInstance.mDungeonProgressList.ContainsKey(name));

		var newData = new DungeonProgress(dungeonMaster);
		msInstance.mDungeonProgressList[name] = newData;

		return newData;
	}
}