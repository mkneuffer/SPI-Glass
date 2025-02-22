using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAnimation : MonoBehaviour
{
    public static ChestAnimation Instance;
    [SerializeField] Animator openChest;
    [SerializeField] private string chestOpen = "OpenChest";

    public void playChestAnim()
    {
        openChest.Play(chestOpen, 0, 0.0f );
    }
}
