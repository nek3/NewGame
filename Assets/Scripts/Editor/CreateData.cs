using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class CreateData
{
	/// <summary>
	/// コスチュームマスタデータの作成
	/// </summary>
	public void CreateCostumeMaster()
	{
		string assetPath = Application.dataPath + "/Little Heroes Mega Pack/Prefabs/01 Choose Costume";

		HeroCostumeContainer container	= new HeroCostumeContainer();

		int id = 1;
		foreach (var path in Directory.GetFiles(assetPath, "*.prefab", SearchOption.AllDirectories)) {
			var localPath = path.Replace(assetPath + @"\", string.Empty);

			string [] splits = localPath.Split(new char[] {'/', '\\'});

			string gender	= splits[0];
			string cos		= splits[1];

			Constant.EGenderType genderType			= (Constant.EGenderType)Enum.Parse(typeof(Constant.EGenderType), gender);
			HeroCostume.ECostumeType costumeType	= (HeroCostume.ECostumeType)Enum.Parse(typeof(HeroCostume.ECostumeType), cos);
			container.Container.Add(new HeroCostume(id++, costumeType, genderType, localPath));
		}

		// JSON化
		var jsonText = JsonUtility.ToJson(container);
		File.WriteAllText(Application.dataPath + "/AssetBundle/MasterData/HeroCostume.json", jsonText);
	}

	/// <summary>
	/// 髪マスタデータ作成
	/// </summary>
	public void CreateHairMaster()
	{
		string assetPath = Application.dataPath + "/Little Heroes Mega Pack/Prefabs/02 Attach Hair";

		var container	= new HeroHairContainer();

		int id = 1;
		foreach (var path in Directory.GetFiles(assetPath, "*.prefab", SearchOption.AllDirectories)) {
			var localPath = path.Replace(assetPath + @"\", string.Empty);

			string [] splits = localPath.Split(new char[] {'/', '\\'});
			string gender	= splits[0];

			// カラーはファイル名をスペース区切りした最後の単語
			string [] fnameSplits	= Path.GetFileNameWithoutExtension(localPath).Split(new char[] {' '});
			string color			= fnameSplits[fnameSplits.Length-1];

			Constant.EGenderType genderType			= (Constant.EGenderType)Enum.Parse(typeof(Constant.EGenderType), gender);

			HeroHair.EHairColorType colorType;
			if (Enum.IsDefined(typeof(HeroHair.EHairColorType), color)) {
				colorType = (HeroHair.EHairColorType)Enum.Parse(typeof(HeroHair.EHairColorType), color);
			} else {
				colorType = HeroHair.EHairColorType.Etc;
				Debug.LogFormat("Unknown hair color type = {0}", color);
			}
			container.Container.Add(new HeroHair(id++, colorType, genderType, localPath));
		}

		// JSON化
		var jsonText = JsonUtility.ToJson(container);
		File.WriteAllText(Application.dataPath + "/AssetBundle/MasterData/HeroHair.json", jsonText);
	}


	private void CreateCostumeEnum()
	{
	}
}
