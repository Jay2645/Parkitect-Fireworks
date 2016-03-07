using Parkitect.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Fireworks.UI
{
	public class FireworkBuilderTab : BuilderTab
	{
		protected override void Awake()
		{
			MakeUIItemEntry();

			MakeItemDisplayPanel();

			base.Awake();
		}

		private void MakeUIItemEntry()
		{
			// Make Builder Entry
			GameObject uiBuilderEntry = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			// Requires a button and a RawImage
			uiBuilderEntry.AddComponent<CanvasRenderer>();
			uiBuilderEntry.AddComponent<RawImage>();
			Button button = uiBuilderEntry.AddComponent<Button>();
			FlatRideBuilderEntry builderEntry = uiBuilderEntry.AddComponent<FlatRideBuilderEntry>();
			builderEntry.name = "UI Entry";

			// Also requires texts for the name and the price of the entry
			GameObject sizeTextGO = Instantiate(FireworksUIBuilder.rectTfmPrefab);
			sizeTextGO.name = "Size Text";
			GameObject nameTextGO = Instantiate(sizeTextGO);
			nameTextGO.name = "Name Text";
			GameObject priceTextGO = Instantiate(sizeTextGO);
			priceTextGO.name = "Price Text";
			sizeTextGO.transform.SetParent(uiBuilderEntry.transform, false);
			nameTextGO.transform.SetParent(uiBuilderEntry.transform, false);
			priceTextGO.transform.SetParent(uiBuilderEntry.transform, false);
			sizeTextGO.AddComponent<CanvasRenderer>();
			priceTextGO.AddComponent<CanvasRenderer>();
			nameTextGO.AddComponent<CanvasRenderer>();
			builderEntry.sizeText = sizeTextGO.AddComponent<Text>();
			builderEntry.priceText = priceTextGO.AddComponent<Text>();
			builderEntry.nameText = nameTextGO.AddComponent<Text>();

			// Add it to our ItemEntryGO template
			builderItemEntryGO = builderEntry;
		}

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

		private void MakeBuilder()
		{
			GameObject fireworkBuilder = new GameObject("Firework Builder");
			FlatRideBuilder builder = fireworkBuilder.AddComponent<FlatRideBuilder>();
			builderGO = builder;
		}

		protected override void addItems()
		{
			foreach (Attraction attraction in ScriptableSingleton<AssetManager>.Instance.getAttractionObjects())
			{
				if (attraction is FlatRide)
				{
					categoryTag = attraction.categoryTag;
					addItem(attraction);
				}
			}
		}

		protected override void addItemEntryFor(BuildableObject item)
		{
			base.addItemEntryFor(item);
		}

		protected override void onBuilt(SerializedMonoBehaviour builtObjectInstance)
		{
			base.onBuilt(builtObjectInstance);
			this.GetComponentInParent<UIWindowFrame>().close();
		}
	}
}
