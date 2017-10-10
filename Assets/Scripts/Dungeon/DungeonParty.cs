using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonParty : MonoBehaviour
{
	private List<Hero>		mPartyMembers = new List<Hero>();

	/// <summary>
	/// パーティメンバーを追加
	/// </summary>
	public void AddPartyMember(Hero hero)
	{
		mPartyMembers.Add(hero);
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
