using Fireworks.Loaders;
using Fireworks.Music;
using Parkitect.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fireworks.UI
{
	class ShowWindow : UIWindow
	{
		private const string AUDIO_SLIDER_NAME = "AudioSlider";

		private AudioSource sliderSource;
		private AudioClip clip;
		private Slider scrubSlider;

		protected override void Start()
		{
			base.Start();
			GameObject sliderGO = AssetBundleLoader.LoadAsset(AssetBundleLoader.UI_ELEMENT_ASSETBUNDLE, AUDIO_SLIDER_NAME);
			sliderGO.transform.SetParent(transform, false);
			scrubSlider = sliderGO.GetComponentInChildren<Slider>();
			sliderSource = sliderGO.GetComponentInChildren<AudioSource>();
			StartCoroutine(LoadClip());
		}

		private IEnumerator LoadClip()
		{
			WWW www = MusicFileLoader.GetClipFromFile("indy.wav");
			if (www == null)
			{
				Debug.Log("WWW was null!");
			}
			clip = www.audioClip;
			if (clip == null)
			{
				Debug.Log("WWW did not return clip! " + www.error + ", " + www.url);
			}
			AudioDataLoadState loadState = clip.loadState;
			while (clip.loadState == AudioDataLoadState.Loading || clip.loadState == AudioDataLoadState.Unloaded)
			{
				yield return null;
			}
			if (clip.loadState != AudioDataLoadState.Loaded)
			{
				clip = null;
				Debug.Log("Could not load audioclip at " + www.url + "! Load state :" + clip.loadState + ", Error: " + www.error);
			}
			else
			{
				Debug.Log("Loaded " + clip + ". Size: " + clip.samples);
				sliderSource.clip = clip;
				sliderSource.Play();
				WaveformMaker waveformMaker = sliderSource.gameObject.AddComponent<WaveformMaker>();
				waveformMaker.slider = scrubSlider;
			}
		}
	}
}
