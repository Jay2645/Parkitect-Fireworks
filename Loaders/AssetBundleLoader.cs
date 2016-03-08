using System;
using System.IO;
using UnityEngine;

namespace Fireworks.Loaders
{
	public static class AssetBundleLoader
	{
		public const string UI_ELEMENT_ASSETBUNDLE = "ui-elements";

		public static string Path;

		public static GameObject LoadAsset(string assetBundleName, string prefabName)
		{
			try
			{
				GameObject asset = new GameObject();

				char dsc = System.IO.Path.DirectorySeparatorChar;

				using (WWW www = new WWW("file://" + Path + dsc + "AssetBundles" + dsc + assetBundleName))
				{
					if (www.error != null)
						throw new Exception("Loading had an error:" + www.error);

					AssetBundle bundle = www.assetBundle;

					try
					{
						asset = UnityEngine.Object.Instantiate(bundle.LoadAsset(prefabName)) as GameObject;

						return asset;
					}
					catch (Exception e)
					{
						LogException(e);
						//return null;
					}
					finally
					{
						bundle.Unload(false);
					}
					return null;
				}
			}
			catch (Exception e)
			{
				LogException(e);
				return null;
			}
		}

		private static void LogException(Exception e)
		{
			StreamWriter sw = File.AppendText(Path + @"/mod.log");

			sw.WriteLine(e);

			sw.Flush();

			sw.Close();
		}
	}
}
