using System.Collections.Generic;
using UnityEngine;

namespace Fireworks.UI.Tracks
{
	/// <summary>
	/// This is an individual firework "track."
	/// Every mortar will have a track that corresponds to it.
	/// In this track, there is a list of times, ranging from 0.00 (the start) to 1.00 (the end).
	/// This is a percentage value representing how much time has elapsed in the song; i.e. 0.50 will be the halfway point in the song.
	/// There are FireworkUISlots for every percentage value from 0.00 to 1.00.
	/// All the FireworkUIData objects contained in each FireworkUISlot are mapped to the parent Mortar.
	/// As things are moved around on this FireworkUITrack, it automatically updates the present show on our parent Mortar.
	/// </summary>
	public class FireworkUITrack : MonoBehaviour
	{
		public const float UI_NONINTERACTABLE_ALPHA = 0.25f;

		private Dictionary<string, FireworkUIData> fireworkList = new Dictionary<string, FireworkUIData>();
		private Dictionary<string, FireworkUISlot> fireworkSlots = new Dictionary<string, FireworkUISlot>();

		private GameObject trackPanel;
		private GameObject fireworkUIPrefab;

		public Mortar mortarParent;
		public string show;

		public bool Interactable
		{
			get
			{
				return _interactable;
			}
			set
			{
				Color setColor = Color.white;
				if (!value)
				{
					setColor.a = UI_NONINTERACTABLE_ALPHA;
				}
				_interactable = value;
				UnityEngine.UI.Image keyframeImage = gameObject.GetComponent<UnityEngine.UI.Image>();
				keyframeImage.color = setColor;

				FireworkUIData[] data = gameObject.GetComponentsInChildren<FireworkUIData>();
				foreach (FireworkUIData uiData in data)
				{
					uiData.ChangeImageState();
				}
			}
		}
		private bool _interactable = true;

		private void Awake()
		{
			trackPanel = gameObject;

			foreach (Transform child in transform)
			{
				fireworkUIPrefab = child.gameObject;
				fireworkUIPrefab.AddComponent<FireworkUISlot>();
				foreach (Transform grandchild in fireworkUIPrefab.transform)
				{
					grandchild.gameObject.AddComponent<FireworkUIData>();
					break;
				}
				break;
			}

			for (float i = 0.00f; i <= 1.00f; i += 0.01f)
			{
				string currentIndex = i.ToString("0.00");
				GameObject slotGO = Instantiate(fireworkUIPrefab);
				slotGO.SetActive(true);
				slotGO.name = currentIndex;

				FireworkUISlot slotInstance = slotGO.GetComponent<FireworkUISlot>();
				slotInstance.transform.SetParent(trackPanel.transform, false);
				slotInstance.Time = currentIndex;
				slotInstance.parentTrack = this;
				fireworkSlots[currentIndex] = slotInstance;

				FireworkUIData dataInstance = slotGO.GetComponentInChildren<FireworkUIData>();
				dataInstance.parentTrack = this;
				dataInstance.Time = currentIndex;
				dataInstance.Firework = string.Empty;
				fireworkList[currentIndex] = dataInstance;
			}
		}

		/// <summary>
		/// Force-updates our current firework list with a list of new firework times.
		/// </summary>
		/// <param name="newFireworkTimes">A Dictionary mapping times to firework names.</param>
		public void UpdateFireworkDictionary(Dictionary<string, string> newFireworkTimes)
		{
			foreach (KeyValuePair<string, string> kvp in newFireworkTimes)
			{
				fireworkList[kvp.Key].Firework = kvp.Value;
			}
		}

		/// <summary>
		/// Forcibly updates the mortar to match the UI.
		/// </summary>
		public void ForceUpdateMortar()
		{
			// Update mortar
			Dictionary<string, string> fireworkTimes = new Dictionary<string, string>();
			foreach (KeyValuePair<string, FireworkUIData> kvp in fireworkList)
			{
				fireworkTimes[kvp.Key] = kvp.Value.Firework;
			}
			mortarParent.UpdateShowTimes(show, fireworkTimes);
		}

		public void RemoveFirework(string time)
		{
			fireworkList[time].Firework = string.Empty;
			mortarParent.RemoveFirework(show, time);
		}

		public void AddFirework(string time, string name)
		{
			fireworkList[time].Firework = name;
			mortarParent.AddFirework(show, time, name);
		}

		/// <summary>
		/// Returns true if we have a firework to launch at the given time.
		/// </summary>
		/// <param name="time">The time to check.</param>
		/// <returns>True if there is a firework to launch at the time, else false.</returns>
		public bool HasFirework(string time)
		{
			return fireworkList[time].HasFirework;
		}

		public FireworkUIData GetDataAtTime(string time)
		{
			return fireworkList[time];
		}

		public FireworkUISlot GetSlotAtTime(string time)
		{
			return fireworkSlots[time];
		}

		/// <summary>
		/// Sets the firework at the given show time. Will also update our parent Mortar with the new show times.
		/// </summary>
		/// <param name="time">The time this firework will go off.</param>
		/// <param name="data">The FireworkUIData representing the firework to launch.</param>
		public void SetDataAtTime(string time, FireworkUIData data)
		{
			fireworkList[time] = data;

			// Probably a better way to do this
			ForceUpdateMortar();
		}
	}
}