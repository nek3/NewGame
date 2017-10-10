using UnityEngine;
using System.Collections;

public static class PathConst 
{
	/// <summary>
	/// StreamingAseetsパスを取得
	/// </summary>
	public static string GetStreamingAssetsPath()
	{
		System.Text.StringBuilder path = new System.Text.StringBuilder();

#if	UNITY_EDITOR
//		if (/*IsSimulation*/true) {
			path.AppendFormat("{0}", System.Environment.CurrentDirectory.Replace(@"\", @"/"));
//		} else {
		//	path.AppendFormat("{0}", Application.streamingAssetsPath);
//		}
#elif UNITY_ANDROID
		path.Append(Application.streamingAssetsPath);
#else	// UNITY_IOS and Other
		path.AppendFormat("{0}", Application.streamingAssetsPath);
#endif

		return path.ToString();
	}
}
