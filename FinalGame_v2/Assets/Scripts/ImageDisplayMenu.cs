using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplay : MonoBehaviour
{
    public Image displayImage;
    public Button nextButton;
    //public Button backButton;

    private List<string> imagePaths = new List<string>();
    private int currentIndex = 0;

    private void Start()
    {
        // Load all images on start
        LoadImages();
        nextButton.onClick.AddListener(ShowNextImage);
        //backButton.onClick.AddListener(ShowPreviousImage);

        // Show first image if available
        if (imagePaths.Count > 0)
        {
            DisplayCurrentImage();
        }
    }

    // Public method to refresh the image list
    public void RefreshImages()
    {
        LoadImages();

        // Reset to first image
        currentIndex = 0;

        // Show first image if available
        if (imagePaths.Count > 0)
        {
            DisplayCurrentImage();
        }
        else
        {
            displayImage.sprite = null; // Clear display if no images
        }
    }

    private void LoadImages()
    {
        string folderPath = Path.Combine(Application.dataPath, "imageGenAi");

        if (Directory.Exists(folderPath))
        {
            // Get all PNG files and sort by creation time (newest first)
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] files = dir.GetFiles("*.png");

            // Sort by creation time (newest first)
            System.Array.Sort(files, (a, b) => b.CreationTime.CompareTo(a.CreationTime));

            foreach (FileInfo file in files)
            {
                imagePaths.Add(file.FullName);
            }
        }
    }

    private void DisplayCurrentImage()
    {
        if (imagePaths.Count == 0) return;

        // Load image from file
        byte[] imageData = File.ReadAllBytes(imagePaths[currentIndex]);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        // Create sprite and display
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        displayImage.sprite = sprite;
        displayImage.preserveAspect = true;

        // Update button interactivity
        UpdateButtonStates();
    }

    public void ShowNextImage()
    {
        if (imagePaths.Count == 0) return;

        currentIndex = (currentIndex + 1) % imagePaths.Count;
        DisplayCurrentImage();
    }

    public void ShowPreviousImage()
    {
        if (imagePaths.Count == 0) return;

        currentIndex = (currentIndex - 1 + imagePaths.Count) % imagePaths.Count;
        DisplayCurrentImage();
    }

    private void UpdateButtonStates()
    {
        // Disable buttons if only one image exists
        bool multipleImages = imagePaths.Count > 1;
        nextButton.interactable = multipleImages;
        //backButton.interactable = multipleImages;
    }
}