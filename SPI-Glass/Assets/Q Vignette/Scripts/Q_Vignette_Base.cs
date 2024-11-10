using UnityEngine;

using UnityEngine.UI;

using UnityEditor; 





[ExecuteInEditMode] 
public class Q_Vignette_Base : MonoBehaviour
{

    // Q_Vignette, a post-processing cheat!

    // The Q_Vignette isn't really a post processing effect at all, but just 4 images placed in the corners of the Canvas to give the Vignette impression
    // If the scale variable is kept low, should see some performance benefits compared to true post processing vignette
    // Other advantages include ease of animation and ability to add a "Sky Vignette" for subtle atmospherics

    // Place the Q_Vignette as a first level child of the main Canvas and ensure its RectTransform is covering the whole screen (Anchors Min to 0, Anchors Max to 1, top, bottom, left right all to 0 - which it should do by default) 

    // The Q_Vignette prefab can sit at the top of your Canvas heirarchy if you just want to affect World Space
    // Alternatively, place it at the bottom of your Canvas heirarchy and it'll affect everything on the Canvas as well

    [System.Serializable]
    public class CornerSprites{
        [SerializeField] public Sprite topLeft;
        [SerializeField] public Sprite topRight;
        [SerializeField] public Sprite bottomLeft;
        [SerializeField] public Sprite bottomRight;
    }

    // Just add more if you feel you need to
    public CornerSprites[] cornerTypeSprites;
    
    // it's useful to have these serialised but there's no need to have them exposed
    [HideInInspector] public RectTransform[] cornerRects;
    [HideInInspector] public Image[] cornerImages;
    


    public virtual void Awake()
    {
        CheckReferences();
    }


    // just in case the script wakes up without any references
    public void CheckReferences()
    {
        if ( cornerImages == null || cornerImages.Length == 0 ){
            GetCornerImages();
        }
        if ( cornerRects == null || cornerRects.Length == 0 ){
            GetCornerRects();
        }
    }
    void GetCornerImages()
    {
        cornerImages = GetComponentsInChildren<Image>();
    }
    void GetCornerRects()
    {
        cornerRects = new RectTransform[4];
        for (int n = 0; n < cornerImages.Length; n++ ){
            cornerRects[n] = cornerImages[n].transform.parent.GetComponent <RectTransform> ();
        }
    }
    


    public void SetVignetteMainStretch( bool stretch )
    {
        cornerImages[0].preserveAspect =! stretch;
        cornerImages[1].preserveAspect =! stretch; 
    }

    public void SetVignetteSkyStretch( bool stretch )
    {
        cornerImages[2].preserveAspect =! stretch;
        cornerImages[3].preserveAspect =! stretch;
    }

    public void SetVignetteMainSprite( int type )
    {
        cornerImages[1].sprite = cornerTypeSprites[type].bottomLeft;
        cornerImages[0].sprite = cornerTypeSprites[type].bottomRight;
    }

    public void SetVignetteSkySprite( int type )
    {
        cornerImages[2].sprite = cornerTypeSprites[type].topLeft;
        cornerImages[3].sprite = cornerTypeSprites[type].topRight;
    }

    public void SetVignetteSkyColor( Color color )
    {
        cornerImages[2].color = color;
        cornerImages[3].color = color;
    }

    public void SetVignetteMainColor( Color color )
    {
        cornerImages[0].color = color;
        cornerImages[1].color = color; 
    }

    public void SetVignetteSkyScale( float scale )
    {
        cornerRects[2].anchorMin = new Vector2( 0 , 1 - scale );
        cornerRects[2].anchorMax = new Vector2( scale , 1 );
        cornerRects[3].anchorMin = new Vector2( 1 - scale , 1 - scale );
        cornerRects[3].anchorMax = new Vector2( 1 , 1 );
    }

    public void SetVignetteMainScale( float scale )
    {
        cornerRects[0].anchorMin=new Vector2( 1 - scale , 0 );
        cornerRects[0].anchorMax=new Vector2( 1 , scale );
        cornerRects[1].anchorMin=new Vector2( 0 , 0 );
        cornerRects[1].anchorMax=new Vector2( scale , scale );
    }

}

