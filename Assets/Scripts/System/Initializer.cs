using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Initializer : MonoBehaviour
{
	private bool	mInitialized	= false;
	
	/// <summary>
	/// 初期化開始処理
	/// </summary>
	void Start ()
	{
		DontDestroyOnLoad(gameObject);

		// 各種初期化
		TaskManager.ITaskGroup group = TaskManager.Instance.CreateSyncTaskGroup();

		AssetBundleLoader.Instance.Initialize();

		group.AddTask(onTaskEnded => {
			MasterDataManager.Initialize(onTaskEnded);
		});

		group.Run(() => {
			// セーブデータロード
			SaveData.Initialize();

			mInitialized	= true;
			SceneManager.LoadScene("CharaTest");
		});
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
