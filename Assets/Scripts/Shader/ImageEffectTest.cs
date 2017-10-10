using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffectTest : MonoBehaviour {

	private Material	mMaterial;
	[SerializeField]
	private bool		mUpdate = false;
	[SerializeField]
	private float		mDuration = 0.0f;
	[SerializeField]
	private float		mSpeed	= 0.5f;

	void Awake()
	{
		var shader = Shader.Find("Hidden/ImageEffectTest");
		mMaterial = new Material(shader);

		mMaterial.SetFloat("_PixelRateX", 1.0f / Screen.width);
		mMaterial.SetFloat("_PixelRateY", 1.0f / Screen.height);

	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
			Vector3 pos = Input.mousePosition;
			
			Vector2 normalizePos = new Vector2(pos.x / Screen.width, pos.y / Screen.height);
			Debug.LogFormat("pos = {0}", normalizePos.ToString());
			StartEffect(normalizePos);
		}
	}

	public void StartEffect(Vector2 pos)
	{
		mMaterial.SetFloat("_CenterX", pos.x);
		mMaterial.SetFloat("_CenterY", pos.y);

		mDuration = 0.0f;
		mUpdate = true;
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!mUpdate) {
			Graphics.Blit(source, destination);
			return;
		}

		mDuration += (Time.deltaTime * (1.0f / mSpeed));
		mDuration = Mathf.Clamp(mDuration, 0.0f, 1.0f);
		if (mDuration >= 1.0f) {
			mDuration = 1.0f;
			mUpdate = false;
		}
		mMaterial.SetFloat("_Duration", mDuration);

		Graphics.Blit(source, destination, mMaterial);
	}
}
