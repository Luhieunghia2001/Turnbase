using UnityEngine;

public class CameraAction : MonoBehaviour
{
    public static CameraAction instance { get; private set; }

    public Transform targetPoint;
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

    /// <summary>
    /// Gọi để camera bám vào CameraTarget của nhân vật đang tới lượt.
    /// </summary>
    public void LookCameraAtTarget(Character character)
    {
        if (character == null) return;

        targetPoint = character.transform.Find("CameraTarget");

        if (targetPoint != null)
        {
            transform.SetParent(targetPoint);
            transform.localPosition = offset;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning($"{character.name} không có child 'CameraTarget'!");
        }
    }
    
}
