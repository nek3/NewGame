using UnityEngine;
using System.Collections;

public static class SaveData
{
	private const string DungeonSaveKey	= "GuMasDgn";

	/// <summary>
	/// 初期化
	/// </summary>
	public static void Initialize()
	{
		DungeonSaveData.Load();
	}

	/// <summary>
	/// データ取得
	/// </summary>
	public static string GetSaveData(string key)
	{
		if (PlayerPrefs.HasKey(key)) {
			return PlayerPrefs.GetString(key);
		}
		return string.Empty;
	}

	/// <summary>
	/// データ設定
	/// </summary>
	public static void SetSaveData(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
	}
}

