using Fireworks.Loaders;
using Fireworks.Music;
using Fireworks.Show;
using Parkitect.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fireworks.UI
{
	public class ShowWindow : UIWindow
	{
		private const string AUDIO_SLIDER_NAME = "Show Panel";

		private static AudioSource sliderSource;
		private AudioClip clip;
		private static Slider scrubSlider;
		private static Text scrubberText;
		public static Button playButton;
		public static Button pauseButton;

		private static Button addFireworkButton;
		public static Dropdown fireworkDropdown;

		private string currentTime = "0.00";

		private static List<ShowTrack> allTracks = new List<ShowTrack>();
		private static ShowTrack currentTrack = null;
		private static Transform trackParent;

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
			PopulateDropdown();
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

			// Tracks
			Transform trackPanel = sliderGO.transform.FindChild("Scroll View/Viewport/Content/Track Template");
			trackParent = trackPanel.parent;
			trackPanel.gameObject.SetActive(false);
			ShowTrack.trackTemplate = trackPanel.gameObject;

			// Load song
			StartCoroutine(LoadClip());
		}

		public static void AddTrack(FireworkLauncher launcher)
		{
			ShowTrack track = new ShowTrack(trackParent, launcher);
			allTracks.Add(track);
		}

		public static void ChangeTrack(ShowTrack newTrack)
		{
			if (newTrack == null || newTrack.isActive)
			{
				// If it's the active track, update our active track
				// If it's null, make our new track null
				currentTrack = newTrack;
			}
			else if (newTrack == currentTrack)
			{
				// It's not active, but it's listed as our active track
				currentTrack = null;
			}

			if (currentTrack == null)
			{
				addFireworkButton.interactable = false;
				fireworkDropdown.interactable = false;
			}
			else
			{
				addFireworkButton.interactable = true;
				fireworkDropdown.interactable = true;
			}
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
			UpdateDropdown();
			if (sliderSource.isPlaying)
			{
				foreach (ShowTrack track in allTracks)
				{
					track.TryLaunchFireworks(currentTime);
				}
			}
		}

		private void AddFirework()
		{
			if (currentTrack == null)
			{
				return;
			}
			currentTrack.ToggleKeyframe(currentTime, fireworkDropdown.value);
		}

		private void PopulateDropdown()
		{
			fireworkDropdown.ClearOptions();
			string[] defaultFireworkNames = new string[]
			{
				 "Firework-1"
			};
			List<string> allFireworks = new List<string>(defaultFireworkNames);
			// Load custom fireworks here later
			fireworkDropdown.AddOptions(allFireworks);
		}

		private void UpdateDropdown()
		{
			if (currentTrack == null)
			{
				return;
			}
			int fireworkIndex = currentTrack.GetFireworkIndexAtTime(currentTime);
			if (fireworkIndex >= 0)
			{
				fireworkDropdown.value = fireworkIndex;
			}
			else if (sliderSource.isPlaying)
			{
				fireworkDropdown.value = 0;
			}
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
