using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData
{

    public int scene;
    public int quest;

    public SceneData(int scene)
    {
        this.scene = scene;
    }
    // Start is called before the first frame update

    public void nextScene()
    {
        scene++;
    }

    public int getScene()
    {
        return scene;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
