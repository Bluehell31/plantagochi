using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.EventSystems;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] private Camera arCam;
    [SerializeField] private GameObject arrowUp;
    [SerializeField] private GameObject arrowDown;
    [SerializeField] private GameObject arrowLeft;
    [SerializeField] private GameObject arrowRight;
    
    private GameObject targetPlant;

    public void SetTargetPlant(GameObject plant)
    {
        targetPlant = plant;
    }

    void Update()
    {
        if (targetPlant == null) return;

        Vector3 viewportPosition = arCam.WorldToViewportPoint(targetPlant.transform.position);

        bool isVisible = viewportPosition.z > 0 &&
                         viewportPosition.x > 0 && viewportPosition.x < 1 &&
                         viewportPosition.y > 0 && viewportPosition.y < 1;

        if (isVisible)
        {
            HideArrows();
        }
        else
        {
            ShowCorrectArrow(viewportPosition);
        }
    }

    void HideArrows()
    {
        arrowUp.SetActive(false);
        arrowDown.SetActive(false);
        arrowLeft.SetActive(false);
        arrowRight.SetActive(false);
    }

    void ShowCorrectArrow(Vector3 viewportPosition)
    {
        HideArrows();

        if (viewportPosition.x < 0.5f)
        {
            arrowLeft.SetActive(true);
        }
        else
        {
            arrowRight.SetActive(true);
        }

        if (viewportPosition.y < 0.5f)
        {
            arrowDown.SetActive(true);
        }
        else
        {
            arrowUp.SetActive(true);
        }
    }
}
