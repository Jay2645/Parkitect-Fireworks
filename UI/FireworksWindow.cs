using Parkitect.UI;
using UnityEngine;

namespace Fireworks.UI
{
	public class FireworksWindow : UIWindow
	{
		private FireworkBuilderTab buildertab;

		protected override void Start()
		{
			base.Start();
			GameObject buildertabGO = Instantiate<GameObject>(FireworksUIBuilder.rectTfmPrefab);
			buildertabGO.transform.SetParent(transform, false);
			buildertab = buildertabGO.AddComponent<FireworkBuilderTab>();
		}
	}
}
