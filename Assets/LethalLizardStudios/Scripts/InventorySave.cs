using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventorySave : MonoBehaviour
{
    private static InventorySave _instance;

    public static InventorySave Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<InventorySave>();
            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    private string saveFolder = "PlayerSaves";
    private string type = ".slotdata";
    private string path;

    public void Start()
    {
        path = Application.dataPath + $"/{saveFolder}/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public void Save(Inventory inventory)
    {
        if (inventory == null)
            return;

        if (inventory.Count() == 0)
            return;

        Debug.Log("<color=yellow>SAVING PLAYER INVENTORY</color>");

        string itemStr = inventory.FormatToString();

        if (File.Exists(path + "Player" + type))
        {
            File.WriteAllText(path + "Player" + type, itemStr);
        }
        else
        {
            var sr = File.CreateText(path + "Player" + type);
            sr.Write(itemStr);
            sr.Close();
        }
    }

    public Dictionary<int, Item> Load(Inventory inventory)
    {
        return null;
    }
}
