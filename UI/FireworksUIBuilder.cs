using Parkitect.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fireworks.UI
{
	public class FireworksUIBuilder : MonoBehaviour
	{
		private FireworksWindow fireworksWindow;
		private UIWindowFrame fireworksWindowFrame;
		private FireworkLauncherItemEntry launcherItemEntry;

		public static GameObject rectTfmPrefab
		{
			get
			{
				// Welcome to the hackiest place on earth
				if (_rectTfmPrefab == null)
				{
					RectTransform foundRectTfm = FindObjectOfType<RectTransform>();
					GameObject instantiatedGO = Instantiate<GameObject>(foundRectTfm.gameObject);
					foreach (Transform child in instantiatedGO.transform)
					{
						Destroy(child);
					}

					Graphic imageComponent = instantiatedGO.GetComponent<Graphic>();
					if (imageComponent != null)
					{
						DestroyImmediate(imageComponent);
					}

					List<Component> childComponents = new List<Component>(instantiatedGO.GetComponents<Component>());
					// Iterate an arbitrary number of times to try and destroy all child components
					for (int i = 0; i < 4; i++)
					{
						foreach (Component component in childComponents)
						{
							if (!(component is RectTransform))
							{
								DestroyImmediate(component);
							}
						}
						childComponents = new List<Component>(instantiatedGO.GetComponents<Component>());
						if (i % 2 == 0)
						{
							// Reverse the order to vary up the order in which we try to destroy the components
							childComponents.Reverse();
						}
					}
					_rectTfmPrefab = instantiatedGO;
					RectTransform rectTfm = _rectTfmPrefab.GetComponent<RectTransform>();
					rectTfm.anchorMin = Vector2.zero;
					rectTfm.anchorMax = Vector2.one;
					rectTfm.anchoredPosition = Vector2.zero;
					rectTfm.sizeDelta = Vector2.zero;
					rectTfm.pivot = new Vector2(0.5f, 0.5f);
				}
				return _rectTfmPrefab;
			}
		}
		private static GameObject _rectTfmPrefab;

		private void Start()
		{
			GameObject windowGO = Instantiate<GameObject>(rectTfmPrefab);
			windowGO.name = "Firework Builder";

			RectTransform fireworkRect = windowGO.GetComponent<RectTransform>();
			fireworkRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 150.0f, 300.0f);
			fireworkRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 250.0f, 500.0f);

			fireworksWindow = windowGO.AddComponent<FireworksWindow>();
			fireworksWindowFrame = UIWindowsController.Instance.spawnWindow(fireworksWindow, null);
			fireworksWindowFrame.setTitle("Firework Builder");
		}

		public void CleanUp()
		{
			Destroy(fireworksWindow.gameObject);
		}
	}
}
