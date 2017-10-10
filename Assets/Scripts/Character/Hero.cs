using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour 
{
	private const string PrefabPathBase	= "Little Heroes Mega Pack/Prefabs/";
	private const string CostumeDir		= "01 Choose Costume/";
	private const string HairDir		= "02 Attach Hair/";
	private const string AccessoryDir	= "03 Attach Accessories/";
	private const string WeaponDir		= "04 Attach Weapons/";

	// アタッチ先
	private const string DummyHead		= "Dummy Prop Head";
	private const string DummyLeft		= "Dummy Prop Left";
	private const string DummyRight		= "Dummy Prop Right";
	private const string DummyBack		= "Dummy Prop Back";

	private GameObject	mRoot;
	private GameObject	mCostume;
	private GameObject	mHair;
	private GameObject	mWeapon;

	private HeroCostume	mCostumeMaster;
	private HeroHair	mHairMaster;

	/// <summary> ロードなどの準備が終わったかどうか </summary>
	public bool IsReady	{ get; set; }

	/// <summary>
	/// インスタンス作成
	/// </summary>
	public static Hero CreateHero(string name, int cosutumeId, int hairId)
	{
		var newObj			= new GameObject(name);
		var instance		= newObj.AddComponent<Hero>();
		instance.mRoot		= newObj;
		instance.IsReady	= false;

		instance.mCostumeMaster		= MasterDataManager.GetMaster<HeroCostumeContainer>().GetById(cosutumeId);
		instance.mHairMaster		= MasterDataManager.GetMaster<HeroHairContainer>().GetById(hairId);
		return instance;
	}

	// Use this for initialization
	void Start () {
		var group = TaskManager.Instance.CreateAsyncTaskGroup();

		group.AddTask(onTaskEnded => {
			// ベース作成
			string path = PrefabPathBase + CostumeDir + mCostumeMaster.PrefabPath;
			Loader.Instance.LoadPrefab(path, go => {
				mCostume = GameObject.Instantiate(go);
				mCostume.transform.localPosition = Vector3.zero;
				onTaskEnded();
			});
		});

		group.AddTask(onTaskEnded => {
			// 髪
			string path = PrefabPathBase + HairDir + mHairMaster.PrefabPath;
			Loader.Instance.LoadPrefab(path, go => {
				mHair = GameObject.Instantiate(go);
				onTaskEnded();
			});
		});


		group.AddTask(onTaskEnded => {
			// 武器
			string path = PrefabPathBase + WeaponDir + "Axes (R Arm)/Axe 01.prefab";
			Loader.Instance.LoadPrefab(path, go => {
				mWeapon = GameObject.Instantiate(go);
				onTaskEnded();
			});
		});

		group.Run(() => {
			Setup();
			IsReady = true;
		});	
	}

	private void Setup()
	{
		DebugUtil.NullAssert(mCostume);

		mCostume.transform.SetParent(mRoot.transform);
		if (mHair != null) {
			var head = mCostume.transform.FindDeep(DummyHead);
			DebugUtil.NullAssert(head);
			mHair.transform.SetParent(head);
			mHair.transform.localPosition = Vector3.zero;
		}
		if (mWeapon != null) {
			//TODO 武器によってアタッチ箇所を変える必要アリ
			var node = mCostume.transform.FindDeep(DummyRight);
			DebugUtil.NullAssert(node);
			mWeapon.transform.SetParent(node);
			mWeapon.transform.localPosition = Vector3.zero;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
