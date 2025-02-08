using UnityEngine;

public class Obscurable : MonoBehaviour
{
    void Start()
    {
        // get all renderers in this object and its children:
        var renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renders)
        {
            renderer.material.renderQueue = 2002; // set their renderQueue
        }
    }
}
