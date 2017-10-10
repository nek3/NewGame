using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class MasterDataManager
{
	/// <summary> マスタデータ用アセットバンドル名 </summary>
	public const string	AssetBundleName	= "masterdata";
	/// <summary> マスタデータファイル拡張子 </summary>
	private const string FileExtension	= ".json";

	private static MasterDataManager	msInstance;

	private static readonly List<Type>	mMasterTypeList = new List<Type>() {
		typeof(HeroCostumeContainer),
		typeof(HeroHairContainer),
	};

	private Dictionary<Type, MasterDataContainerBase> mMasterDic = new Dictionary<Type, MasterDataContainerBase>();



	private MasterDataManager()
	{
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="onEnded"></param>
	public static void Initialize(Action onEnded)
	{
		if (msInstance != null) {
			onEnded();
			return;
		}
		msInstance = new MasterDataManager();

		TaskManager.ITaskGroup group = TaskManager.Instance.CreateAsyncTaskGroup();
		foreach (var type in mMasterTypeList) {
			Type t = type;
			group.AddTask(onTaskEnded => {
				msInstance.LoadMasterData(t, onTaskEnded);
			});
		}
		group.Run(onEnded);
	}

	/// <summary>
	/// マスタデータ取得
	/// </summary>
	public static T GetMaster<T>() where T : class
	{
		DebugUtil.NullAssert(msInstance);
		return msInstance.mMasterDic[typeof(T)] as T;
	}
	/// <summary>
	/// マスタデータロード
	/// </summary>
	private void LoadMasterData(Type type, Action onLoaded)
	{
		string assetName		= type.ToString().Replace("Container", string.Empty) + FileExtension;
		AssetBundleLoader.Instance.LoadAsset(AssetBundleName, assetName, loadedObj => {
			var textAsset = loadedObj as TextAsset;
			Type t = Type.GetType(type.ToString());
			var container = JsonUtility.FromJson(textAsset.text, t);
			mMasterDic[type] = container as MasterDataContainerBase;
			if (onLoaded != null) {
				onLoaded();
			}
		});
	}

	/// <summary>
	/// マスタデータインターフェイス
	/// </summary>
	public interface IMasterData
	{
		int Id { get; set; }
	}

	/// <summary>
	/// マスタデータコンテナベースクラス
	/// </summary>
	[Serializable]
	public abstract class MasterDataContainerBase
	{
	}
}
