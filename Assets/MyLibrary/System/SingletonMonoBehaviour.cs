using UnityEngine;
using System.Collections;

///	<summary>
///	シングルトンMonoBehaviour。
///	Awake()またはCheckInstance()を Instance より先に呼んでおくと、オブジェクト検索コストが無くなる
///	</summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour 
	where T : MonoBehaviour
{
	private static T	msInstance;
	/// <summary>
	/// インスタンス
	/// </summary>
	public static T Instance {
		get {
			if (msInstance == null) {
				msInstance = FindObjectOfType<T>();
				if (msInstance == null) {
					DebugUtil.LogFormat("SingletonMonoBehaviour<{0}> is not found.", typeof(T).ToString());
					// 無い場合は作る
					GameObject go = new GameObject(typeof(T).ToString());
					msInstance = go.AddComponent<T>();
				}
			}
			return msInstance;
		}
	}

	/// <summary>
	/// インスタンス取得
	/// ※自動でインスタンス生成を行わない
	/// </summary>
	/// <returns>インスタンスが存在する場合はインスタンスを返す。</returns>
	public static T GetInstance()
	{
		return msInstance;
	}

	/// <summary>
	/// 型を指定してインスタンス取得
	/// ※自動でインスタンス生成を行わない
	/// </summary>
	/// <typeparam name="TClass">取得したいインスタンスのクラス型</typeparam>
	/// <returns>インスタンスが存在し、指定した型の場合にはインスタンスを返す。その他の場合はnullを返す。</returns>
	public static TClass GetInstance<TClass>() where TClass : class
	{
		return msInstance as TClass;
	}

	/// <summary>
	/// 起動時
	/// </summary>
	public virtual void Awake()
	{
		CheckInstance();
	}

	/// <summary>
	/// シングルトンインスタンスのチェック
	/// </summary>
	/// <returns>成否を返す</returns>
	protected bool CheckInstance()
	{
		if (msInstance == null) {
			msInstance = this as T;
			return true;
		} else if (msInstance == this) {
			return true;
		}
		DebugUtil.Assert(false, "SingletonMonoBehaviour is not singleton.");
		return false;
	}

	/// <summary>
	/// 削除時
	/// </summary>
	protected virtual void OnDestroy()
	{
		if (msInstance == this) {
			msInstance = null;
		}
	}
}
