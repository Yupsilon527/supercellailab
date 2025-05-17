using UnityEngine;

public class EntOverlay : MonoBehaviour
{
    
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
