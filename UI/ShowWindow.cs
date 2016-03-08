using Fireworks.Loaders;
using Parkitect.UI;
using UnityEngine;

namespace Fireworks.UI
{
	class ShowWindow : UIWindow
	{
		private const string AUDIO_SLIDER_NAME = "AudioSlider";

		protected override void Start()
		{
			base.Start();
			GameObject sliderGO = AssetBundleLoader.LoadAsset(AssetBundleLoader.UI_ELEMENT_ASSETBUNDLE, AUDIO_SLIDER_NAME);
			sliderGO.transform.SetParent(transform, false);
		}
	}
}
