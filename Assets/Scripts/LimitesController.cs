using UnityEngine;
using Mapbox.Unity.Map;
public class LimitesController : MonoBehaviour 
{
    [SerializeField] private AbstractMap map;
    [SerializeField] private float mapLimitXPercent = 0.5f;
    [SerializeField] private float mapLimitZPercent = 0.5f;
    [SerializeField] private float smoothSpeed = 5f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float screenRatio;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = transform.position;
        screenRatio = (float)Screen.width / Screen.height;
    }

    void Update() 
    {
        targetPosition = transform.position;
        float actualLimitX = mapLimitXPercent * screenRatio * 100f;
        float actualLimitZ = mapLimitZPercent * 100f;

        float distanceX = Mathf.Abs(targetPosition.x - initialPosition.x);
        float distanceZ = Mathf.Abs(targetPosition.z - initialPosition.z);

        if (distanceX > actualLimitX || distanceZ > actualLimitZ)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, 
                initialPosition.x - actualLimitX, 
                initialPosition.x + actualLimitX);
            targetPosition.z = Mathf.Clamp(targetPosition.z, 
                initialPosition.z - actualLimitZ, 
                initialPosition.z + actualLimitZ);

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
    }
}
