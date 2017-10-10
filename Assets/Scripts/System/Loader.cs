using UnityEngine;
using System.Collections;
using System;
using System.IO;

/// <summary>
/// ローダ
/// </summary>
public class Loader : SingletonMonoBehaviour<Loader>
{
	/// <summary>
	/// プレハブをロードする
	/// </summary>
	public void LoadPrefab(string path, Action<GameObject> onLoaded)
	{
		DebugUtil.NullAssert(onLoaded);

		string assetBundleName	= AssetBundleName.GetAssetBundleNameFromPath(path);
		string assetName		= Path.GetFileName(path);
		AssetBundleLoader.Instance.LoadAsset(assetBundleName, assetName, loadedObj => {
			onLoaded(loadedObj as GameObject);
		});
	}
}
