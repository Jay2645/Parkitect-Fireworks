using Fireworks.Loaders;
using Fireworks.UI;
using MiniJSON;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Fireworks
{
	public class Main : IMod, IModSettings
	{
		private Dictionary<string, object> settingsValueDictionary = new Dictionary<string, object>();
		private FireworksUIBuilder uiBuilder;

		#region Start Mod
		public void onEnabled()
		{
			AssetBundleLoader.Path = Path;
			GameObject go = new GameObject("Fireworks UI Builder");
			LauncherCreator.CreateLaunchers();
			uiBuilder = go.AddComponent<FireworksUIBuilder>();
			ReadSettingsFile();
		}

		public void onDisabled()
		{

		}
		#endregion

		#region Settings Panel
		public void onDrawSettingsUI()
		{

		}

		private bool DrawBoolField(string fieldName, bool initialValue)
		{
			bool value = GUILayout.Toggle(initialValue, fieldName);
			settingsValueDictionary[fieldName] = value;
			return value;
		}

		private float DrawFloatField(string fieldName, float initialValue)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(fieldName);
			string floatString = GUILayout.TextField(initialValue.ToString("F1"));
			float floatValue;
			if (!float.TryParse(floatString, out floatValue))
			{
				floatValue = initialValue;
			}
			else if (floatValue == 0.0f)
			{
				floatValue = initialValue;
			}
			GUILayout.EndHorizontal();
			settingsValueDictionary[fieldName] = floatString;
			return floatValue;
		}

		public void onSettingsOpened()
		{

		}

		public void onSettingsClosed()
		{
			WriteSettingsFile();
		}
		#endregion

		#region Save/Load Settings
		public void WriteSettingsFile()
		{
			string jsonDictionary = Json.Serialize(settingsValueDictionary);
			File.WriteAllText(Path + @"/settings.json", jsonDictionary);
		}

		public void ReadSettingsFile()
		{

		}
		#endregion

		#region Mod Details
		/// <summary>
		///     Gets the name of this instance.
		/// </summary>
		public string Name
		{
			get
			{
				return "Fireworks";
			}
		}

		/// <summary>
		///     Gets the description of this instance.
		/// </summary>
		public string Description
		{
			get
			{
				return "Allows you to create your own fireworks shows.";
			}
		}

		private string _identifier;
		/// <summary>
		///     Gets an unique identifier of this mod.
		/// <summary>
		public string Identifier
		{
			get
			{
				return _identifier;
			}
			set
			{
				_identifier = value;
			}
		}

		private string _path;
		public string Path
		{
			get
			{
				return _path;
			}
			set
			{
				_path = value;
			}
		}
		#endregion
	}
}
