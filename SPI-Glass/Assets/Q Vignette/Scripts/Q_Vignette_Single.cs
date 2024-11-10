using UnityEngine;

using UnityEngine.UI;

using UnityEditor; 




[ExecuteInEditMode] 

public class Q_Vignette_Single : Q_Vignette_Base
{

    // Q_Vignette, a post-processing cheat!

    // The Q_Vignette isn't really a post processing effect at all, but just 4 images placed in the corners of the Canvas to give the Vignette impression
    // If the scale variable is kept low, should see some performance benefits compared to true post processing vignette
    // Other advantages include ease of animation and ability to add a "Sky Vignette" for subtle atmospherics

    // Place the Q_Vignette as a first level child of the main Canvas and ensure its RectTransform is covering the whole screen (Anchors Min to 0, Anchors Max to 1, top, bottom, left right all to 0 - which it should do by default) 

    // The Q_Vignette prefab can sit at the top of your Canvas heirarchy if you just want to affect World Space
    // Alternatively, place it at the bottom of your Canvas heirarchy and it'll affect everything on the Canvas as well

    public float mainScale = 0.4f; // size of vignette from 0 to 2       0.5 and above will see the images overlap and efficiency of this method dropping, although it's a nice effect so if you're not on mobile, it'll be great!
    [System.NonSerialized]public float o_mainScale = 0; // to register change

    public Color mainColor = new Color( 0.1698113f , 0.09192452f , 0.05526878f , 1 ); // shade and transparency of the effect
    [System.NonSerialized]public Color o_mainColor=new Color(0,0,0,0); // to register change

    public int mainCornerType = 0; // the sprite set to use for main corners
    [System.NonSerialized]public int o_mainCornerType; // to register change

    public bool stretchMainToScreenRatio = true; // to have the vignette stretch to screen aspect ratio or remain circular
    [System.NonSerialized]public bool o_stretchMainToScreenRatio = false; // to register change

    

                
    void Start()
    {
        
        CheckReferences();

        SetVignetteMainStretch(stretchMainToScreenRatio);
        SetVignetteSkyStretch(stretchMainToScreenRatio);
        SetVignetteMainSprite(mainCornerType);
        SetVignetteSkySprite(mainCornerType);
        SetVignetteMainColor(mainColor);
        SetVignetteSkyColor(mainColor);
        SetVignetteMainScale(mainScale);
        SetVignetteSkyScale(mainScale);
        
    }

    // #### The following region can be safely commented out if you are NOT planning to ANIMATE the Vignette
    // #### note: without the following code it may still jerkily animate in the editor, but it wont animate in the build

    #region just for animating
        void Update()
        {
            // having an update check is the easiest way to do this (I think)!
            // getters and setters don't seem to like Inspector, nor custom Editor scripts, nor being animated!
            CheckReferences();
            UpdateVignette();
        }    
    #endregion





    public void UpdateVignette()
    {
        CheckVignetteSprite();
        CheckVignetteStretch();
        CheckVignetteScale();
        CheckVignetteColor();
        
    }


    

    void CheckVignetteSprite()
    {
        if ( mainCornerType != o_mainCornerType )
        {
            mainCornerType = Mathf.Clamp( mainCornerType , 0 , cornerTypeSprites.Length-1 );
            o_mainCornerType = mainCornerType;
            SetVignetteMainSprite( mainCornerType );
            SetVignetteSkySprite( mainCornerType );



        }
    }

    void CheckVignetteStretch()
    {
        if ( stretchMainToScreenRatio != o_stretchMainToScreenRatio ){
            o_stretchMainToScreenRatio = stretchMainToScreenRatio;

            
            SetVignetteMainStretch( stretchMainToScreenRatio );
            SetVignetteSkyStretch( stretchMainToScreenRatio );
            
        }
    }

    void CheckVignetteColor()
    {
        if ( mainColor != o_mainColor)
        {
            o_mainColor=mainColor;
            SetVignetteSkyColor( mainColor );
            SetVignetteMainColor( mainColor );
        }
    }

    void CheckVignetteScale()
    {
        if ( mainScale != o_mainScale ){
            mainScale = Mathf.Clamp( mainScale , 0f , 5f );
            o_mainScale = mainScale;
            SetVignetteSkyScale( mainScale );
            SetVignetteMainScale( mainScale );
        }     
    }

    


}

