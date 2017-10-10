using UnityEngine;
using System.Collections;

public class Vector2<T>
{
	public T x { get; set; }
	public T y { get; set; }

	/// <summary> 値をセット </summary>
	public Vector2<T> Set(T valX, T valY)
	{
		x = valX;
		y = valY;
		return this;
	}

	/// <summary>
	/// 現在のインスタンスをコピーして新しいインスタンスを返す
	/// </summary>
	/// <returns>新しく作成されたインスタンス</returns>
	public Vector2<T> CopyTo()
	{
		return new Vector2<T>().CopyFrom(this);
	}

	/// <summary> コピー </summary>
	public Vector2<T> CopyFrom(Vector2<T> src)
	{
		x = src.x;
		y = src.y;
		return this;
	}
}
