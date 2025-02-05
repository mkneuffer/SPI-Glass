using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData
{

    public int scene;
    public int quest;

    public SceneData(int scene, int quest)
    {
        this.scene = scene;
        this.quest = quest;
    }
    // Start is called before the first frame update

    public void nextScene()
    {
        scene++;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
