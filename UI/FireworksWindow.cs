using Parkitect.UI;
using UnityEngine;

namespace Fireworks.UI
{
	/// <summary>
	/// A window holding all fireworks launchers players can build.
	/// </summary>
	public class FireworksWindow : UIWindow
	{
		private MortarBuilderTab buildertab;

		protected override void Start()
		{
			base.Start();
			// Create a child Transform holding the builder tab
			GameObject buildertabGO = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			buildertabGO.transform.SetParent(transform, false);
			buildertab = buildertabGO.AddComponent<MortarBuilderTab>();
		}

		public void CleanUp()
		{
			buildertab.CleanUp();
		}
	}
}
