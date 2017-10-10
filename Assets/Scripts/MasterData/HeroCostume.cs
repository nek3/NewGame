using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HeroCostume : MasterDataManager.IMasterData
{
	public enum ECostumeType
	{
		Archer,
		Barbarian,
		Casual,
		Knight,
		Naked,
		Sorceress,
		Wizard,
		Special,
	}

	[SerializeField]	private int						mId;
	[SerializeField]	private ECostumeType			mType;
	[SerializeField]	private Constant.EGenderType	mGender;
	[SerializeField]	private string					mPrefabPath;

	public int				Id		{ get { return mId; } set { mId = value; } }
	public ECostumeType		Type { get { return mType; } }
	public Constant.EGenderType	Gender { get {  return mGender; } }
	public string				PrefabPath { get { return mPrefabPath; } }

	public HeroCostume(int id, ECostumeType type, Constant.EGenderType gender, string path)
	{
		mId			= id;
		mType		= type;
		mGender		= gender;
		mPrefabPath	= path;
	}

}

[Serializable]
public class HeroCostumeContainer : MasterDataManager.MasterDataContainerBase
{
	[SerializeField]	protected List<HeroCostume>	mContainer	= new List<HeroCostume>();

	public List<HeroCostume>	Container { get { return mContainer; } }

	/// <summary>
	/// 指定IDのデータを取得
	/// </summary>
	public HeroCostume GetById(int id)
	{
		return mContainer.First(data => data.Id == id);
	}
}