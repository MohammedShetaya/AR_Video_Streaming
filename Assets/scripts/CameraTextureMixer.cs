using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; 
public class CameraTextureMixer : MonoBehaviour
{

	[SerializeField]
	private RenderTexture ARTexture; 

	private void Awake()
	{
		/**ARCameraManager cameraManager = GetComponent<ARCameraManager>();
		ARCameraBackground ARBackground = GetComponent<ARCameraBackground>();

		cameraManager.frameReceived += (e) =>
		{
			foreach (var t in e.textures)
			{
				Graphics.Blit(t, ARTexture, ARBackground.material);
			}
		};*/

	}

	private void OnPreRender()
	{
		Camera arCamera = GetComponent<Camera>();
		arCamera.targetTexture = ARTexture ;

	}

	private void OnPostRender()
	{
		Camera arCamera = GetComponent<Camera>();
		arCamera.targetTexture = null;
		Graphics.Blit(ARTexture, null as RenderTexture);
		RenderTexture.ReleaseTemporary(ARTexture);
	}
}
