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

		private static AudioSource sliderSource;
		private AudioClip clip;
		private static Slider scrubSlider;
		private static Text scrubberText;
		public static Button playButton;
		public static Button pauseButton;

		private static Button addFireworkButton;
		private static Dropdown fireworkDropdown;

		private Dictionary<string, List<Firework>> fireworkLaunchTimes = new Dictionary<string, List<Firework>>();
		private static List<Firework> currentFireworkList;
		private string currentTime = "0.00";

		protected override void Start()
		{
			base.Start();
			GameObject sliderGO = AssetBundleLoader.LoadAsset(AssetBundleLoader.UI_ELEMENT_ASSETBUNDLE, AUDIO_SLIDER_NAME);
			sliderGO.transform.SetParent(transform, false);

			// Load Scrubber
			scrubSlider = sliderGO.GetComponentInChildren<Slider>();
			scrubSlider.value = 0.0f;
			sliderSource = sliderGO.GetComponentInChildren<AudioSource>();
			Transform fileTextTfm = scrubSlider.transform.parent.FindChild("File Text");
			scrubberText = fileTextTfm.GetComponent<Text>();
			scrubSlider.onValueChanged.AddListener(new UnityAction<float>(OnScrub));
			scrubSlider.gameObject.SetActive(false);

			// Firework Details
			Transform detailPanel = sliderGO.transform.FindChild("Firework Detail Panel");
			addFireworkButton = detailPanel.transform.FindChild("Add Firework Button").GetComponent<Button>();
			addFireworkButton.onClick.AddListener(new UnityAction(AddFirework));
			fireworkDropdown = detailPanel.GetComponentInChildren<Dropdown>();
			addFireworkButton.interactable = false;
			fireworkDropdown.interactable = false;

			// Playback
			Transform playbackControls = sliderGO.transform.FindChild("Playback Controls");
			playButton = playbackControls.transform.FindChild("Play").GetComponent<Button>();
			playButton.onClick.AddListener(new UnityAction(PlaySong));
			playButton.interactable = false;

			pauseButton = playbackControls.transform.FindChild("Pause").GetComponent<Button>();
			pauseButton.onClick.AddListener(new UnityAction(PauseSong));
			pauseButton.interactable = false;

			// Load song
			StartCoroutine(LoadClip());
		}

		private IEnumerator LoadClip()
		{
			scrubSlider.gameObject.SetActive(false);
			playButton.interactable = false;
			pauseButton.interactable = false;
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
				playButton.interactable = true;
				Debug.Log("Loaded " + clip + ". Size: " + clip.samples);
				sliderSource.clip = clip;
				scrubberText.text = www.url;
				WaveformMaker waveformMaker = sliderSource.gameObject.AddComponent<WaveformMaker>();
				waveformMaker.slider = scrubSlider;
			}
		}

		private void OnScrub(float newValue)
		{
			currentTime = newValue.ToString("0.00");
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
			if (FireworkLauncher.builtLaunchers.Count == 0)
			{
				return;
			}
			Firework firework = new Firework();
			firework.launcher = FireworkLauncher.builtLaunchers[fireworkDropdown.value];
			currentFireworkList.Add(firework);
			fireworkLaunchTimes[currentTime] = currentFireworkList;
			UpdateDropdown();
		}

		public static void UpdateDropdown()
		{
			fireworkDropdown.value = 0;
			fireworkDropdown.ClearOptions();
			if (FireworkLauncher.builtLaunchers.Count == 0)
			{
				fireworkDropdown.interactable = false;
				addFireworkButton.interactable = false;
				return;
			}

			List<string> fireworkStrings = new List<string>();
			List<FireworkLauncher> usedLaunchers = new List<FireworkLauncher>();
			foreach (Firework firework in currentFireworkList)
			{
				if (firework.launcher == null)
				{
					continue;
				}
				fireworkStrings.Add(firework.ToString());
				usedLaunchers.Add(firework.launcher);
			}

			foreach (FireworkLauncher launcher in FireworkLauncher.builtLaunchers)
			{
				if (usedLaunchers.Contains(launcher))
				{
					continue;
				}
				fireworkStrings.Add(launcher.name);
			}
			fireworkStrings.Reverse();
			fireworkDropdown.AddOptions(fireworkStrings);
			fireworkDropdown.interactable = true;
			addFireworkButton.interactable = true;
		}

		private void PlaySong()
		{
			sliderSource.Play();
			playButton.interactable = false;
			pauseButton.interactable = true;
		}

		private void PauseSong()
		{
			sliderSource.Stop();
			pauseButton.interactable = false;
			playButton.interactable = true;
		}
	}
}
