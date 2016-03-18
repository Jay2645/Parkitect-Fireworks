using Fireworks.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Fireworks
{
	public class MortarShow
	{
		public string showName;
		public Dictionary<string, string> fireworkTimes = new Dictionary<string, string>();

		public void AddFirework(string time, string fireworkName)
		{
			fireworkTimes[time] = fireworkName;
		}

		public void RemoveFirework(string time)
		{
			fireworkTimes.Remove(time);
		}

		public string GetFireworkName(string time)
		{
			if (fireworkTimes == null)
			{
				return string.Empty;
			}

			string output;
			if (fireworkTimes.TryGetValue(time, out output))
			{
				return output;
			}
			else
			{
				return string.Empty;
			}
		}

		public Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> serializedMortar = new Dictionary<string, object>();
			serializedMortar["show"] = showName;
			serializedMortar["times"] = fireworkTimes;
			return serializedMortar;
		}

		public static MortarShow Deserialize(Dictionary<string, object> serializedShow)
		{
			MortarShow show = new MortarShow();
			show.showName = (string)serializedShow["show"];
			Dictionary<string, object> serializedTimes = (Dictionary<string, object>)serializedShow["times"];
			show.fireworkTimes = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> kvp in serializedTimes)
			{
				show.fireworkTimes[kvp.Key] = (string)kvp.Value;
			}
			return show;
		}
	}

	/// <summary>
	/// A mortar is a piece of scenery which launches a firework during the show.
	/// It is the only thing which is serialized, so it also contains all information about all shows involving it.
	/// Each mortar has:
	/// * A list of the shows it's involved in
	/// * A time it launches fireworks during that show
	/// * The name of the firework it launches at that time
	/// </summary>
	public class Mortar : Deco
	{
		/// <summary>
		/// All the mortars that we have ever built.
		/// </summary>
		public static List<Mortar> builtLaunchers = new List<Mortar>();
		/// <summary>
		/// All our shows.
		/// The key is the name of the show.
		/// The value is another Dictionary:
		/// * The key of the second Dictionary contains the time a firework is launched.
		/// * The value of the second Dictionary contains the name of the firework that is launched.
		/// </summary>
		//[Serialized]
		public List<MortarShow> shows = new List<MortarShow>();
		//public Dictionary<string, Dictionary<string, string>> shows = new Dictionary<string, Dictionary<string, string>>();

		protected override void Awake()
		{
			buildOnGrid = true;
			base.Awake();
		}

		public override void Start()
		{
			if (!dontSerialize && !isBlueprint && !isPreview)
			{
				AddLauncher(this);
			}
			base.Start();
		}

		/// <summary>
		/// Adds a specified firework to a specified show at a specified time.
		/// </summary>
		/// <param name="showName">The name of the show the firework should be added to.</param>
		/// <param name="showTime">The time in the show that the firework should be added.</param>
		/// <param name="fireworkName">The name of the firework that we're launching.</param>
		public void AddFirework(string showName, string showTime, string fireworkName)
		{
			MortarShow show = GetShow(showName);
			if (show == null)
			{
				show = new MortarShow();
				show.showName = showName;
				shows.Add(show);
			}

			show.AddFirework(showTime, fireworkName);
		}

		public void RemoveFirework(string showName, string showTime)
		{
			MortarShow show = GetShow(showName);
			if (show == null)
			{
				show = new MortarShow();
				show.showName = showName;
				shows.Add(show);
			}

			show.RemoveFirework(showTime);
		}

		/// <summary>
		/// Returns the name of a firework found at a specific time.
		/// </summary>
		/// <param name="showName">The show to look at.</param>
		/// <param name="showTime">The time in the show to try and find a firework.</param>
		/// <returns>The name of the firework if there is one, an empty string if there isn't a firework there.</returns>
		public string GetFireworkNameAtTime(string showName, string showTime)
		{
			MortarShow currentShow = GetShow(showName);
			if (currentShow == null)
			{
				return string.Empty;
			}
			return currentShow.GetFireworkName(showTime);
		}

		public bool HasFirework(string showName, string showTime)
		{
			return GetFireworkNameAtTime(showName, showTime) != string.Empty;
		}


		private MortarShow GetShow(string showName)
		{
			MortarShow show = null;
			foreach (MortarShow mortarShow in shows)
			{
				if (mortarShow.showName == showName)
				{
					show = mortarShow;
					break;
				}
			}
			return show;
		}

		/// <summary>
		/// Launch any fireworks in the specified show at the specified time.
		/// </summary>
		/// <param name="showName">The show to launch the fireworks from.</param>
		/// <param name="showTime">The time in the show to launch the fireworks.</param>
		public void PlayShow(string showName, string showTime)
		{
			string fireworkName = GetFireworkNameAtTime(showName, showTime);
			if (!string.IsNullOrEmpty(fireworkName))
			{
				LaunchFirework(fireworkName);
			}
		}

		private void LaunchFirework(string fireworkToLaunch)
		{
			ParticleSystem firework = Firework.GetFirework(fireworkToLaunch);
			if (firework == null)
			{
				throw new System.NullReferenceException("Firework " + fireworkToLaunch + " was invalid!");
			}
			else
			{
				firework.transform.position = transform.position;
				firework.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
			}
		}

		public override void onDestructionClick()
		{
			RemoveLauncher(this);
			base.onDestructionClick();
		}

		public override void serialize(SerializationContext context, Dictionary<string, object> values)
		{
			base.serialize(context, values);
			Dictionary<string, object>[] dictionaryArray = new Dictionary<string, object>[shows.Count];
			for (int index = 0; index < shows.Count; index++)
			{
				dictionaryArray[index] = shows[index].Serialize();
			}
			values.Add("shows", dictionaryArray);
		}

		public override void deserialize(SerializationContext context, Dictionary<string, object> values)
		{
			base.deserialize(context, values);
			List<object> list = (List<object>)values["shows"];
			shows.Clear();
			for (int index = 0; index < list.Count; index++)
			{
				shows.Add(MortarShow.Deserialize((Dictionary<string, object>)list[index]));
			}
		}

		private static void AddLauncher(Mortar launcher)
		{
			builtLaunchers.Add(launcher);
			ShowWindow.AddTrack(launcher);
		}

		private static void RemoveLauncher(Mortar launcher)
		{
			builtLaunchers.Remove(launcher);
		}

		public void CleanUp()
		{
			ScriptableSingleton<AssetManager>.Instance.unregisterObject(this);
			if (TryKill())
			{
				Debug.Log("Cleaning up " + this);
			}
		}
	}
}
