using UnityEngine;
using UnityEngine.UI;

public class HideUi : MonoBehaviour
{
    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (rawImage.enabled)
            {
                HideImage();
            }
            else
            {
                ShowImage();
            }
        }
    }

    void HideImage()
    {
        rawImage.enabled = false;
    }

    void ShowImage()
    {
        rawImage.enabled = true;
    }
}
