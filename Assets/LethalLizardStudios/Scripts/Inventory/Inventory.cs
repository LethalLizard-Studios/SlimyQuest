/* Rights Reserved to Leland TL Carter of LethalLizard Studios ©2024
-- Last Change: 10/28/2023
*/

using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Inventory>();
            return _instance;
        }
    }

    [SerializeField] private Hotbar hotbar;
    [SerializeField] private Dictionary<int, Item> itemsInInventory = new Dictionary<int, Item>();

    private void Start()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
            _instance = this;
    }

    private int CheckForParent(int blockID)
    {
        if (blockID <= 0)
            return -1;

        Block block = Registry.AtIndex(blockID - 1);

        return block.m_properties.m_hasOtherDrop ? block.m_properties.m_otherdrop.ID : blockID;
    }

    public int Count() { return itemsInInventory.Count; }

    public string FormatToString()
    {
        string itemStr = "";

        foreach (var item in itemsInInventory)
            itemStr += "/" + item.Key + "," + item.Value.slot + "," + item.Value.Count();

        return itemStr;
    }

    public Item AddItem(int blockID)
    {
        blockID = CheckForParent(blockID);

        if (itemsInInventory.ContainsKey(blockID))
            itemsInInventory[blockID].Add();
        else
        {
            int slot = hotbar.FindEmptySlot();
            if (slot != -2)
                itemsInInventory.Add(blockID, new Item(blockID, slot));
            else
                return null;
        }
        hotbar.SetSlot(itemsInInventory[blockID].slot, blockID, itemsInInventory[blockID].Count());

        return itemsInInventory[blockID];
    }

    public Item AddCreativeItem(int blockID)
    {
        blockID = CheckForParent(blockID);

        if (itemsInInventory.ContainsKey(blockID))
            itemsInInventory[blockID].AddCreative();
        else
        {
            int slot = hotbar.FindEmptySlot();
            if (slot != -2)
                itemsInInventory.Add(blockID, new Item(blockID, slot));
        }
        hotbar.SetSlot(itemsInInventory[blockID].slot, blockID, itemsInInventory[blockID].Count());

        return itemsInInventory[blockID];
    }

    public Item RemoveItem(int blockID)
    {
        blockID = CheckForParent(blockID);

        if (itemsInInventory.ContainsKey(blockID))
        {
            if (itemsInInventory[blockID].Remove())
                itemsInInventory.Remove(blockID);
            else
                return itemsInInventory[blockID];
        }

        return null;
    }
}

[System.Serializable]
public class Item
{
    public int slot;

    private int ID;
    private int amount;

    public Item(int ID, int slot)
    {
        this.ID = ID;
        this.slot = slot;
        amount = 1;
    }

    public int Identify() { return ID; }
    public int Count() { return amount; }

    public void AddCreative() { amount = 99; }

    public void Add() { amount++; }
    public bool Remove()
    {
        amount--;
        if (amount <= 0)
            return true;
        return false;
    }
}
