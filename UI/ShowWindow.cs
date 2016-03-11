using Fireworks.Loaders;
using Fireworks.Music;
using Parkitect.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fireworks.UI
{
	class ShowWindow : UIWindow
	{
		private const string AUDIO_SLIDER_NAME = "Show Panel";

		private AudioSource sliderSource;
		private AudioClip clip;
		private Slider scrubSlider;
		private Text scrubberText;

		private Button addFireworkButton;
		private Dropdown fireworkDropdown;

		private Dictionary<float, List<Firework>> fireworkLaunchTimes = new Dictionary<float, List<Firework>>();
		private List<Firework> currentFireworkList;
		private float currentTime = 0.0f;

		protected override void Start()
		{
			base.Start();
			GameObject sliderGO = AssetBundleLoader.LoadAsset(AssetBundleLoader.UI_ELEMENT_ASSETBUNDLE, AUDIO_SLIDER_NAME);
			sliderGO.transform.SetParent(transform, false);

			// Load Scrubber
			scrubSlider = sliderGO.GetComponentInChildren<Slider>();
			sliderSource = sliderGO.GetComponentInChildren<AudioSource>();
			Transform fileTextTfm = scrubSlider.transform.parent.FindChild("File Text");
			scrubberText = fileTextTfm.GetComponent<Text>();
			scrubSlider.onValueChanged.AddListener(new UnityAction<float>(OnScrub));
			scrubSlider.gameObject.SetActive(false);
			StartCoroutine(LoadClip());

			// Firework Details
			Transform detailPanel = sliderGO.transform.FindChild("Firework Detail Panel");
			addFireworkButton = detailPanel.transform.FindChild("Add Firework Button").GetComponent<Button>();
			addFireworkButton.onClick.AddListener(new UnityAction(AddFirework));
			fireworkDropdown = detailPanel.GetComponentInChildren<Dropdown>();

			// Playback
			Transform playbackControls = sliderGO.transform.FindChild("Playback Controls");
			Button play = playbackControls.transform.FindChild("Play").GetComponent<Button>();
			play.onClick.AddListener(new UnityAction(PlaySong));
			Button pause = playbackControls.transform.FindChild("Pause").GetComponent<Button>();
			pause.onClick.AddListener(new UnityAction(PauseSong));
		}

		private IEnumerator LoadClip()
		{
			scrubSlider.gameObject.SetActive(false);
			WWW www = MusicFileLoader.GetClipFromFile("tiki.wav");
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
				scrubSlider.gameObject.SetActive(true);
				Debug.Log("Loaded " + clip + ". Size: " + clip.samples);
				sliderSource.clip = clip;
				scrubberText.text = www.url;
				WaveformMaker waveformMaker = sliderSource.gameObject.AddComponent<WaveformMaker>();
				waveformMaker.slider = scrubSlider;
			}
		}

		private void OnScrub(float newValue)
		{
			currentTime = newValue;
			if (!fireworkLaunchTimes.ContainsKey(currentTime))
			{
				fireworkLaunchTimes[currentTime] = new List<Firework>();
			}
			currentFireworkList = fireworkLaunchTimes[currentTime];
			UpdateDropdown();
			if (sliderSource.isPlaying)
			{
				foreach (Firework firework in currentFireworkList)
				{
					firework.Launch();
				}
			}
		}

		private void AddFirework()
		{
			Firework firework = new Firework();
			currentFireworkList.Add(firework);
			fireworkLaunchTimes[currentTime] = currentFireworkList;
			UpdateDropdown();
		}

		private void UpdateDropdown()
		{
			fireworkDropdown.value = 0;
			fireworkDropdown.ClearOptions();
			List<string> fireworkStrings = new List<string>();
			foreach (Firework firework in currentFireworkList)
			{
				fireworkStrings.Add(firework.ToString());
			}
			fireworkDropdown.AddOptions(fireworkStrings);
		}

		private void PlaySong()
		{
			sliderSource.Play();
		}

		private void PauseSong()
		{
			sliderSource.Stop();
		}
	}
}
