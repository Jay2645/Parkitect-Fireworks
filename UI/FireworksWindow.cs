using Parkitect.UI;
using UnityEngine;

namespace Fireworks.UI
{
	/// <summary>
	/// A window holding all fireworks launchers players can build.
	/// Also maybe shows? Or perhaps a dedicated show panel would be best.
	/// 
	/// Show panel:
	/// * Audio Clip source is background of slider
	/// * Below slider are keyframe events of firework launches
	/// * Below keyframes is a button to add a new firework launch
	/// * Pressing button allows player to select a shell and a launcher
	/// * Creates new Firework class to launch a shell when the slider hits that percent mark
	/// </summary>
	public class FireworksWindow : UIWindow
	{
		private FireworkLauncherBuilderTab buildertab;

		protected override void Start()
		{
			base.Start();
			GameObject buildertabGO = Instantiate<GameObject>(FireworksUIBuilder.rectTfmPrefab);
			buildertabGO.transform.SetParent(transform, false);
			buildertab = buildertabGO.AddComponent<FireworkLauncherBuilderTab>();
		}
	}
}
