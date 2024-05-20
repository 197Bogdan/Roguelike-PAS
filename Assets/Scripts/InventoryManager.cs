using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> items = new List<Item>();

    public Transform ItemContent;
    public GameObject InventoryItem;

    public Toggle EnableRemove;

    public InventoryItemController[] InventoryItems;

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(Item newItem)
    {
        foreach (Item iterItem in items)
        {
            if (iterItem.itemName == newItem.itemName)
            {
                iterItem.quantity += 1;
                return;
            }
        }

        newItem.quantity = 1;
        items.Add(newItem);
    }

    public void RemoveItem(Item item) 
    {
        foreach (Item iterItem in items)
        {
            if (iterItem.itemName == item.itemName)
            {
                iterItem.quantity -= 1;

                if (iterItem.quantity == 0)
                    items.Remove(item);
                
                return;
            }
        }
    }

    public void ListItems()
    {
        //Clean content before open
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in items) 
        {
            GameObject gameObj = Instantiate(InventoryItem, ItemContent);

            var itemName = gameObj.transform.Find("ItemName").GetComponent<Text>();
            var itemImage = gameObj.transform.Find("ItemImage").GetComponent<Image>();
            var removeButton = gameObj.transform.Find("RemoveButton").GetComponent<Button>();

            itemName.text = item.itemName;
            itemImage.sprite = item.icon;

            if(EnableRemove.isOn) 
            {
                removeButton.gameObject.SetActive(true);
            }
        }

        SetInventoryItems();
    }

    public void EnableItemsRemove()
    {
        if(EnableRemove.isOn)
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(true);
            }
        } 
        else
        {
            foreach (Transform item in ItemContent)
            {
                item.Find("RemoveButton").gameObject.SetActive(false);
            }
        }
    }

    public void SetInventoryItems()
    {
        InventoryItems = ItemContent.GetComponentsInChildren<InventoryItemController>();

        for (int i = 0; i < items.Count; i++)
        {
            InventoryItems[i].AddItem(items[i]);
        }
    }
}
