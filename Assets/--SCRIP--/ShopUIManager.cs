using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject itemPrefab;  // The single item prefab
    public Transform cosmeticContent;  // Content panel for the cosmetic shop
    public GameObject shopPanel;
    public Scrollbar verticalScrollbar;
    public List<ItemData> cosmeticItems = new List<ItemData>();  // List of items in the cosmetic shop

    void Start()
    {
        shopPanel.SetActive(false);
        verticalScrollbar.gameObject.SetActive(true);
    }

    public void ToggleShopPanel()
    {
        bool isActive = shopPanel.activeSelf;


        // Toggle the shop panel visibility
        shopPanel.SetActive(!isActive);

        // Force the Scrollbar to be enabled if it's disabled
        verticalScrollbar.gameObject.SetActive(true);
        verticalScrollbar.enabled = true;  // Make sure the scrollbar is enabled
        verticalScrollbar.value = 0f;  // Scroll to top
        verticalScrollbar.size = 0.3f;
        Debug.Log("happ");

        // Force a layout update (if necessary)
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalScrollbar.GetComponent<RectTransform>());

        // When re-enabling, restore the scrollbar value
        if (!isActive)
        {
            PopulatePanel(cosmeticContent, cosmeticItems);
        }
    }

    // Populate the panel with items dynamically
    void PopulatePanel(Transform content, List<ItemData> items)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Loop through each cosmetic item and instantiate the UI elements
        foreach (ItemData item in items)
        {
            // Only instantiate if the item is not purchased (using player's owned items data)
            if (Player.Instance.ownedBanners[item.itemID] == 0)  // Check if the item is not owned
            {
                GameObject itemObj = Instantiate(itemPrefab, content);  // Instantiate the single prefab

                // Find and set components inside the prefab
                itemObj.GetComponentInChildren<TextMeshProUGUI>().text = item.name;  // Set the item name
                itemObj.GetComponentInChildren<Image>().sprite = item.Image;  // Set the item icon

                // Get the purchase button and set up the action
                Button purchaseButton = itemObj.GetComponentInChildren<Button>();
                purchaseButton.onClick.RemoveAllListeners();  // Ensure no previous listeners are attached
                purchaseButton.onClick.AddListener(() => PurchaseItem(item));  // Add purchase action

                // Optionally, you can change the button text depending on whether the item is purchased
                TextMeshProUGUI buttonText = purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Buy";  // Button text will always be "Buy" if the item is not purchased

                Debug.Log("Display content");
            }

            if (Player.Instance.ownedBanners[item.itemID] == 1)
            {
                Debug.Log("owned all banners");
                Player.Instance.ownedBanners[item.itemID] = 0;
            }
        }

    }

    // Handle the purchase action
    void PurchaseItem(ItemData item)
    {
        if (item.isPurchased)
        {
            Debug.Log("Item already purchased!");
            return;
        }

        if (CanAffordItem(item))
        {
            item.isPurchased = true;
            Debug.Log("Item purchased: " + item.name);

            // Remove the item from the shop (i.e., repopulate the shop UI)
            UpdateItemDisplay();
        }
        else
        {
            Debug.Log("Not enough currency to purchase the item!");
        }
    }

    // Check if the player has enough currency (you can adjust this logic)
    bool CanAffordItem(ItemData item)
    {
        // Assume player has a currency system; replace with actual check
        float playerCurrency = 100f;  // Example currency amount
        return playerCurrency >= item.itemPrice;
    }

    // Optionally, update the display to reflect purchased items
    void UpdateItemDisplay()
    {
        PopulatePanel(cosmeticContent, cosmeticItems);  // Re-populate to update button text or other UI elements
    }
}

[System.Serializable]
public class ItemData
{
    public int itemID;
    public string name;
    public Sprite Image;
    public float itemPrice;  // Adding price for purchase
    public bool isPurchased;  // To track if the item is already purchased
}