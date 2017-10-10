using UnityEngine;
using System.Collections;
using UnityEditor;

public static class MyMenu
{
	public const string MenuRoot	= "MyMenu";
	/// <summary>
	/// アセットバンドルのシミュレーションON/OFF
	/// </summary>
	[MenuItem(MenuRoot + "/アセットバンドル名再設定", false, 100)]
	public static void SetupAssetBundleName()
	{
		AssetBundleName.SetupAssetBundleName();
	}

	[MenuItem(MenuRoot + "/ヒーローデータ作成", false, 200)]
	public static void CreateHeroData()
	{
		var createData = new CreateData();
		createData.CreateCostumeMaster();
		createData.CreateHairMaster();
	}

	[MenuItem(MenuRoot + DungeonEditorWindow.MenuName, false, 300)]
	public static void OpenDungeonEditor()
	{
		EditorWindow.GetWindow<DungeonEditorWindow>(false, "DungeonEditor", true);
	}
}
