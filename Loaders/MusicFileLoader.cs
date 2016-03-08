using System;
using System.IO;
using UnityEngine;

namespace Fireworks.Loaders
{
	public static class MusicFileLoader
	{
		public static WWW GetClipFromFile(string audioClipName)
		{
			Debug.LogError("Fetching clip at " + "file://" + AssetBundleLoader.Path + System.IO.Path.DirectorySeparatorChar + "Audio" + System.IO.Path.DirectorySeparatorChar + audioClipName);
			try
			{
				AudioClip clip = null;

				char dsc = System.IO.Path.DirectorySeparatorChar;

				WWW www = new WWW("file://" + AssetBundleLoader.Path + dsc + "Audio" + dsc + audioClipName);
				if (www.error != null)
				{
					throw new Exception("Loading had an error:" + www.error);
				}

				clip = www.GetAudioClip(true, false, AudioType.WAV);
				return www;
			}
			catch (Exception e)
			{
				LogException(e);
				return null;
			}
		}

		private static void LogException(Exception e)
		{
			StreamWriter sw = File.AppendText(AssetBundleLoader.Path + @"/mod.log");

			sw.WriteLine(e);

			sw.Flush();

			sw.Close();
		}
	}

}
