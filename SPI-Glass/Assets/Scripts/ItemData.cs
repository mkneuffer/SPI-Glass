using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{

    // What does item need
    // public string itemName;
    public bool inInventory;
    public string name;
    public int itemNum;
    [SerializeField] private Sprite itemModel;
    [SerializeField] private GameObject itemIcon;

    public Sprite GetItemIcon() => itemModel;
    public GameObject GetGameObject() => itemIcon;
    public string GetItemName() => name;

}
