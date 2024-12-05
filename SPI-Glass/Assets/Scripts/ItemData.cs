using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
=======
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
>>>>>>> Stashed changes
public class ItemData : MonoBehaviour
{

    // What does item need
    public bool inInventory;
    public string name;
    public int itemNum;
    [SerializeField] private GameObject itemModel;
<<<<<<< Updated upstream
    [SerializeField] private GameObject itemIcon;
    
=======
    [SerializeField] private Sprite itemIcon;

    public Sprite GetItemIcon() => itemIcon;
    public GameObject GetGameObject() => itemModel;
    public string GetItemName() => name;

>>>>>>> Stashed changes
}
