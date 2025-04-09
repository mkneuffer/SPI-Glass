using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData
{
    public float volume;
    public string name;


    public SettingsData(float volume, string name)
    {
        this.volume = volume;
        this.name = name;
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
    public string getName()
    {
        return name;
    }

    public void setName(string newName)
    {
        name = newName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
