using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

/// <summary>
/// アセットバンドル名の自動生成ツール
/// </summary>
public class AssetBundleName
{
	private class NameSetting
	{
		public string		BundleName	{ get; private set; }
		public string[]		Dirs		{ get; private set; }

		public NameSetting(string bundleName, params string [] dirPathList)
		{
			BundleName		= bundleName;
			DebugUtil.Assert(dirPathList.Length > 0);

			Dirs = new string[dirPathList.Length];
			dirPathList.CopyTo(Dirs, 0);
		}
	}
	private static List<NameSetting> msNameDic = new List<NameSetting>() {
		new NameSetting("hero/prefab/cos/female",				"Little Heroes Mega Pack/Prefabs/01 Choose Costume/Female"),
		new NameSetting("hero/prefab/cos/male",					"Little Heroes Mega Pack/Prefabs/01 Choose Costume/Male"),
		new NameSetting("hero/prefab/hair/female",				"Little Heroes Mega Pack/Prefabs/02 Attach Hair/Female"),
		new NameSetting("hero/prefab/hair/male",				"Little Heroes Mega Pack/Prefabs/02 Attach Hair/Male"),
		new NameSetting("hero/prefab/acce/female",				"Little Heroes Mega Pack/Prefabs/03 Attach Accessories/Female"),
		new NameSetting("hero/prefab/acce/male",				"Little Heroes Mega Pack/Prefabs/03 Attach Accessories/Male"),
		new NameSetting("hero/prefab/weapon/arrows",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Arrows"),					
		new NameSetting("hero/prefab/weapon/axes",				"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Axes (R Arm)"),				
		new NameSetting("hero/prefab/weapon/bells",				"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Bells (R Arm )"),			
		new NameSetting("hero/prefab/weapon/claws",				"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Claws (Both Arms)"),			
		new NameSetting("hero/prefab/weapon/clubs",				"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Clubs (R Arm)"),				
		new NameSetting("hero/prefab/weapon/crossbows",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Crossbows (L Arm)"),			
		new NameSetting("hero/prefab/weapon/daggers",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Daggers (R Arm)"),			
		new NameSetting("hero/prefab/weapon/hammers",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Hammers (R Arm)"),			
		new NameSetting("hero/prefab/weapon/handauras",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Hand Auras (Both Arms)"),	
		new NameSetting("hero/prefab/weapon/knuckles",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Knuckles (Both Arms)"),		
		new NameSetting("hero/prefab/weapon/maces",				"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Maces (R Arm)"),				
		new NameSetting("hero/prefab/weapon/peasant",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Peasant Tools (R Arm)"),		
		new NameSetting("hero/prefab/weapon/quivers",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Quivers (Back)"),			
		new NameSetting("hero/prefab/weapon/scepters",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Scepters (R Arm)"),			
		new NameSetting("hero/prefab/weapon/scythes",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Scythes (R Arm)"),			
		new NameSetting("hero/prefab/weapon/shields",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Shields (L Arm)"),			
		new NameSetting("hero/prefab/weapon/spears",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Spears (R Arm)"),			
		new NameSetting("hero/prefab/weapon/swords",			"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Swords (R Arm)"),			
		new NameSetting("hero/prefab/weapon/twohandswords",		"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Two Handed Swords (R Arm)"),	
		new NameSetting("hero/prefab/weapon/wands",				"Little Heroes Mega Pack/Prefabs/04 Attach Weapons/Wands (R Arm)"),				
		// Master
		new NameSetting(MasterDataManager.AssetBundleName,		"AssetBundle/MasterData"),
	};


#if UNITY_EDITOR
	/// <summary>
	/// アセットの追加、更新時に呼ばれる
	/// </summary>
	public static void OnPostprocessAllAssets(string assetPath, AssetImporter importer)
	{
		if (importer is MonoImporter) {
			return;
		}
		// アセットバンドル名を設定.
		importer.assetBundleName = GetAssetBundleNameFromPath(assetPath);
	}

	/// <summary>
	/// アセットバンドル名を設定
	/// </summary>
	public static void SetupAssetBundleName()
	{
		var nameCount = new Dictionary<string,string>();

		// 各アセットに対してアセットバンドル名を設定する.
		string[] allAssets = AssetDatabase.GetAllAssetPaths();
		int index = 0;
		foreach (string path in allAssets) {
			// 進行状況.
			float progress = (float)++index / allAssets.Length;
			EditorUtility.DisplayProgressBar("SetupAssetBundleName", string.Format("[{0:D5}/{1:D5}] : {2}", index, allAssets.Length, Path.GetFileNameWithoutExtension(path)), progress);

			AssetImporter importer = AssetImporter.GetAtPath(path);
			if (importer is MonoImporter) {
				// スクリプトは除外
				continue;
			}

			// アセットバンドル名を設定
			importer.assetBundleName = GetAssetBundleNameFromPath(path);	// アセットバンドル対象外の場合は、string.Emptyが返る

			if (importer.assetBundleName != string.Empty) {
				string fileName = Path.GetFileName(importer.assetBundleName);

				string assetBundleName;
				if (nameCount.TryGetValue(fileName, out assetBundleName)) {
					if (assetBundleName != importer.assetBundleName) {
						// ファイル名の重複
						DebugUtil.LogWarningFormat("{0} is duplicate name\n{1}\n{2}", fileName, assetBundleName, importer.assetBundleName);
					}
				}
				nameCount[fileName] = importer.assetBundleName;
				importer.SaveAndReimport();
			}
		}

		// 未使用アセットバンドル名を削除.
		AssetDatabase.RemoveUnusedAssetBundleNames();
		AssetDatabase.SaveAssets();

		EditorUtility.ClearProgressBar();
	}
#endif//UNITY_EDITOR

	/// <summary>
	/// パスからアセットバンドル名を取得する
	/// </summary>
	public static string GetAssetBundleNameFromPath(string path)
	{
		// "Assets"はパスから除外
		path = System.Text.RegularExpressions.Regex.Replace(path, @"^Assets/", string.Empty);

		for (int i = 0 ; i < msNameDic.Count ; i++) {
			var setting = msNameDic[i];

			for (int j = 0 ; j < setting.Dirs.Length ; j++) {
				if (path.IndexOf(setting.Dirs[j]) == 0) {
					// 一致した時点で終了。重複するパスを指定する場合は、深い物を先に設定する事
					return setting.BundleName;
				}
			}
		}
		return string.Empty;
	}
}
