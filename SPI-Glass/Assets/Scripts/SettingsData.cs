using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData
{
    public float volume;


    public SettingsData(float volume)
    {
        this.volume = volume;
    }

    public override string ToString()
    {
        return ("volume set to: " + volume);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float getVolume()
    {
        return volume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
