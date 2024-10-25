using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{

    // What does item need
    public bool inInventory;
    public string name;
    public int itemNum;
    [SerializeField] private Sprite itemModel;
    [SerializeField] private GameObject itemIcon;
    
}
