using UnityEngine;

public class CameraAction : MonoBehaviour
{
    public static CameraAction instance { get; private set; }
    public Transform cameraTarget;
    public Vector3 offset;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //Invoke("LookCameraAtTarget", 2f);
    }



    public void LookCameraAtTarget()
    {
        cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;
        transform.SetParent(cameraTarget);
        transform.localPosition = offset;
        transform.localRotation = Quaternion.identity;
    }
}
