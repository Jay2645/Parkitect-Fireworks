using Parkitect.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fireworks.UI
{
	/// <summary>
	/// Creates all of the fireworks UI.
	/// </summary>
	public class FireworksUIBuilder : MonoBehaviour
	{
		public static Material ghostMat;
		public static Material ghostIntersectMat;
		public static Material ghostCantBuildMat;

		private FireworksWindow fireworksWindow;
		private UIWindowFrame fireworksWindowFrame;
		private FireworkLauncherItemEntry launcherItemEntry;

		private UIMenuButton clone;

		public static GameObject rectTfmPrefab
		{
			get
			{
				// Welcome to the hackiest place on earth
				if (_rectTfmPrefab == null)
				{
					// RectTransforms can't be added to objects directly; you have to instantiate a prefab
					// It's a pain to deal with the assetbundles needed to do that, so we're just going to steal one
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

					// Reset the Rect Transform:
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
			MakeBuilderMenuTab();
			MakeFireworksBuilderWindow();
			MakeShowWindow();
		}

		private void MakeBuilderMenuTab()
		{
			// Get all the buttons along the bottom
			UIMenuButton[] allMenuButtons = FindObjectsOfType<UIMenuButton>();
			DecoBuilderTab builderTab = null;
			UIWindow decorationWindow = null;
			foreach (UIMenuButton button in allMenuButtons)
			{
				//  Hopefully they never change the name
				if (button.name == "DecoBuilder")
				{
					decorationWindow = Instantiate(button.windowContentGO);
					builderTab = decorationWindow.gameObject.GetComponentInChildren<DecoBuilderTab>();
					GameObject fireworkUIMenuButton = Instantiate(button.gameObject);
					fireworkUIMenuButton.transform.SetParent(button.transform.parent, false);
					clone = fireworkUIMenuButton.GetComponent<UIMenuButton>();
					RectTransform cloneTfm = fireworkUIMenuButton.GetComponent<RectTransform>();
					Vector2 anchorMin = cloneTfm.anchorMin;
					Vector2 anchorMax = cloneTfm.anchorMax;
					anchorMin.x = 0.3515f;
					anchorMax.x = 0.3615f;
					cloneTfm.anchorMin = anchorMin;
					cloneTfm.anchorMax = anchorMax;
					fireworkUIMenuButton.name = "FireworkBuilder";
					UITooltip tooltip = fireworkUIMenuButton.GetComponent<UITooltip>();
					tooltip.text = "Firework Builder";
					break;
				}
			}
			if (builderTab != null)
			{
				Builder builder = Instantiate(builderTab.builderGO);
				ghostMat = builder.ghostMaterial;
				ghostIntersectMat = builder.ghostIntersectMaterial;
				ghostCantBuildMat = builder.ghostCantBuildMaterial;
				Destroy(builder.gameObject);
			}
			Destroy(decorationWindow.gameObject);
		}

		private void MakeFireworksBuilderWindow()
		{
			UIWindowSettings settings = MakeWindow("Firework Builder", "FireworkBuilder", false, new Vector2(500.0f, 300.0f));
			GameObject windowGO = settings.gameObject;

			// Actually add the window component, which makes it renderable
			fireworksWindow = windowGO.AddComponent<FireworksWindow>();

			// When our tab is clicked, it'll open up the new window
			clone.windowContentGO = fireworksWindow;
		}

		private void MakeShowWindow()
		{
			UIWindowSettings settings = MakeWindow("Show Creator", "ShowCreator", false, new Vector2(600.0f, 200.0f));
			GameObject windowGO = settings.gameObject;
			ShowWindow showWindow = windowGO.AddComponent<ShowWindow>();

			UIWindowsController.Instance.spawnWindow(showWindow, null);
		}

		private static UIWindowSettings MakeWindow(string windowName, string windowTag, bool pinnable, Vector2 size)
		{
			// Create the fireworks window GameObject
			GameObject windowGO = Instantiate<GameObject>(rectTfmPrefab);
			windowGO.name = windowName;

			// Size the fireworks window
			RectTransform fireworkRect = windowGO.GetComponent<RectTransform>();
			fireworkRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, size.y / 2.0f, size.y);
			fireworkRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, size.x / 2.0f, size.x);

			// Add various settings to the window
			UIWindowSettings settings = windowGO.AddComponent<UIWindowSettings>();
			settings.pinnable = pinnable;
			settings.uniqueTagString = windowTag;
			settings.title = windowName;

			return settings;
		}

		public void CleanUp()
		{
			Destroy(fireworksWindow.gameObject);
			Destroy(clone.gameObject);
		}
	}
}
