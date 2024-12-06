using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EMFManager : MonoBehaviour
{
    [SerializeField] private Image EMFLevel1;
    [SerializeField] private Image EMFLevel2;
    [SerializeField] private Image EMFLevel3;
    [SerializeField] private Image EMFLevel4;
    [SerializeField] private Image EMFLevel5;
    [SerializeField] private GameObject currentGhost;
    [SerializeField] private GameObject player;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider hauntedZone;
    [SerializeField] private GameObject EMF;

    private float distance;

    private Color Level1_Off = new Color32(0, 69, 71, 255);
    private Color Level1_On = new Color32(0, 250, 239, 255);
    private Color Level2_Off = new Color32(1, 51, 10, 255);
    private Color Level2_On = new Color32(19, 250, 84, 255);
    private Color Level3_Off = new Color32(43, 51, 1, 255);
    private Color Level3_On = new Color32(252, 235, 5, 255);
    private Color Level4_Off = new Color32(51, 32, 1, 255);
    private Color Level4_On = new Color32(252, 153, 5, 255);
    private Color Level5_Off = new Color32(51, 1, 1, 255);
    private Color Level5_On = new Color32(252, 17, 17, 255);
    // Start is called before the first frame update
    void Start()
    {
        EMFLevel1.color = Level1_Off;
        EMFLevel2.color = Level2_Off;
        EMFLevel3.color = Level3_Off;
        EMFLevel4.color = Level4_Off;
        EMFLevel5.color = Level5_Off;
        Debug.Log("EMF colors updated");
    }

    // Update is called once per frame
    void Update()
    {
        getDistance();
        updateEMF();
    }

    public float getDistance()
    {
        float ghostX = currentGhost.transform.position.x;
        float ghostZ = currentGhost.transform.position.z;
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        distance = Mathf.Abs(ghostX - playerX) + Mathf.Abs(ghostZ - playerY);
        //Debug.Log("Distance: " + distance);
        return distance;
    }

    public void updateEMF()
    {
        if (distance < 0)
        {
            //Debug.Log("negative distance");
        }
        else if (distance > 0 && distance <= 100)
        {
            EMFLevel1.color = Level1_On;
            EMFLevel2.color = Level2_On;
            EMFLevel3.color = Level3_On;
            EMFLevel4.color = Level4_On;
            EMFLevel5.color = Level5_On;
            //Debug.Log("Level 5");
        }
        else if (distance > 100 && distance <= 120)
        {
            EMFLevel1.color = Level1_On;
            EMFLevel2.color = Level2_On;
            EMFLevel3.color = Level3_On;
            EMFLevel4.color = Level4_On;
            EMFLevel5.color = Level5_Off;
            //Debug.Log("Level 4");
        }
        else if (distance > 120 && distance <= 140)
        {
            EMFLevel1.color = Level1_On;
            EMFLevel2.color = Level2_On;
            EMFLevel3.color = Level3_On;
            EMFLevel4.color = Level4_Off;
            EMFLevel5.color = Level5_Off;
            //Debug.Log("Level 3");
        }
        else if (distance > 140 && distance <= 160)
        {
            EMFLevel1.color = Level1_On;
            EMFLevel2.color = Level2_On;
            EMFLevel3.color = Level3_Off;
            EMFLevel4.color = Level4_Off;
            EMFLevel5.color = Level5_Off;
            //Debug.Log("Level 2");
        }
        else if (distance > 160 && distance <= 180)
        {
            EMFLevel1.color = Level1_On;
            EMFLevel2.color = Level2_Off;
            EMFLevel3.color = Level3_Off;
            EMFLevel4.color = Level4_Off;
            EMFLevel5.color = Level5_Off;
            //Debug.Log("Level 1");
        }
        else
        {
            EMFLevel1.color = Level1_Off;
            EMFLevel2.color = Level2_Off;
            EMFLevel3.color = Level3_Off;
            EMFLevel4.color = Level4_Off;
            EMFLevel5.color = Level5_Off;
            //Debug.Log("Level 0");
        }
    }
    
    public void setEMFActive(bool state)
    {
        EMF.SetActive(state);
        Debug.Log("EMF state changed");
    }
}
