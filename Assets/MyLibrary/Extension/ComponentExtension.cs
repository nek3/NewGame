using UnityEngine;
using System.Collections;

public static class ComponentExtension
{
	/// <summary>
	/// 指定した名前のTransformを孫以下まで検索する
	/// </summary>
	public static Transform FindDeep(this Component self, string name, bool includeInactive = false)
	{
		Transform[] children = self.GetComponentsInChildren<Transform>(includeInactive);
		for (int i = 0 ; i < children.Length ; i++) {
			if (children[i].name == name) {
				return children[i];
			}
		}
		return null;
	}
}
