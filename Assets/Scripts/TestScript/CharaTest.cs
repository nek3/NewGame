using UnityEngine;
using System.Collections;

public class CharaTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AssetBundleLoader.Instance.Initialize();

		Hero.CreateHero("Hero1", 3, 10);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
