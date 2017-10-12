using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

//TODO: Undo
//		宝箱設定
//		敵設定

/// <summary>
/// ダンジョン作成ウィンドウ
/// </summary>
public class DungeonEditorWindow : EditorWindow
{
	public const string MenuName = "/ダンジョン作成...";

	private enum ETipTexture {
		Empty,
		Wall,
		Boss,
		EntryPoint,
		UpSteps,
		DownSteps,
		TreasureBox,

		Max
	}
	private static string mSavePath = string.Empty;
	private static string SavePath { 
		get {
			if (mSavePath == null) {
				mSavePath = Application.dataPath + "/AssetBundle" + Dungeon.MasterDataPath;
			}
			return mSavePath;
		}
	}

	// マップ描画領域
	private const float MapStartX		= 4.0f;	// ピクセル
	private const float MapStartY		= 4.0f; // ピクセル
	private const float MapRectWidth	= 0.7f;	// ウィンドウ幅に対する割合
	private const float MapRectHeight	= 0.9f;	// ウィンドウ高さに対する割合
	// マップ描画領域右端から各種フィールドまでのマージン
	private const float ParamFieldMarginLeft	= 0.01f;
	private const float ParamFieldMarginTop		= 0.01f;
	
	// マップグリッド描画時に点線の数
	private const int MapDotterLineCount	= 8;

	// マップチップボタンサイズ
	private const float MapTipButtonSize	= 48;

	/// <summary> ウィンドウ描画領域 </summary>
	private Rect ControlRect { get { return this.position; } }
	/// <summary> １セルの描画幅 </summary>
	private float CellWidth { get { return Mathf.Min(ControlRect.width * MapRectWidth / SelectFloor.Width, ControlRect.height * MapRectHeight / SelectFloor.Height); } }
	/// <summary> 1セルの描画高さ </summary>
	private float CellHeight { get { return CellWidth; } }

	/// <summary> フィールドの高さ </summary>
	private float FieldHeight { get { return 20; } }
	/// <summary> フィールド間の高さ間隔 </summary>
	private float FieldInterval { get { return FieldHeight + 4; } }
	/// <summary> インデント幅 </summary>
	private float FieldIndentWidth { get { return 12; } }
	/// <summary> ボタンデフォルトサイズ </summary>
	private float ButtonWidth { get { return 32; } }


	private static Texture2D mTexture;
	/// <summary> マップチップテクスチャ </summary>
	private static TipTexture [] mTipTextures;

	/// <summary>
	/// 選択中のフロア
	/// </summary>
	private Dungeon.Floor SelectFloor {
		get {
			if (0 > mSelectFloorIndex) {
				return null;
			}
			return mDungeon.GetFloor(mSelectFloorIndex);
		}
	}

	//-------------------------------------------------------------------------
	//	各種パラメータ
	//-------------------------------------------------------------------------
	// ダンジョン情報
	private static Dungeon	mDungeon;
	/// <summary> 選択中のフロア </summary>
	private static int		mSelectFloorIndex		= -1;
	// 保存ファイル名
	private static string	mFileName;
	// グリッドを描画するかどうか
	private static bool		mDrawGrid	= true;

	//------------------------------------------------------------------
	//	エディタ上の設定
	//------------------------------------------------------------------
	/// <summary> フロア選択スクロール </summary>
	private Vector2 mFloorSelectPosition	= new Vector2();
	/// <summary> フロア選択コントロールを表示するかどうか </summary>
	private bool	mShowSelectFloor		= false;
	private int		mTmpDungeonWidth;
	private int		mTmpDungeonHeight;
	/// <summary> 選択中のチップ </summary>
	private TipTexture mSelectTip = null;

	/// <summary>
	/// Windowオープンした時
	/// </summary>
	private void OnEnable()
	{
		Initialize();
		ResetTempParam();
		// メニューのチェックを付ける
		UnityEditor.Menu.SetChecked(MyMenu.MenuRoot + MenuName, true);
	}

