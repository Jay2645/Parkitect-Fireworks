using Fireworks.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fireworks.Show
{
	public class ShowTrack
	{
		private static int trackNumber = 0;
		public static GameObject trackTemplate = null;
		public bool isActive
		{
			get;
			private set;
		}

		public string showName;
		private Mortar launcher;
		public Transform keyframeParent;

		private GameObject selectedLauncherGO;

		private Dictionary<string, ShowKeyframe> trackKeyframes = new Dictionary<string, ShowKeyframe>();

		public static ShowTrack MakeTrack(Transform trackParent, Mortar launcher, string showName)
		{
			if (trackTemplate == null)
			{
				Debug.Log("Track Template has not been set! Total number of tracks: " + trackNumber);
				return null;
			}

			ShowTrack track = new ShowTrack();
			track.launcher = launcher;
			track.showName = showName;

			GameObject trackInstance = Object.Instantiate(trackTemplate);
			trackInstance.gameObject.SetActive(true);
			trackInstance.transform.SetParent(trackParent, false);

			trackInstance.name = "Track " + trackNumber;
			trackNumber++;

			track.keyframeParent = trackInstance.transform.FindChild("Track");
			track.CreateKeyframes(track.keyframeParent);

			Transform trackEnabled = trackInstance.transform.FindChild("Enable Track");
			Toggle enableTrackToggle = trackEnabled.GetComponent<Toggle>();

			enableTrackToggle.onValueChanged.AddListener(new UnityAction<bool>(track.OnTrackEnableChange));
			enableTrackToggle.isOn = true;

			return track;
		}

		private void CreateKeyframes(Transform trackPanel)
		{
			Toggle source = trackPanel.GetComponentInChildren<Toggle>();
			source.gameObject.SetActive(false);
			for (int i = 0; i <= 100; i++)
			{
				Toggle keyframe = Object.Instantiate(source);
				keyframe.gameObject.SetActive(true);
				ShowKeyframe showKeyframe = keyframe.gameObject.AddComponent<ShowKeyframe>();
				showKeyframe.keyframe = keyframe;
				// HorizontalLayoutGroup takes care of moving the keyframes
				keyframe.transform.SetParent(source.transform.parent);
				string keyframeName;
				if (i == 100)
				{
					keyframeName = "1.00";
				}
				else
				{
					string numberToPercent = "0.";
					if (i < 10)
					{
						numberToPercent += "0";
					}
					numberToPercent += i.ToString();
					keyframeName = numberToPercent;
				}
				keyframe.name = keyframeName;
				showKeyframe.time = keyframeName;
				showKeyframe.isOn = launcher.HasFirework(showName, keyframeName);
				showKeyframe.track = this;
				trackKeyframes[keyframeName] = showKeyframe;
			}
		}

		public void OnTrackEnableChange(bool newValue)
		{
			isActive = newValue;

			Color setColor = Color.white;
			if (!isActive)
			{
				setColor.a = 0.25f;
			}
			Image keyframeImage = keyframeParent.GetComponent<Image>();
			keyframeImage.color = setColor;

			Toggle[] childToggles = keyframeParent.GetComponentsInChildren<Toggle>();
			foreach (Toggle toggle in childToggles)
			{
				toggle.interactable = isActive;
			}

			ShowWindow.ChangeTrack(this);
		}

		public void OnBecomeActiveTrack()
		{
			selectedLauncherGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			selectedLauncherGO.transform.parent = launcher.transform;
			selectedLauncherGO.transform.localPosition = Vector3.up;
			Color ghostColor = Color.green;
			ghostColor.a = 0.25f;
			selectedLauncherGO.GetComponent<Renderer>().material.color = ghostColor;
		}

		public void OnBecomeInactiveTrack()
		{
			Object.Destroy(selectedLauncherGO);
		}

		public void ToggleKeyframe(ShowKeyframe keyframe)
		{
			int dropdownValue = ShowWindow.fireworkDropdown.value;
			if (keyframe.isOn)
			{
				Dropdown.OptionData fireworkOption = ShowWindow.fireworkDropdown.options[dropdownValue];
				launcher.AddFirework(showName, keyframe.time, fireworkOption.text);
			}
			else
			{
				launcher.RemoveFirework(showName, keyframe.time);
			}
		}

		public void ToggleKeyframe(string currentTime, int dropdownValue)
		{
			bool keyframeIsActive = !trackKeyframes[currentTime].isOn;
			trackKeyframes[currentTime].isOn = keyframeIsActive;
			if (keyframeIsActive)
			{
				Dropdown.OptionData fireworkOption = ShowWindow.fireworkDropdown.options[dropdownValue];
				launcher.AddFirework(showName, currentTime, fireworkOption.text);
			}
			else
			{
				launcher.RemoveFirework(showName, currentTime);
			}
		}

		public void TryLaunchFireworks(string currentTime)
		{
			if (trackKeyframes[currentTime].isOn)
			{
				launcher.PlayShow(showName, currentTime);
			}
		}

		public int GetFireworkIndexAtTime(string currentTime)
		{
			if (trackKeyframes[currentTime].isOn)
			{
				string fireworkName = launcher.GetFireworkNameAtTime(showName, currentTime);
				for (int i = 0; i < ShowWindow.fireworkDropdown.options.Count; i++)
				{
					Dropdown.OptionData fireworkOption = ShowWindow.fireworkDropdown.options[i];
					if (fireworkOption.text == fireworkName)
					{
						return i;
					}
				}
			}

			return -1;
		}
	}
}
