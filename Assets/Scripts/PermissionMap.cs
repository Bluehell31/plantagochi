using UnityEngine;
using UnityEngine.Android;

public class PermissionMap : MonoBehaviour
{
    void Start()
    {
        #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }

        if (!Input.location.isEnabledByUser)
        {
            Input.location.Start();
        }
        #endif
    }
}