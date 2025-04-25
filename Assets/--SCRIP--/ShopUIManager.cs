using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject itemPrefab;  // The single item prefab
    public Transform cosmeticContent;  // Content panel for the cosmetic shop
    public Transform inventoryContent;
    public GameObject shopPanel;
    public GameObject inventoryPanel;
    public TextMeshProUGUI tokensText;

    public GameObject menuBanner;
    public GameObject accBanner;

    public List<ItemData> cosmeticItems = new List<ItemData>();  // List of items in the cosmetic shop

    

    void Start()
    {
        shopPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        menuBanner.GetComponent<Image>().sprite = cosmeticItems[Player.Instance.currentBanner].Image;
        accBanner.GetComponent<Image>().sprite = cosmeticItems[Player.Instance.currentBanner].Image;
    }

    public void ToggleShopPanel()
    {
        bool isActive = shopPanel.activeSelf;


        // Toggle the shop panel visibility
        shopPanel.SetActive(!isActive);
        Debug.Log("happ");

        // When re-enabling, restore the scrollbar value
        if (!isActive)
        {
            PopulatePanel(cosmeticContent, cosmeticItems);
        }
    }

    public void ToggleInventoryPanel()
    {
        bool isActive = inventoryPanel.activeSelf;


        // Toggle the shop panel visibility
        inventoryPanel.SetActive(!isActive);
        Debug.Log("happ");

        // When re-enabling, restore the scrollbar value
        if (!isActive)
        {
            PopulateInventory(inventoryContent, cosmeticItems);
        }
    }

    // Populate the panel with items dynamically
    void PopulatePanel(Transform content, List<ItemData> items)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }


        int i = 0;
        // Loop through each cosmetic item and instantiate the UI elements
        foreach (ItemData item in items)
        {
            //Debug.Log(i);
            // Only instantiate if the item is not purchased (using player's owned items data)
            //if (i == 19)
                //continue;
            item.isPurchased = Player.Instance.ownedBanners[i] == 0 ? false : true;
            i++;
            
            if (item.isPurchased == false)  // Check if the item is not owned
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
                buttonText.text = item.itemPrice.ToString();  // Button text will always be "Buy" if the item is not purchased

                Debug.Log("Display content");
            }

            /*if (Player.Instance.ownedBanners[item.itemID] == 1)
            {
                Debug.Log("owned all banners");
                Player.Instance.ownedBanners[item.itemID] = 0;
            }*/
        }

    }

    void PopulateInventory(Transform content, List<ItemData> items)
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }


        int i = 0;
        // Loop through each cosmetic item and instantiate the UI elements
        foreach (ItemData item in items)
        {
            //Debug.Log(i);
            // Only instantiate if the item is not purchased (using player's owned items data)
            //if (i == 19)
            //continue;
            item.isPurchased = Player.Instance.ownedBanners[i] == 0 ? false : true;
            i++;

            if (item.isPurchased == true)  // Check if the item is  owned
            {
                GameObject itemObj = Instantiate(itemPrefab, content);  // Instantiate the single prefab

                // Find and set components inside the prefab
                itemObj.GetComponentInChildren<TextMeshProUGUI>().text = item.name;  // Set the item name
                itemObj.GetComponentInChildren<Image>().sprite = item.Image;  // Set the item icon

                Button equipButton = itemObj.GetComponentInChildren<Button>();
                equipButton.onClick.RemoveAllListeners();  // Ensure no previous listeners are attached
                equipButton.onClick.AddListener(() => EquipCos(item));  

                // Optionally, you can change the button text depending on whether the item is purchased
                TextMeshProUGUI buttonText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "Equip";

                Debug.Log("Display content");
            }

            /*if (Player.Instance.ownedBanners[item.itemID] == 1)
            {
                Debug.Log("owned all banners");
                Player.Instance.ownedBanners[item.itemID] = 0;
            }*/
        }

    }

    void EquipCos(ItemData item)
    {
        item.equipped = true;
        menuBanner.GetComponent<Image>().sprite = item.Image;
        accBanner.GetComponent<Image>().sprite = item.Image;
        Player.Instance.currentBanner = item.itemID;
        Player.Instance.SavePlayer();
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
            Player.Instance.tokens -= (int)item.itemPrice;
            
            item.isPurchased = true;
            Debug.Log("Item purchased: " + item.isPurchased);
            Player.Instance.ownedBanners[item.itemID] = 1;
            
            // Remove the item from the shop (i.e., repopulate the shop UI)

            Player.Instance.SavePlayer();
            //Player.Instance.LoadPlayer();
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
        float playerCurrency = Player.Instance.tokens;
        return playerCurrency >= item.itemPrice;
    }

    // Optionally, update the display to reflect purchased items
    void UpdateItemDisplay()
    {
        PopulatePanel(cosmeticContent, cosmeticItems);  // Re-populate to update button text or other UI elements
        tokensText.text = Player.Instance.tokens.ToString();
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
    public bool equipped;
}