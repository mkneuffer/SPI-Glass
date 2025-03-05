using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserBeam
{
    // Add static reference to track the last created laser
    private static GameObject lastLaserObj;

    Vector3 pos, dir;
    GameObject laserObj;
    LineRenderer laser;
    List<Vector3> laserIndices = new List<Vector3>();
    ThiefGhost ghost;
    private bool leftColliderHit;
    private bool rightColliderHit;
    private bool frontColliderHit;
    private bool backColliderHit;
    private bool leftFrontColliderHit;
    private bool rightFrontColliderHit;
    private bool leftBackColliderHit;
    private bool rightBackColliderHit;

    public LaserBeam(Vector3 pos, Vector3 dir, Material material)
    {
        // Destroy the previous laser before creating a new one
        if (lastLaserObj != null)
        {
            GameObject.Destroy(lastLaserObj);
        }

        this.laserObj = new GameObject();
        this.laserObj.name = "Laser Beam";
        // Store reference to current laser
        lastLaserObj = this.laserObj;

        this.pos = pos;
        this.dir = dir;
        this.laser = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;

        this.laser.startWidth = 0.1f;
        this.laser.endWidth = 0.1f;
        this.laser.material = material;
        this.laser.startColor = Color.green;
        this.laser.endColor = Color.green;
        leftColliderHit = false;
        rightColliderHit = false;
        frontColliderHit = false;
        backColliderHit = false;
        leftFrontColliderHit = false;
        rightFrontColliderHit = false;
        leftBackColliderHit = false;
        rightBackColliderHit = false;
        CastRay(pos, dir, laser);
    }

    void CastRay(Vector3 pos, Vector3 dir, LineRenderer laser)
    {
        laserIndices.Add(pos);

        Ray ray = new Ray(pos, dir);
        RaycastHit[] hits = new RaycastHit[10];
        int hitCount = Physics.RaycastNonAlloc(ray, hits, 30, 1);

        if (hitCount > 0)
        {
            // Find and process the closest hits first
            for (int i = 0; i < hitCount - 1; i++)
            {
                for (int j = i + 1; j < hitCount; j++)
                {
                    if (hits[j].distance < hits[i].distance)
                    {
                        // Swap elements to sort by distance (simple Bubble Sort swap)
                        RaycastHit temp = hits[i];
                        hits[i] = hits[j];
                        hits[j] = temp;
                    }
                }
            }
            CheckHit(hits, dir, laser, hitCount, ray);
        }
        else
        {
            laserIndices.Add(ray.GetPoint(30));
        }
        UpdateLaser();
    }

    void UpdateLaser()
    {
        int count = 0;
        laser.positionCount = laserIndices.Count;

        foreach (Vector3 idx in laserIndices)
        {
            laser.SetPosition(count, idx);
            count++;
        }
    }
    void CheckHit(RaycastHit[] hits, Vector3 direction, LineRenderer laser, int hitCount, Ray ray)
    {
        bool collideWithMirror = false;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hitInfo = hits[i];
            //Debug.Log($"Collision detected with: {hitInfo.collider.gameObject.name} (Tag: {hitInfo.collider.gameObject.tag})");
            if (hitInfo.collider.gameObject.CompareTag("Mirror"))
            {
                Vector3 pos = hitInfo.point;
                Vector3 dir = Vector3.Reflect(direction, hitInfo.normal);
                CastRay(pos, dir, laser);
                collideWithMirror = true;
                break;
            }

            if (hitInfo.collider.gameObject.CompareTag("Ghost"))
            {
                ThiefGhost ghost = hitInfo.transform.gameObject.GetComponent<ThiefGhost>();
                ghost.StunGhost();
            }
            else if (hitInfo.collider.gameObject.CompareTag("GhostLaserColliderFront"))
            {
                frontColliderHit = true;
            }
            else if (hitInfo.collider.gameObject.CompareTag("GhostLaserColliderBack"))
            {
                backColliderHit = true;
            }
            else if (hitInfo.collider.gameObject.CompareTag("GhostLaserColliderLeft"))
            {
                leftColliderHit = true;
            }
            else if (hitInfo.collider.gameObject.CompareTag("GhostLaserColliderRight"))
            {
                rightColliderHit = true;
            }
            else if (hitInfo.collider.gameObject.name.Equals("ColliderBackRight"))
            {
                rightBackColliderHit = true;
            }
            else if (hitInfo.collider.gameObject.name.Equals("ColliderBackLeft"))
            {
                leftBackColliderHit = true;
            }
            else if (hitInfo.collider.gameObject.name.Equals("ColliderFrontRight"))
            {
                rightFrontColliderHit = true;
            }
            else if (hitInfo.collider.gameObject.name.Equals("ColliderFrontLeft"))
            {
                leftFrontColliderHit = true;
            }
            else if (!collideWithMirror)
            {
                laserIndices.Add(hitInfo.point);
                UpdateLaser();
            }
        }
        if (!collideWithMirror)
        {
            laserIndices.Add(ray.GetPoint(30));
        }
        int count = BoolToInt(leftBackColliderHit) + BoolToInt(rightBackColliderHit) + BoolToInt(leftFrontColliderHit) + BoolToInt(rightFrontColliderHit) + BoolToInt(leftColliderHit) + BoolToInt(backColliderHit) + BoolToInt(rightColliderHit) + BoolToInt(frontColliderHit);

        ThiefGhost thiefGhost = GameObject.Find("ThiefGhost(Clone)").GetComponent<ThiefGhost>();
        //Ghost Trapped
        thiefGhost.UpdateTrapped(count >= 8);
    }


    private int BoolToInt(bool b)
    {
        return b ? 1 : 0;
    }
}