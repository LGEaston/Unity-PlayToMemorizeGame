using System.Collections.Generic;
using UnityEngine.UI; // For Button class
using UnityEngine;
using TMPro;
using static OllamaHandler;
using System.IO;

public class ViewDataMenu : MonoBehaviour
{
    public Image imageDisplay; // Assign to the Image UI component in the Inspector

    public TMP_Text inputTextDisplay; // Assign to Text (TMP) inside InputTextDisplay (Button)
    public TMP_Text[] nameTextDisplays; // Assign to Text (TMP) inside NameDisplay1, NameDisplay2, NameDisplay3 (Buttons)
    public Button nextButton; // Assign to NextButton (Button)
    //public Button backButton; // Assign to BackButton (Button)

    private List<UserData> userDataList;
    private int currentIndex = 0;

    //void Start()
    //Here the OnEnabled() is used instead of Start() so that the ViewDataMenu script gets updated
    // to reload the JSON file whenever the ViewDataMenu GameObject is enabled
    void OnEnable()
    {
        Debug.Log("ViewDataMenu enabled.");

        // Load the JSON file
        LoadFromJson();

        // Display the first set of data
        DisplayCurrentData();

        // Add listeners to buttons
        nextButton.onClick.AddListener(OnNextButtonClicked);
        //backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/userData.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            Debug.Log("JSON file content: " + json); // Add this line
            UserDataWrapper wrapper = JsonUtility.FromJson<UserDataWrapper>(json);
            userDataList = wrapper.userDataList;
            Debug.Log("Data loaded successfully.");
        }
        else
        {
            Debug.LogError("No data file found. Initializing empty list.");
            userDataList = new List<UserData>();
        }
    }

    private void DisplayCurrentData()
    {
        if (currentIndex < userDataList.Count)
        {
            UserData currentData = userDataList[currentIndex];

            // Display the input text
            inputTextDisplay.text = currentData.inputText;

            // Display the generated names
            for (int i = 0; i < currentData.generatedNames.Length; i++)
            {
                nameTextDisplays[i].text = currentData.generatedNames[i];
            }
            // Load and display the image
            if (!string.IsNullOrEmpty(currentData.imageFileName))
            {
                string imagePath = Path.Combine(Application.dataPath, "ImportedImages", currentData.imageFileName);
                if (File.Exists(imagePath))
                {
                    // Load the image as a Texture2D
                    byte[] imageData = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);

                    // Convert the Texture2D to a Sprite
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

                    // Assign the Sprite to the Image component
                    imageDisplay.sprite = sprite;
                    Debug.Log("Image displayed: " + currentData.imageFileName);
                }
                else
                {
                    Debug.LogError("Image file not found: " + imagePath);
                    imageDisplay.sprite = null; // Clear the image if the file is not found
                }
            }
            else
            {
                Debug.LogWarning("No image file name found for current data.");
                imageDisplay.sprite = null; // Clear the image if no file name is available
            }
        }
        else
        {
            Debug.Log("No more data to display.");
        }
    }

    private void OnNextButtonClicked()
    {
        // Move to the next set of data
        currentIndex++;

        // If we reach the end, loop back to the start
        if (currentIndex >= userDataList.Count)
        {
            currentIndex = 0;
        }

        // Display the new data
        DisplayCurrentData();
    }


    //private void OnBackButtonClicked()
    //{
    //    // Hide the ViewDataMenu
    //    gameObject.SetActive(false);

    //    // Show the UserInputMenu
    //    GameObject userInputMenu = GameObject.Find("UserInputMenu"); // Replace with the correct name
    //    if (userInputMenu != null)
    //    {
    //        userInputMenu.SetActive(true);
    //    }
    //}

    [System.Serializable]
    private class UserDataWrapper
    {
        public List<UserData> userDataList;
    }

    [System.Serializable]
    public class UserData
    {
        public string inputText;
        public string[] generatedNames;
        public string imageFileName;

        public UserData(string input, string[] names, string imageFileName)
        {
            inputText = input;
            generatedNames = names;
            this.imageFileName = imageFileName;
        }
    }
}