using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HeroWeaponType : ScriptableObject
{
	public enum EType {
		Arrow,
		Axe,
		Bell,
		Claw,
		Club,
		Crossbow,
		Dagger,
		Hammer,
		HandAura,
		Knuckle,
		Mace,
		PeasantTool,
		Quivers,
		Scepter,
		Scythe,
		Shield,
		Spear,
		Sword,
		TowHandedSword,
		Wand,
	}
	public enum EAttachType {
		LeftHand,
		RightHand,
		BothHand,
		Back
	}
	public static Dictionary<EType, Data>	DataList = new Dictionary<EType, Data>() {
		{ EType.Arrow,			new Data(EType.Arrow,		"Arrows",						EAttachType.Back) },
		{ EType.Axe,			new Data(EType.Axe,			"Axes (R Arm)",					EAttachType.RightHand)},
		{ EType.Bell,			new Data(EType.Bell,		"Bells (R Arm )",				EAttachType.RightHand)},
		{ EType.Claw,			new Data(EType.Claw,		"Claws (Both Arms)",			EAttachType.BothHand)},
		{ EType.Club,			new Data(EType.Club,		"Clubs (R Arm)",				EAttachType.RightHand)},
		{ EType.Crossbow,		new Data(EType.Crossbow,	"Crossbows (L Arm)",			EAttachType.LeftHand)},
		{ EType.Dagger,			new Data(EType.Dagger,		"Daggers (R Arm)",				EAttachType.RightHand)},
		{ EType.Hammer,			new Data(EType.Hammer,		"Hammers (R Arm)",				EAttachType.RightHand)},
		{ EType.HandAura,		new Data(EType.HandAura,	"Hand Auras (Both Arms)",		EAttachType.BothHand)},
		{ EType.Knuckle,		new Data(EType.Knuckle,		"Knuckles (Both Arms)",			EAttachType.BothHand)},
		{ EType.Mace,			new Data(EType.Mace,		"Maces (R Arm)",				EAttachType.RightHand)},
		{ EType.PeasantTool,	new Data(EType.PeasantTool,	"Peasant Tools (R Arm)",		EAttachType.RightHand)},
		{ EType.Quivers,		new Data(EType.Quivers,		"Quivers (Back)",				EAttachType.Back)},
		{ EType.Scepter,		new Data(EType.Scepter,		"Scepters (R Arm)",				EAttachType.RightHand)},
		{ EType.Scythe,			new Data(EType.Scythe,		"Scythes (R Arm)",				EAttachType.RightHand)},
		{ EType.Shield,			new Data(EType.Shield,		"Shields (L Arm)",				EAttachType.LeftHand)},
		{ EType.Spear,			new Data(EType.Spear,		"Spears (R Arm)",				EAttachType.RightHand)},
		{ EType.Sword,			new Data(EType.Sword,		"Swords (R Arm)",				EAttachType.RightHand)},
		{ EType.TowHandedSword,	new Data(EType.TowHandedSword,	"Two Handed Swords (R Arm)",	EAttachType.RightHand)},
		{ EType.Wand,			new Data(EType.Wand,			"Wands (R Arm)",				EAttachType.RightHand)},
	};

	/// <summary>
	/// 武器タイプデータを取得
	/// </summary>
	public static Data GetTypeData(EType type)
	{
		return DataList[type];
	}

	/// <summary>
	/// 1つ分のデータ
	/// </summary>
	public class Data {
		public EType		Type	{ get; private set; }
		public string		Folder	{ get; private set; }
		public EAttachType	Attach	{ get; private set; }

		public Data(EType type, string folder, EAttachType attach)
		{
			Type = type;
			Folder = folder;
			Attach = attach;
		}
	}
}
