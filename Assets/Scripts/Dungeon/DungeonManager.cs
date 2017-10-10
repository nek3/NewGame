using UnityEngine;
using System.Collections;
using System;
using UniRx;

/// <summary>
/// ダンジョンマネージャ
/// </summary>
public class DungeonManager
{
	private string			mDungeonFileName;
	/// <summary> ダンジョンマスタデータ </summary>
	private Dungeon			mDungeonMaster;
	/// <summary> ダンジョンに挑むパーティ </summary>
	private DungeonParty	mParty;

	//--- 現在位置など ---//
	DungeonProgress			mDungeonProgress;
	/// <summary> 現在の階数 </summary>
	private int				mFloorNumber;
	/// <summary> フロア内の位置 </summary>
	private Vector2<int>	mPos;

	//-------------------------------------------------------------------------
	//	public メソッド
	//-------------------------------------------------------------------------
	/// <summary>
	/// ロード
	/// </summary>
	public void LoadDungeonData(string fileName, Action onLoaded)
	{
		mDungeonFileName = fileName;
		AssetBundleLoader.Instance.LoadAsset(MasterDataManager.AssetBundleName, mDungeonFileName, loadedObj => {
			var textAsset = loadedObj as TextAsset;
			mDungeonMaster = JsonUtility.FromJson<Dungeon>(textAsset.text);

			if (onLoaded != null) {
				onLoaded();
			}
		});
	}

	/// <summary>
	/// ダンジョンに挑むパーティを設定
	/// </summary>
	public void EntryParty(DungeonParty party)
	{
		mParty = party;
	}
	
	/// <summary>
	/// ダンジョン処理開始
	/// </summary>
	public void Start()
	{
		// 進行データがあれば取得
		mDungeonProgress = DungeonSaveData.GetData(mDungeonFileName);
		if (mDungeonProgress == null) {
			// 進行データを新規作成
			mDungeonProgress = DungeonSaveData.CreateData(mDungeonFileName, mDungeonMaster);
			// 開始位置を検索
			mPos = FindFloorSteps(mDungeonMaster.GetFloor(mFloorNumber), (int)Dungeon.Cell.EStepsValue.Up);
		}

	}

	/// <summary>
	/// 1ターン進める
	/// </summary>
	public void UpdateTurn()
	{
	}

	//-------------------------------------------------------------------------
	//	private メソッド
	//-------------------------------------------------------------------------
	/// <summary>
	/// フロアから階段の位置を取得
	/// </summary>
	private Vector2<int> FindFloorSteps(Dungeon.Floor floor, Dungeon.Cell.EStepsValue stepValue)
	{
		Dungeon.Cell[] cells = floor.Cells;

		for (int i = 0 ; i < cells.Length ; i++) {
			if (cells[i].CellType == Dungeon.Cell.ECellType.Steps && cells[i].Values[0] == (int)stepValue) {
				var result = new Vector2<int>();
				result.Set(i % floor.Width, (int)(i / floor.Width));
				return result;
			}
		}
		return null;
	}
}
