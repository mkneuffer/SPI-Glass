using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.Experimental.Lightship.AR.WorldPositioning;

public class AddWPSObjects : MonoBehaviour
{
    [SerializeField] ARWorldPositioningObjectHelper positioningHelper;

    // Start is called before the first frame update
    void Start()
    {
        // Placing object at Institute Park Stage, coordinates 42.276461272578814, -71.80675176742707
        // Placing object at Foisie, coordinates 42.274350308882866, -71.80862701089623
        // Placing object at my house bc I forgor and its late :P 42.27312573443756, -71.81572391900738
        
        //home coords
        double home_lat = 42.27312573443756;
        double home_long = -71.81572391900738;

        // Institute Park coords
        double inst_lat = 42.276461272578814;
        double inst_long = -71.80862701089623;

        // Foisie coords
        double foisie_lat = 42.274350308882866;
        double foisie_long = -71.80862701089623;

        // Triangulated point coords
        double center_lat = (home_lat + inst_lat + foisie_lat) / 3;
        double center_long = (home_long + inst_long + foisie_long) / 3;

        
        double all_altitude = 0.0; // We're using camera-relative positioning so make the cube appear at the same height as the camera

        // cube placed at my house
        GameObject home_cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        home_cube.transform.localScale *= 2.0f;
        positioningHelper.AddOrUpdateObject(home_cube, home_lat, home_long, all_altitude, Quaternion.identity);

        // sphere placed at Institute Park
        GameObject inst_sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        inst_sphere.transform.localScale *= 2.0f;
        positioningHelper.AddOrUpdateObject(inst_sphere, inst_lat, inst_long, all_altitude, Quaternion.identity);

        // Cylinder placed at Foisie
        GameObject foisie_cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        foisie_cyl.transform.localScale *= 2.0f;
        positioningHelper.AddOrUpdateObject(foisie_cyl, foisie_lat, foisie_long, all_altitude, Quaternion.identity);

        // Quad placed at centerpoint of all locations
        GameObject center_quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        center_quad.transform.localScale *= 2.0f;
        positioningHelper.AddOrUpdateObject(center_quad, center_lat, center_long, all_altitude, Quaternion.identity).
    }

    // Update is called once per frame
    void Update()
    {

    }
}