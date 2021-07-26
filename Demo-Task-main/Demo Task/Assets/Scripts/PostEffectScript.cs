using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostEffectScript : MonoBehaviour
{
    public Material mat;
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
    //src is the full rendered scene that you would normally
    //send directly to the monitor
    //Doing Image effect in CPU
    Graphics.Blit(src, dest, mat);
    }
}
