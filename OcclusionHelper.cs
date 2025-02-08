using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionHelper : MonoBehaviour
{
    // holds invisible material
    public Material invisibleMat;

    // Set all walls to invisible material in beginning.
    // all walls the should hide other objects behind should be children of WallController object.
    void Start()
    {
        // get all renderers in this object and its children:
        var renders = GetComponentsInChildren<Renderer>();
        foreach (var render in renders)
        {
            render.material = invisibleMat;
        }
    }
}
