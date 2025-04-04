using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using static OllamaHandler;
using System.IO;

// Import library to change scenes in Unity
//using UnityEngine.SceneManagement;

public class QuizGame : MonoBehaviour
{
    public TMP_Text scoreText; // Assign to ScoreText (TMP)

    public Image imageDisplay; // Assign to the Image UI component in the Inspector

    //public TMP_Text inputTextDisplay; // Assign to Text (TMP) inside InputTextDisplay (Button)
    public TMP_Text[] nameTextDisplays; // Assign to Text (TMP) inside NameDisplay1, NameDisplay2, NameDisplay3 (Buttons)
    public Button nextButton; // Assign to NextButton (Button)

    private List<UserData> userDataList;
    private int currentIndex = 0;
    private int score = 0;

    //public void ReturnQuizGameMenu()

    //{
    //    // Or calling the scenes using the Scene's Name
    //    SceneManager.LoadScene("QuizGameMenu");
    //}

    void OnEnable()
    {
        Debug.Log("ViewDataMenu enabled.");

        // Load the JSON file
        LoadFromJson();

        // Debug: Log the number of entries in userDataList
        Debug.Log($"Number of entries in userDataList: {userDataList.Count}");

        // Debug: Log the currentIndex
        Debug.Log($"Current index: {currentIndex}");

        // Display the first set of data
        DisplayCurrentData();

        // Add listeners to buttons
        nextButton.onClick.AddListener(OnNextButtonClicked);
        foreach (var button in nameTextDisplays)
        {
            button.GetComponentInParent<Button>().onClick.AddListener(() => OnAnswerClicked(button));
        }
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
        if (userDataList == null || userDataList.Count == 0)
        {
            Debug.LogError("No data available to display.");
            return;
        }

        // Ensure currentIndex is within bounds
        if (currentIndex < 0 || currentIndex >= userDataList.Count)
        {
            Debug.LogError($"Current index {currentIndex} is out of range. Resetting to 0.");
            currentIndex = 0;
        }

        UserData currentData = userDataList[currentIndex];

        // Create a list of all possible answers
        List<string> answers = new List<string>(currentData.generatedNames);

        // Add the correct answer only if it's not already in the list
        if (!answers.Contains(currentData.inputText))
        {
            answers.Add(currentData.inputText);
        }

        Debug.Log("Answers before shuffling: " + string.Join(", ", answers));

        // Shuffle the answers
        answers = ShuffleList(answers);

        Debug.Log("Answers after shuffling: " + string.Join(", ", answers));

        // Ensure we don't exceed the number of buttons
        int numButtons = nameTextDisplays.Length;
        if (answers.Count > numButtons)
        {
            Debug.LogWarning($"More answers ({answers.Count}) than buttons ({numButtons}). Truncating answers.");
            answers = answers.GetRange(0, numButtons); // Truncate the list to match the number of buttons
        }

        // Assign the shuffled answers to the buttons
        for (int i = 0; i < nameTextDisplays.Length; i++)
        {
            if (i < answers.Count)
            {
                nameTextDisplays[i].text = answers[i];
                Debug.Log($"Button {i + 1} text: {nameTextDisplays[i].text}");
            }
            else
            {
                nameTextDisplays[i].text = ""; // Clear the button text if there are fewer answers than buttons
            }
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

    //private void DisplayCurrentData()
    //{
    //    if (userDataList == null || userDataList.Count == 0)
    //    {
    //        Debug.LogError("No data available to display.");
    //        return;
    //    }

    //    if (currentIndex < userDataList.Count)
    //    {
    //        UserData currentData = userDataList[currentIndex];

    //        // Display the input text
    //        //inputTextDisplay.text = currentData.inputText;

    //        // Create a list of all possible answers
    //        List<string> answers = new List<string>(currentData.generatedNames);

    //        // Add the correct answer only if it's not already in the list
    //        if (!answers.Contains(currentData.inputText))
    //        {
    //            answers.Add(currentData.inputText);
    //        }

    //        Debug.Log("Answers before shuffling: " + string.Join(", ", answers));

    //        // Shuffle the answers
    //        answers = ShuffleList(answers);

    //        Debug.Log("Answers after shuffling: " + string.Join(", ", answers));

    //        // Assign the shuffled answers to the buttons
    //        for (int i = 0; i < nameTextDisplays.Length; i++)
    //        {
    //            nameTextDisplays[i].text = answers[i];
    //            Debug.Log($"Button {i + 1} text: {nameTextDisplays[i].text}");
    //        }

    //        // Load and display the image
    //        if (!string.IsNullOrEmpty(currentData.imageFileName))
    //        {
    //            string imagePath = Path.Combine(Application.dataPath, "ImportedImages", currentData.imageFileName);
    //            if (File.Exists(imagePath))
    //            {
    //                // Load the image as a Texture2D
    //                byte[] imageData = File.ReadAllBytes(imagePath);
    //                Texture2D texture = new Texture2D(2, 2);
    //                texture.LoadImage(imageData);

    //                // Convert the Texture2D to a Sprite
    //                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

    //                // Assign the Sprite to the Image component
    //                imageDisplay.sprite = sprite;
    //                Debug.Log("Image displayed: " + currentData.imageFileName);
    //            }
    //            else
    //            {
    //                Debug.LogError("Image file not found: " + imagePath);
    //                imageDisplay.sprite = null; // Clear the image if the file is not found
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogWarning("No image file name found for current data.");
    //            imageDisplay.sprite = null; // Clear the image if no file name is available
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("No more data to display.");
    //    }
    //}

    //private void DisplayCurrentData()
    //{
    //    if (currentIndex < userDataList.Count)
    //    {
    //        UserData currentData = userDataList[currentIndex];

    //        // Display the input text
    //        inputTextDisplay.text = currentData.inputText;

    //        // Create a list of all possible answers
    //        List<string> answers = new List<string>(currentData.generatedNames);
    //        answers.Add(currentData.inputText); // Add the correct answer

    //        // Shuffle the answers
    //        answers = ShuffleList(answers);

    //        // Assign the shuffled answers to the buttons
    //        for (int i = 0; i < nameTextDisplays.Length; i++)
    //        {
    //            nameTextDisplays[i].text = answers[i];
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("No more data to display.");
    //    }
    //}

    private List<string> ShuffleList(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    private void OnAnswerClicked(TMP_Text clickedText)
    {
        if (clickedText.text == userDataList[currentIndex].inputText)
        {
            score++;
            scoreText.text = "SCORE: " + score;
        }

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