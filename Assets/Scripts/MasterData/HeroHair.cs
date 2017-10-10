using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HeroHair : MasterDataManager.IMasterData
{
	public enum EHairColorType
	{
		Aqua,
		Black,
		Blonde,
		Blue,
		Brown,
		Cyan,
		Fire,
		Green,
		Grey,
		Magenta,
		Purple,
		Red,
		White,
		Yellow,


		Etc
	}

	[SerializeField]	private int						mId;
	[SerializeField]	private EHairColorType			mColorType;
	[SerializeField]	private Constant.EGenderType	mGender;
	[SerializeField]	private string					mPrefabPath;

	public int				Id				{ get { return mId; } set { mId = value; } }
	public EHairColorType	ColorType		{ get { return mColorType; } }
	public Constant.EGenderType	Gender		{ get { return mGender; } }
	public string				PrefabPath	{ get { return mPrefabPath; } }

	public HeroHair(int id, EHairColorType colorType, Constant.EGenderType gender, string path)
	{
		mId			= id;
		mColorType	= colorType;
		mGender		= gender;
		mPrefabPath	= path;
	}

}

[Serializable]
public class HeroHairContainer : MasterDataManager.MasterDataContainerBase
{
	[SerializeField]	protected List<HeroHair>	mContainer	= new List<HeroHair>();
	public List<HeroHair>	Container { get { return mContainer; } }

	/// <summary>
	/// 指定IDのデータを取得
	/// </summary>
	public HeroHair GetById(int id)
	{
		return mContainer.First(data => data.Id == id);
	}

}