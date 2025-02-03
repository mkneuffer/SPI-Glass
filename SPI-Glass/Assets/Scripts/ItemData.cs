using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : MonoBehaviour
{

    // What does item need
    // public string itemName;
    public bool inInventory;
    public string name;
    public int itemNum;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private GameObject itemModel;

    public Sprite GetItemIcon() => itemIcon;
    public GameObject GetGameObject() => itemModel;
    public string GetItemName() => name;

    public Sprite getIcon()
    {
        return itemIcon;
    }

    public int getNum()
    {
        return itemNum;
    }
}
