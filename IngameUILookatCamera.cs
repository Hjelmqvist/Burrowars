using UnityEngine;

/// <summary>
/// Class for ingame UI elements to always rotate towards the camera
/// </summary>
public class IngameUILookatCamera : MonoBehaviour
{
    void Update()
    {
        Vector3 target = Camera.main.transform.position;
        target.x = transform.position.x;
        transform.LookAt(target);
    }
}
