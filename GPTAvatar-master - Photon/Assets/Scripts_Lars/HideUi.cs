using UnityEngine;
using UnityEngine.UI;

public class HideUi : MonoBehaviour
{
    // Reference to the RawImage component
    private RawImage rawImage;

    // Start is called before the first frame update
    void Start()
    {
        // Get the RawImage component attached to this GameObject
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the "H" key is pressed
        if (Input.GetKeyDown(KeyCode.H))
        {
            // Toggle the visibility of the image
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

    // Function to hide the image
    void HideImage()
    {
        rawImage.enabled = false;
    }

    // Function to show the image
    void ShowImage()
    {
        rawImage.enabled = true;
    }
}