	/// <summary>
	/// Windowクローズした時
	/// </summary>
	protected void OnDisable()
	{
		Release();
		// メニューのチェックを外す
		UnityEditor.Menu.SetChecked(MyMenu.MenuRoot + MenuName, false);
	}
	
	/// <summary>
	/// フォーカスされた時
	/// </summary>
	private void OnFocus()
	{
	}

	/// <summary>
	/// 描画
	/// </summary>
	private void OnGUI()
	{
		DrawMap();

		if (mDrawGrid) {
			// グリッド描画
			DrawGridLine();
		}

		Vector2 pos = new Vector2(GetWidth(MapRectWidth + ParamFieldMarginLeft), GetHeight(ParamFieldMarginTop));
		// オプション
		pos = DrawOption(pos);
		// ダンジョン設定
		pos = DrawDungeonSettingField(pos);

        // クリックされた位置を探して、その場所に画像データを入れる
        Event e = Event.current;
        if (mSelectTip != null && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)) {
			int mapPosX, mapPosY;
			if (TryGetCellPosition(e.mousePosition, out mapPosX, out mapPosY)) {
				// ボタンダウン時は選択中のチップで描画
				var cell = SelectFloor.GetCell(mapPosX, mapPosY);
				if (cell.CellType != mSelectTip.CellType) {
					SelectFloor.SetCell(mapPosX, mapPosY, new Dungeon.Cell(mSelectTip.CellType, mSelectTip.CellValue));
					Repaint();
				}
			}
		}

	}

	/// <summary>
	/// オプション項目描画
	/// </summary>
	private Vector2 DrawOption(Vector2 startPos)
	{
		Rect pos = new Rect(startPos, new Vector2(GetWidth(0.2f), FieldHeight));
		mDrawGrid = EditorGUI.ToggleLeft(pos, "グリッド表示", mDrawGrid);

		pos.y += FieldInterval;
		return new Vector2(pos.xMin, pos.yMax);
	}

	/// <summary>
	/// ダンジョン設定項目描画
	/// </summary>
	private Vector2 DrawDungeonSettingField(Vector2 pos)
	{
		// フロア選択項目の描画
		pos = DrawFloorSelectField(pos);

		// ダンジョンサイズ設定項目の描画
		pos = DrawFloorSizeField(pos);
		// マップチップ選択の描画
		pos = DrawDungeonSelectTip(pos);
		// セーブロード
		pos = DrawSaveField(pos);
		return pos;
	}

	/// <summary>
	/// フロア選択フィールド描画
	/// </summary>
	private Vector2 DrawFloorSelectField(Vector2 startPos)
	{
		Rect pos = new Rect(startPos, new Vector2(64.0f, FieldHeight));
		EditorGUI.LabelField(pos, "フロア数");
		pos.x += FieldIndentWidth + pos.width;
		GUI.enabled = false;
		EditorGUI.IntField(pos, mDungeon.FloorCount);
		GUI.enabled = true;

		pos.x = startPos.x + FieldIndentWidth;
		pos.y += FieldInterval;
		pos.width = 64.0f;

		if (GUI.Button(pos, "下に追加")) {
			mDungeon.AddFloor(mSelectFloorIndex);
			mSelectFloorIndex++;
		}
		pos.x += pos.width + FieldIndentWidth;
		if (GUI.Button(pos, "上に追加")) {
			mDungeon.AddFloor(mSelectFloorIndex + 1);
		}
		
		pos.x = startPos.x;
		pos.y += FieldInterval;
		pos.width = 64.0f;
		EditorGUI.LabelField(pos, "フロア選択");
		pos.x += FieldIndentWidth + pos.width;
		GUI.enabled = false;
		EditorGUI.IntField(pos, mSelectFloorIndex);
		GUI.enabled = true;
		float selectFloorPosX = pos.x;

		pos.x += FieldIndentWidth + pos.width;
		pos.width = 48.0f;
		if (GUI.Button(pos, "選択")) {
			mShowSelectFloor = true;
		}

		pos.x += FieldIndentWidth + pos.width;
		GUI.enabled = mDungeon.FloorCount > 1;
		if (GUI.Button(pos, "削除")) {
			int nextSelect = mSelectFloorIndex;
			if (mSelectFloorIndex == mDungeon.FloorCount - 1) {
				nextSelect --;
			}
			mDungeon.RemoveFloor(mSelectFloorIndex);
			mSelectFloorIndex = nextSelect;
			Repaint();
		}
		GUI.enabled = true;


		pos.x = selectFloorPosX;
		pos.y += FieldInterval;
		if (mShowSelectFloor) {
			string [] floores = new string[mDungeon.FloorCount];
			for (int i = 0 ; i < mDungeon.FloorCount ; i++) {
				floores[i] = i.ToString();
			}
			pos.width	= 96.0f;
			pos.height	= Mathf.Min(FieldHeight * mDungeon.FloorCount, FieldHeight * 4);

			mFloorSelectPosition = GUI.BeginScrollView(pos, mFloorSelectPosition, new Rect(0, 0, 64, FieldHeight * mDungeon.FloorCount));
			int select = GUI.SelectionGrid(new Rect(0, 0, 64, FieldHeight * mDungeon.FloorCount), -1, floores, 1);
			if (select >= 0) {
				mSelectFloorIndex = select;
				mShowSelectFloor = false;
				Repaint();
			}
			GUI.EndScrollView();

			pos.y += pos.height;
		}
		pos.x = startPos.x;
		pos.height = FieldHeight;
		return new Vector2(pos.xMin, pos.yMax);
	}

	/// <summary>
	/// ダンジョンサイズフィールド描画
	/// </summary>
	private Vector2 DrawFloorSizeField(Vector2 startPos)
	{
		Rect pos = new Rect(startPos, new Vector2(GetWidth(0.2f), FieldHeight));
		EditorGUI.LabelField(pos, "フロアサイズ");
		Rect floorSizePos = new Rect(pos.x + FieldIndentWidth, pos.y + FieldInterval, 32.0f, FieldHeight);
		mTmpDungeonWidth = EditorGUI.IntField(floorSizePos, mTmpDungeonWidth);
		floorSizePos.x += floorSizePos.width + FieldIndentWidth;
		mTmpDungeonHeight = EditorGUI.IntField(floorSizePos, mTmpDungeonHeight);
		floorSizePos.x += floorSizePos.width + FieldIndentWidth;
		floorSizePos.width = ButtonWidth;
		if (GUI.Button(floorSizePos, "反映")) {
			ApplyParam();
			GUI.FocusControl(null);
		}
		floorSizePos.x += floorSizePos.width + (FieldIndentWidth * 0.5f);
		if (GUI.Button(floorSizePos, "戻す")) {
			ResetTempParam();
			GUI.FocusControl(null);
		}

		pos.y += floorSizePos.height + FieldInterval;
		return new Vector2(pos.xMin, pos.yMax);
	}

	/// <summary>
	/// マップチップ選択描画
	/// </summary>
	private Vector2 DrawDungeonSelectTip(Vector2 startPos)
	{
		Rect pos = new Rect(startPos, new Vector2(GetWidth(0.2f), FieldHeight));
		EditorGUI.LabelField(pos, string.Format("チップ選択 ({0})", mSelectTip != null ? mSelectTip.CellType.ToString() : "未選択"));

		Rect buttonPos = new Rect(pos.x + FieldIndentWidth, pos.y + FieldInterval, MapTipButtonSize, MapTipButtonSize);
		foreach (var tip in mTipTextures) {
			bool select = tip == mSelectTip;
			if (GUI.Toggle(buttonPos, select, tip.Texture) != select) {
				// 選択状態変更
				mSelectTip = tip;
				break;
			}

			buttonPos.x += buttonPos.width + FieldIndentWidth;
			if (buttonPos.xMax >= ControlRect.width) {
				buttonPos.x = pos.x + FieldIndentWidth;
				buttonPos.y += buttonPos.height + (FieldInterval * 0.25f);
			}
		}

		pos.y = buttonPos.yMax + FieldInterval;
		return new Vector2(pos.xMin, pos.yMax);
	}

	/// <summary>
	/// セーブ・ロードフィールド
	/// </summary>
	private Vector2 DrawSaveField(Vector2 startPos)
	{
		Rect pos = new Rect(startPos, new Vector2(GetWidth(0.25f), FieldHeight));
		// ファイル名
		GUI.enabled = false;
		EditorGUI.LabelField(pos, "ファイル名");
		GUI.enabled = true;
		pos.x += FieldIndentWidth;
		pos.y += FieldInterval;
		mFileName = EditorGUI.TextField(pos, mFileName);

		pos.y += FieldInterval;
		pos.width = ButtonWidth * 2.0f;
		// セーブボタン
		GUI.enabled = mDungeon != null;
		if (GUI.Button(pos, "Save")) {
			string defaultName = Path.GetFileNameWithoutExtension(string.IsNullOrEmpty(mFileName) ? "DungeonMap" : mFileName);
			var savePath = EditorUtility.SaveFilePanel("SaveDungeonMap", SavePath, defaultName, "json");
			if (!string.IsNullOrEmpty(savePath)) {
				string jsonText = JsonUtility.ToJson(mDungeon);
				File.WriteAllText(savePath, jsonText);
				AssetDatabase.Refresh();

				mFileName = Path.GetFileNameWithoutExtension(savePath);
			}
			Debug.Log(savePath);
		}
		GUI.enabled = true;

		pos.x += pos.width + FieldIndentWidth;
		// ロードボタン
		if (GUI.Button(pos, "Load")) {
			var loadPath = EditorUtility.OpenFilePanelWithFilters("LoadDungeonMap", SavePath, new string[] {"dungeon files", "json" });
			if (!string.IsNullOrEmpty(loadPath)) {
				string jsonText = File.ReadAllText(loadPath);
				mDungeon = JsonUtility.FromJson<Dungeon>(jsonText);
				mSelectFloorIndex = 0;

				mFileName = Path.GetFileNameWithoutExtension(loadPath);
			}
		}

		pos.x = startPos.x;
		pos.y += pos.height + FieldInterval;
		pos.width *= 2.0f;

		// 新規作成ボタン
		if (GUI.Button(pos, "新規作成")) {
			mDungeon = null;
			Initialize();
			ResetTempParam();
		}
		pos.y += pos.height + FieldInterval;
		return new Vector2(pos.xMin, pos.yMax);
	}

	/// <summary>
	/// 指定座標の下にあるマスの番号を返す
	/// </summary>
	private bool TryGetCellPosition(Vector2 pos, out int x, out int y)
	{
		if (SelectFloor == null) {
			x = y = 0;
			return false;
		}
		Rect leftTopRect = GetMapCellRect(0, 0);
		Vector2 mapLocalPos = new Vector2(pos.x - leftTopRect.xMin, pos.y - leftTopRect.yMin);
		x = Mathf.FloorToInt(mapLocalPos.x / CellWidth);
		y = Mathf.FloorToInt(mapLocalPos.y / CellHeight);

		if (x < 0 || y < 0 || x >= SelectFloor.Width || y >= SelectFloor.Height) {
			// マップ外
			return false;
		}
		return true;
	}

	/// <summary>
	/// マップ描画
	/// </summary>
	private void DrawMap()
	{
		// 描画領域塗りつぶし
		Rect areaRect = GetMapDrawRect();
		Handles.DrawSolidRectangleWithOutline(areaRect, new Color(0.2f, 0.2f, 0.2f), Color.white);

		if (SelectFloor == null) {
			return;
		}
		// マス描画
		for (int y = 0; y < SelectFloor.Height; y++) {
			for (int x = 0; x < SelectFloor.Width; x++) {
				Texture2D tex = null;
				var cell = SelectFloor.GetCell(x, y);
				switch (cell.CellType) {
					case Dungeon.Cell.ECellType.Empty:
						tex = mTipTextures[(int)ETipTexture.Empty].Texture;
						break;
					case Dungeon.Cell.ECellType.Wall:
						tex = mTipTextures[(int)ETipTexture.Wall].Texture;
						break;
					case Dungeon.Cell.ECellType.EntryPoint:
						tex = mTipTextures[(int)ETipTexture.EntryPoint].Texture;
						break;
					case Dungeon.Cell.ECellType.TreasureBox:
						tex = mTipTextures[(int)ETipTexture.TreasureBox].Texture;
						break;
					case Dungeon.Cell.ECellType.Steps:
						if (cell.Values[0] == (int)Dungeon.Cell.EStepsValue.Up) {
							tex = mTipTextures[(int)ETipTexture.UpSteps].Texture;
						} else {
							tex = mTipTextures[(int)ETipTexture.DownSteps].Texture;
						}
						break;
					case Dungeon.Cell.ECellType.Boss:
						tex = mTipTextures[(int)ETipTexture.Boss].Texture;
						break;
				}
				GUI.DrawTexture(GetMapCellRect(x, y), tex);
			}
		}
	}

	// グリッド線を描画
	private void DrawGridLine()
	{
		if (SelectFloor == null) {
			return;
		}
		Color lineColor	= new Color(1.0f, 1.0f, 1.0f);
		Color dotterLineColor = new Color(0.8f, 0.8f, 0.8f);

		Rect leftTopRect = GetMapCellRect(0, 0);
		Rect rightBottomRect = GetMapCellRect(SelectFloor.Width - 1, SelectFloor.Height - 1);
		// 縦線
		for (int i = 0 ; i < SelectFloor.Width + 1 ; i++) {
			var p1 = new Vector2(leftTopRect.xMin + CellWidth * i,	leftTopRect.yMin);
			var p2 = new Vector2(leftTopRect.xMin + CellWidth * i,	rightBottomRect.yMax);
			if (i % MapDotterLineCount == 0) {
				Handles.color = lineColor;
				
			} else {
				Handles.color = dotterLineColor;
			}
			Handles.DrawLine(p1, p2);
		}
		// 横線
		for (int i = 0 ; i < SelectFloor.Height + 1 ; i++) {
			var p1 = new Vector2(leftTopRect.xMin,		leftTopRect.yMin + CellHeight * i);
			var p2 = new Vector2(rightBottomRect.xMax,	leftTopRect.yMin + CellHeight * i);
			if (i % MapDotterLineCount == 0) {
				Handles.color = lineColor;
				
			} else {
				Handles.color = dotterLineColor;
			}
			Handles.DrawLine(p1, p2);
		}

	}

	/// <summary>
	/// マップ描画範囲を取得
	/// </summary>
	private Rect GetMapDrawRect()
	{
		return new Rect(MapStartX, MapStartY, ControlRect.width * MapRectWidth, ControlRect.height * MapRectHeight);
	}

	/// <summary>
	/// 指定位置（セル単位）の描画領域（ピクセル単位）を取得
	/// </summary>
	private Rect GetMapCellRect(int x, int y)
	{
		return new Rect(MapStartX + CellWidth * x, MapStartY + CellHeight * y, CellWidth, CellHeight);
	}

	private float GetWidth(float rate)
	{
		return ControlRect.width * rate;
	}
	private float GetHeight(float rate)
	{
		return ControlRect.height * rate;
	}

	private void ApplyParam()
	{
		if (SelectFloor != null) {
			SelectFloor.SetSize(mTmpDungeonWidth, mTmpDungeonHeight);
			ResetTempParam();
		}
	}
	private void ResetTempParam()
	{
		if (SelectFloor != null) {
			mTmpDungeonWidth	= SelectFloor.Width;
			mTmpDungeonHeight	= SelectFloor.Height;
		}
	}

	/// <summary>
	/// 初期化系
	/// </summary>
	private void Initialize()
	{
		if (mTexture == null) {
			mTexture = Resources.Load<Texture2D>("MapTip");
		}
		if (mTipTextures == null) {
			mTipTextures = new TipTexture[(int)ETipTexture.Max];
			mTipTextures[(int)ETipTexture.Empty]		= new TipTexture(mTexture, "Empty", Dungeon.Cell.ECellType.Empty, 0);
			mTipTextures[(int)ETipTexture.Wall]			= new TipTexture(mTexture, "Wall", Dungeon.Cell.ECellType.Wall, 0);
			mTipTextures[(int)ETipTexture.Boss]			= new TipTexture(mTexture, "Boss", Dungeon.Cell.ECellType.Boss, 0);
			mTipTextures[(int)ETipTexture.EntryPoint]	= new TipTexture(mTexture, "Player", Dungeon.Cell.ECellType.EntryPoint, 0);
			mTipTextures[(int)ETipTexture.UpSteps]		= new TipTexture(mTexture, "UpSteps", Dungeon.Cell.ECellType.Steps, (int)Dungeon.Cell.EStepsValue.Up);
			mTipTextures[(int)ETipTexture.DownSteps]	= new TipTexture(mTexture, "DownSteps", Dungeon.Cell.ECellType.Steps, (int)Dungeon.Cell.EStepsValue.Down);
			mTipTextures[(int)ETipTexture.TreasureBox]	= new TipTexture(mTexture, "TreasureBox1", Dungeon.Cell.ECellType.TreasureBox, 0);
		}
		if (mDungeon == null) {
			mDungeon = new Dungeon();
			mSelectFloorIndex = 0;
		}
	}

	/// <summary>
	/// 解放系
	/// </summary>
	private void Release()
	{
		if (mTipTextures != null) {
			foreach (var tip in mTipTextures) {
				tip.Destory();
			}
			mTipTextures = null;
		}
	}


	/// <summary>
	/// マップチップテクスチャ
	/// </summary>
	private class TipTexture
	{
		private const int Width = 32;
		private const int Height = 32;
		
		private string					mName;

		/// <summary> テクスチャ </summary>
		public Texture2D				Texture { get; private set; }
		public Dungeon.Cell.ECellType	CellType { get; private set; }
		public int						CellValue { get; private set; }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TipTexture(Texture2D texture, string name, Dungeon.Cell.ECellType cellType, int value)
		{
			mName = name;
			CellType = cellType;
			CellValue = value;

			Texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);

			int x = 0;
			int y = 0;
			switch (cellType) {
			case Dungeon.Cell.ECellType.Empty:
				break;
			case Dungeon.Cell.ECellType.Wall:
				x = 1;
					break;
			case Dungeon.Cell.ECellType.Steps:
				if (value == (int)Dungeon.Cell.EStepsValue.Up)
				{
					x = 2;
				}
				else
				{
					x = 3;
				}
				break;

			case Dungeon.Cell.ECellType.TreasureBox:
				x = 5;
				break;

			case Dungeon.Cell.ECellType.Boss:
				x = 8;
				break;
			}
			x *= Width;
			y *= Height;

			for (int j = 0; j < Height; j++) {
				for (int i = 0; i < Width; i++)	{
					Texture.SetPixel(i, j, texture.GetPixel(x + i, y + j));
				}
			}
			Texture.Apply();


			Debug.Assert(Texture != null);
		}

		/// <summary>
		/// 解放
		/// </summary>
		public void Destory()
		{
			if (Texture != null) {
				Texture = null;
			}
		}
	}
}
