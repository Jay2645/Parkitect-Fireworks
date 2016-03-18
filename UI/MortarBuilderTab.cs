using Parkitect.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Fireworks.UI
{
	/// <summary>
	/// The tab where players can select the firework launcher they want to build.
	/// </summary>
	public class MortarBuilderTab : BuilderTab
	{
		protected override void Awake()
		{
			MakeUIItemEntry();

			MakeItemDisplayPanel();

			MakeBuilder();

			base.Awake();
		}

		/// <summary>
		/// This creates the template for all of the little UI thumbnails used by the window
		/// </summary>
		private void MakeUIItemEntry()
		{
			// Make Builder Entry
			GameObject uiBuilderEntry = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			// Requires a button and a RawImage
			uiBuilderEntry.AddComponent<CanvasRenderer>();
			uiBuilderEntry.AddComponent<RawImage>();
			Button button = uiBuilderEntry.AddComponent<Button>();
			MortarItemEntry builderEntry = uiBuilderEntry.AddComponent<MortarItemEntry>();
			builderEntry.name = "UI Entry";

			// Also requires texts for the name and the price of the entry
			GameObject nameTextGO = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			nameTextGO.name = "Name Text";
			GameObject priceTextGO = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			priceTextGO.name = "Price Text";
			nameTextGO.transform.SetParent(uiBuilderEntry.transform, false);
			priceTextGO.transform.SetParent(uiBuilderEntry.transform, false);
			priceTextGO.AddComponent<CanvasRenderer>();
			nameTextGO.AddComponent<CanvasRenderer>();
			builderEntry.priceText = priceTextGO.AddComponent<Text>();
			builderEntry.nameText = nameTextGO.AddComponent<Text>();

			/*GameObject backgroundImageGO = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			backgroundImageGO.AddComponent<CanvasRenderer>();
			Image image = backgroundImageGO.AddComponent<Image>();
			Color backgroundColor = Color.grey;
			backgroundColor.a = 0.15f;
			image.color = backgroundColor;
			backgroundImageGO.transform.SetParent(uiBuilderEntry.transform, false);
			backgroundImageGO.transform.SetAsLastSibling();*/

			// Add it to our ItemEntryGO template
			builderItemEntryGO = builderEntry;
		}

		/// <summary>
		/// This creates the area where all the thumbnails will be displayed
		/// </summary>
		private void MakeItemDisplayPanel()
		{
			GameObject itemPanelGO = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			itemPanelGO.transform.SetParent(transform, false);

			GridLayoutGroup windowLayout = itemPanelGO.AddComponent<GridLayoutGroup>();
			windowLayout.cellSize = new Vector2(100.0f, 100.0f);
			itemContentPanel = itemPanelGO.GetComponent<RectTransform>();
			itemContentPanel.anchorMin = Vector2.zero;
			itemContentPanel.anchorMax = Vector2.one;
			itemContentPanel.anchoredPosition = Vector2.zero;
			itemContentPanel.sizeDelta = Vector2.zero;
			itemContentPanel.pivot = new Vector2(0.5f, 0.5f);
		}

		/// <summary>
		/// This creates the actual builder that will be used to place the thumbnails once we click on them
		/// </summary>
		private void MakeBuilder()
		{
			GameObject fireworkBuilder = new GameObject("Firework Builder");
			MortarBuilder builder = fireworkBuilder.AddComponent<MortarBuilder>();
			builderGO = builder;
		}

		protected override void addItems()
		{
			// Add all firework launchers
			foreach (Mortar launcher in MortarBuilder.prefabLaunchers)
			{
				if (launcher == null)
				{
					Debug.Log("Null launcher in MortarBuilder!");
					continue;
				}
				addItem(launcher);
			}
		}

		protected override void onBuilt(SerializedMonoBehaviour builtObjectInstance)
		{
			base.onBuilt(builtObjectInstance);
			this.GetComponentInParent<UIWindowFrame>().close();
		}

		public void CleanUp()
		{
			ObjectPoolExtensions.DestroyAll(builderItemEntryGO);
			builderItemEntryGO = null;
		}
	}
}
