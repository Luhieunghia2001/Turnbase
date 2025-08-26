using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("MainCamera");
        if (camObj != null)
        {
            cam = camObj.transform;
        }
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            // Giữ cho UI song song với mặt phẳng camera
            transform.rotation = cam.rotation;
        }
    }
}
