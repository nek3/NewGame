using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetBundleLoader : SingletonMonoBehaviour<AssetBundleLoader>
{
	public bool IsSimulationMode	{ get; set; }

	private Dictionary<string, IAssetRequest>	mAssetRequestList = new Dictionary<string,IAssetRequest>();

	/// <summary>
	/// 初期化
	/// </summary>
	/// <returns></returns>
	public bool Initialize()
	{
		IsSimulationMode	= true;
		return true;
	}

	/// <summary>
	/// アセットバンドルからアセットをロード
	/// </summary>
	public void LoadAsset(string assetBundleName, string assetName, Action<UnityEngine.Object> onEnded = null)
	{
		// アセットバンドルロード
		LoadAssetBundle(assetBundleName, bundle => {
			// アセットロード
			string key = string.Format("{0}/{1}", assetBundleName, assetName);
			IAssetRequest req = null;
			if (mAssetRequestList.TryGetValue(key, out req)) {
				// 登録済み
				req.AddEndedRequest(onEnded);
				return;
			}
#if UNITY_EDITOR
			if (IsSimulationMode) {
				req = new RequestAssetForSimulateMode();
			}
#endif
			if (req == null) {
				req = new RequestAsset();
			}
			req.Start(bundle, assetBundleName, assetName, onEnded);
			mAssetRequestList[key] = req;
		});

	}

	/// <summary>
	/// アセットバンドルをロード
	/// </summary>
	public void LoadAssetBundle(string assetBundleName, Action<AssetBundle> onEnded = null)
	{
#if UNITY_EDITOR
		if (IsSimulationMode) {
			if (onEnded != null) {
				onEnded(null);
			}
			return;
		}
#endif
		//TODO StreamingAssetsからのロード処理を実装する事
	}

	/// <summary>
	/// 更新
	/// </summary>
	public void Update () 
	{
		List<string> deleteKeyList = null;
		foreach (KeyValuePair<string, IAssetRequest> pair in mAssetRequestList) {
			if (pair.Value.Update()) {
				// 終了
				if (deleteKeyList == null) {
					deleteKeyList = new List<string>();
				}
				deleteKeyList.Add(pair.Key);
			}
		}
		if (deleteKeyList != null) {
			for (int i = 0 ; i < deleteKeyList.Count ; i++) {
				mAssetRequestList.Remove(deleteKeyList[i]);
			}
		}
	}



#if false
	private class RequestAssetBundle
	{
		private WWW		mRequest;
		private string	mAssetBundleName;

		public RequestAssetBundle(string assetBundleName, Action<AssetBundle> onEnded)
		{
			mAssetBundleName	= assetBundleName;
			
			string path = string.Format("file:///{0}/{1}", PathConst.GetStreamingAssetsPath(), assetBundleName);
			mRequest	= WWW.LoadFromCacheOrDownload(path, 0);

		}
	}
#endif


	private interface IAssetRequest
	{
		/// <summary> ロード開始 </summary>
		void Start(AssetBundle assetBundle, string assetBundleName, string assetName, Action<UnityEngine.Object> onEnded);

		/// <summary> ロード中のフレーム更新 </summary>
		/// <returns> ロード状態じゃなくなった場合にtrueを返す </returns>
		bool Update();

		/// <summary> 終了リクエスト追加 </summary>
		void AddEndedRequest(Action<UnityEngine.Object> onEnded);
	}

	/// <summary>
	/// アセットロードリクエスト
	/// </summary>
	private class RequestAsset : IAssetRequest
	{
		public AssetBundleRequest Request	{ get; protected set; }

		private bool						mIsLoading;
		private Action<UnityEngine.Object>	mEnded	= delegate{};

		/// <summary> 終了リクエスト追加 </summary>
		public void AddEndedRequest(Action<UnityEngine.Object> onEnded)
		{
			mEnded += onEnded;
		}

		/// <summary>
		/// ロード開始
		/// </summary>
		public void Start(AssetBundle assetBundle, string assetBundleName, string assetName, Action<UnityEngine.Object> onEnded)
		{
			DebugUtil.Assert(!mIsLoading);
			
			mEnded	+= onEnded;
			Request = assetBundle.LoadAssetAsync(assetName);
			DebugUtil.Assert(Request != null);
			mIsLoading	= true;
		}

		/// <summary>
		/// ロード中のフレーム更新
		/// </summary>
		/// <returns> ロード状態じゃなくなった場合にtrueを返す </returns>
		public bool Update()
		{
			if (!mIsLoading) {
				return true;
			}
			DebugUtil.Assert(Request != null);

			try {
				if (Request.isDone && Request.asset != null) {
					//XXX mizoguchi AssetBundle.LoadAsset系がキャッシュしてくれるので、自前でキャッシュ処理はしない
					// ロード終了イベント
					if (mEnded != null) {
						mEnded(Request.asset);
					}
					mIsLoading = false;
					return true;
				}
			} catch (System.NullReferenceException ex) {
				DebugUtil.Log(ex.Source);
				DebugUtil.Log(ex.StackTrace);
				mIsLoading = false;
			}
			return false;
		}
	}

#if UNITY_EDITOR
	/// <summary>
	/// シミュレートモード時のアセットロード
	/// </summary>
	private class RequestAssetForSimulateMode : IAssetRequest
	{
		private bool						mIsLoading;
		private string						mAssetName;
		private string						mAssetBundleName;

		private Action<UnityEngine.Object>	mEnded	= delegate{};

		/// <summary> 終了リクエスト追加 </summary>
		public void AddEndedRequest(Action<UnityEngine.Object> onEnded)
		{
			mEnded += onEnded;
		}

		/// <summary>
		/// ロード開始
		/// </summary>
		public void Start(AssetBundle assetBundle, string assetBundleName, string assetName, Action<UnityEngine.Object> onEnded)
		{
			DebugUtil.Assert(!mIsLoading);
			
			mIsLoading			= true;
			mAssetName			= assetName;
			mAssetBundleName	= assetBundleName;
			mEnded				+= onEnded;
		}

		/// <summary>
		/// ロード中のフレーム更新
		/// </summary>
		/// <returns> ロード状態じゃなくなった場合にtrueを返す </returns>
		public bool Update()
		{
			if (!mIsLoading) {
				return true;
			}

			string assetName = System.IO.Path.GetFileNameWithoutExtension(mAssetName);
			string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(mAssetBundleName, assetName);
			if (assetPaths == null || assetPaths.Length <= 0) {
				// エラー
				DebugUtil.LogFormat("Asset '{0}' is simulate mode loading error. BundleName:{1}", mAssetName, mAssetBundleName);
				if (mEnded != null) {
					mEnded(null);
				}
				mIsLoading	= false;
				return true;
			}
			UnityEngine.Object loadedObject = null;

			if (assetPaths.Length == 1) {
				loadedObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPaths[0]);
			} else {
				// 複数見つかったパスの中から拡張子まで一致している物を検索
				foreach (var path in assetPaths) {
					if (path.IndexOf(mAssetName) >= 0) {
						loadedObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
						break;
					}
				}
			}
			if (loadedObject != null) {
				if (mEnded != null) {
					mEnded(loadedObject);
				}
				mIsLoading	= false;
			} else {
				DebugUtil.Assert(false);
			}

			return true;
		}
	}
#endif//UNITY_EDITOR
}
