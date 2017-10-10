using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ダンジョン
/// </summary>
[Serializable] 
public class Dungeon
{
	/// <summary> マスタデータの配置パス </summary>
	public const string MasterDataPath	= "/MasterData/Dungeon";

	/// <summary> 階層情報 </summary>
	[SerializeField]	private List<Floor>		mFloores;

	//-----------------------------------------------------
	//	プロパティ
	//-----------------------------------------------------
	public int FloorCount { get { return mFloores.Count; } }
	//-----------------------------------------------------
	//	メソッド
	//-----------------------------------------------------
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public Dungeon()
	{
		mFloores = new List<Floor>(1);
		mFloores.Add(new Floor(this));
	}

	/// <summary>
	/// 1階分のデータを取得
	/// </summary>
	public Floor GetFloor(int index)
	{
		return mFloores[index];
	}

#if UNITY_EDITOR
	/// <summary>
	/// フロア追加
	/// </summary>
	public void AddFloor(int index)
	{
		if (index >= FloorCount) {
			mFloores.Add(new Floor(this));
		} else {
			index = Mathf.Max(index, 0);
			mFloores.Insert(index, new Floor(this));
		}
	}

	/// <summary>
	/// フロア削除
	/// </summary>
	public void RemoveFloor(int index)
	{
		if (index < 0 || index >= FloorCount) {
			return;
		}
		mFloores.RemoveAt(index);
	}

#endif//UNITY_EDITOR

	//=========================================================================
	/// <summary>
	/// ダンジョン1階分のデータ
	/// </summary>
	[Serializable] 
	public class Floor
	{
		/// <summary> 1階のセル数 </summary>
		[SerializeField]	private int			mWidth;
		[SerializeField]	private int			mHeight;
		[SerializeField]	private Cell []		mCells;

		private Dungeon		mDungeon;

		public int Width { get { return mWidth; } }
		public int Height { get { return mHeight; } }
		public Cell[] Cells { get { return mCells; } }

		public Floor()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Floor(Dungeon dungeon)
		{
			mDungeon = dungeon;
#if UNITY_EDITOR
			SetSize(16, 16);
#endif
		}

		/// <summary>
		/// 1マス分データ取得
		/// </summary>
		public Cell GetCell(int x, int y)
		{
			return mCells[y * mWidth + x];
		}

		/// <summary>
		/// 1マス分データ設定
		/// </summary>
		public void SetCell(int x, int y, Cell cell)
		{
			mCells[y * mWidth + x].CopyFrom(cell);
		}

		/// <summary>
		/// コピー
		/// </summary>
		public void CopyFrom(Floor src)
		{
			mDungeon = src.mDungeon;
			mWidth = 0;
			mHeight = 0;
			SetSize(src.Width, src.Height);

			for (int y = 0 ; y < mHeight ; y++) {
				for (int x = 0 ; x < mWidth ; x++) {
					GetCell(x, y).CopyFrom(src.GetCell(x, y));
				}
			}
		}

#if UNITY_EDITOR
		/// <summary>
		/// サイズ変更
		/// </summary>
		public void SetSize(int width, int height)
		{
			if (mWidth == width && mHeight == height) {
				return;
			}

			var newCells = new Cell[width * height];
			for (int i = 0 ; i < newCells.Length ; i++) {
				newCells[i] = new Cell();
			}

			// 現在の設定をコピー
			if (mCells != null) {
				for (int y = 0 ; y < Mathf.Min(mHeight, height) ; y++) {
					for (int x = 0 ; x < Mathf.Min(mWidth, width) ; x++) {
						newCells[y * width + x].CopyFrom(GetCell(x, y));
					}
				}
			}
			mCells = newCells;
			mWidth = width;
			mHeight = height;
		}
#endif//UNITY_EDITOR

	}// class Floor

	//=========================================================================
	/// <summary>
	/// 1マス分のデータ
	/// </summary>
	[Serializable] 
	public class Cell
	{
		public enum ECellType {
			Empty,			// 通路
			Wall,			// 壁
			EntryPoint,		// 開始位置
			Steps,			// 階段
			TreasureBox,	// 宝箱
			Boss,			// ボス
		}
		public enum EStepsValue {
			Up,
			Down
		}

		[SerializeField]	private ECellType	mCellType;
		[SerializeField]	private int []		mValues;


		public ECellType	CellType	{ get { return mCellType; } }
		public int []		Values		{ get { return mValues; }}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public Cell()
		{
			mCellType = ECellType.Wall;
			mValues = null;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Cell(ECellType type, params int []  values)
		{
			mCellType = type;
			mValues = values;
		}

		/// <summary>
		/// コピーコンストラクタ
		/// </summary>
		public Cell(Cell src)
		{
			CopyFrom(src);
		}

		/// <summary>
		/// コピー
		/// </summary>
		public void CopyFrom(Cell src)
		{
			mCellType = src.CellType;
			mValues = src.Values;
		}
	}// class Cell
}// class Dungeon
