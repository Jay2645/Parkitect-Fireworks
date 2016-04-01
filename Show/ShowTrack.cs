using Fireworks.UI;
using Fireworks.UI.Tracks;
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
		public FireworkUITrack track;

		private GameObject selectedLauncherGO;

		//private Dictionary<string, ShowKeyframe> trackKeyframes = new Dictionary<string, ShowKeyframe>();

		public static ShowTrack MakeTrack(Transform trackParent, Mortar launcher, string showName)
		{
			if (trackTemplate == null)
			{
				Debug.Log("Track Template has not been set! Total number of tracks: " + trackNumber);
				return null;
			}

			ShowTrack showTrack = new ShowTrack();
			showTrack.launcher = launcher;
			showTrack.showName = showName;

			GameObject trackInstance = Object.Instantiate(trackTemplate);
			trackInstance.gameObject.SetActive(true);
			trackInstance.transform.SetParent(trackParent, false);

			trackInstance.name = "Track " + trackNumber;
			trackNumber++;

			Transform keyframeParent = trackInstance.transform.FindChild("Track");
			showTrack.track = keyframeParent.gameObject.AddComponent<FireworkUITrack>();
			showTrack.track.show = showName;
			showTrack.track.mortarParent = launcher;
			//showTrack.track.UpdateFireworkDictionary(launcher.GetAllFireworksForShow(showName));

			Transform trackEnabled = trackInstance.transform.FindChild("Enable Track");
			Toggle enableTrackToggle = trackEnabled.GetComponent<Toggle>();

			enableTrackToggle.onValueChanged.AddListener(new UnityAction<bool>(showTrack.OnTrackEnableChange));
			enableTrackToggle.isOn = true;

			return showTrack;
		}

		public void OnTrackEnableChange(bool newValue)
		{
			isActive = newValue;

			Color setColor = Color.white;
			if (!isActive)
			{
				setColor.a = 0.25f;
			}

			track.Interactable = newValue;
			Image keyframeImage = track.gameObject.GetComponent<Image>();
			keyframeImage.color = setColor;

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

		/*public void ToggleKeyframe(ShowKeyframe keyframe)
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
		}*/

		public void ToggleKeyframe(string currentTime, int dropdownValue)
		{
			if (track.HasFirework(currentTime))
			{
				track.RemoveFirework(currentTime);
			}
			else
			{
				Dropdown.OptionData fireworkOption = ShowWindow.fireworkDropdown.options[dropdownValue];
				track.AddFirework(currentTime, fireworkOption.text);
			}
		}

		public void TryLaunchFireworks(string currentTime)
		{
			if (track.HasFirework(currentTime))
			{
				launcher.PlayShow(showName, currentTime);
			}
		}

		public int GetFireworkIndexAtTime(string currentTime)
		{
			if (track.HasFirework(currentTime))
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
