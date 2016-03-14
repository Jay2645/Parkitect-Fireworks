using Fireworks.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Fireworks.Show
{
	public class ShowTrack
	{
		public ShowTrack(Transform trackParent, FireworkLauncher launcher)
		{
			GameObject trackInstance = Object.Instantiate(trackTemplate);
			trackInstance.gameObject.SetActive(true);
			trackInstance.transform.SetParent(trackParent, false);

			trackInstance.name = "Track " + trackNumber;
			trackNumber++;

			keyframeParent = trackInstance.transform.FindChild("Track");
			CreateKeyframes(keyframeParent);

			Transform trackEnabled = trackInstance.transform.FindChild("Enable Track");
			Toggle enableTrackToggle = trackEnabled.GetComponent<Toggle>();

			enableTrackToggle.onValueChanged.AddListener(new UnityAction<bool>(OnTrackEnableChange));
			enableTrackToggle.isOn = true;

			this.launcher = launcher;
		}

		private static int trackNumber = 0;
		public static GameObject trackTemplate = null;
		public bool isActive
		{
			get;
			private set;
		}

		private FireworkLauncher launcher;
		public Transform keyframeParent;

		private Dictionary<string, ShowKeyframe> trackKeyframes = new Dictionary<string, ShowKeyframe>();
		private Dictionary<string, Firework> trackFireworks = new Dictionary<string, Firework>();

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

		public void ToggleKeyframe(ShowKeyframe keyframe)
		{
			int dropdownValue = ShowWindow.fireworkDropdown.value;
			if (keyframe.isOn)
			{
				Dropdown.OptionData fireworkOption = ShowWindow.fireworkDropdown.options[dropdownValue];
				trackFireworks[keyframe.time] = new Firework(fireworkOption.text, dropdownValue, launcher);
			}
			else
			{
				trackFireworks.Remove(keyframe.time);
			}
		}

		public void ToggleKeyframe(string currentTime, int dropdownValue)
		{
			bool keyframeIsActive = !trackKeyframes[currentTime].isOn;
			trackKeyframes[currentTime].isOn = keyframeIsActive;
			if (keyframeIsActive)
			{
				Dropdown.OptionData fireworkOption = ShowWindow.fireworkDropdown.options[dropdownValue];
				trackFireworks[currentTime] = new Firework(fireworkOption.text, dropdownValue, launcher);
			}
			else
			{
				trackFireworks.Remove(currentTime);
			}
		}

		public void TryLaunchFireworks(string currentTime)
		{
			if (trackKeyframes[currentTime].isOn)
			{
				trackFireworks[currentTime].Launch();
			}
		}

		public int GetFireworkIndexAtTime(string currentTime)
		{
			if (trackKeyframes[currentTime].isOn)
			{
				return trackFireworks[currentTime].fireworkIndex;
			}
			else
			{
				return -1;
			}
		}
	}
}
