//Better Refined prompt and Name Extractations
// But no AI input prompts repromts.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
// Used to call the button component
using UnityEngine.UI;
using Microsoft.Extensions.AI;
//using NUnit.Framework.Internal;
using TMPro;

using System.Net.Http;
using System.IO;
using System.Text;
using Unity.VisualScripting;
//using System.Diagnostics;

public class OllamaHandler : MonoBehaviour
{
    public TMP_InputField InputText; // Assign to InputField (TMP)
    public TMP_Text OutputText; // Assign to Text (TMP) inside Scroll View's Content
    public Button ImportImageButton; // Assign to the new ImportImageButton

    public Image displayImage; // Assign the Image component for display in the Inspector

    private List<UserData> userDataList = new List<UserData>(); // List to store all user data
    private string imageFileName = ""; // To store the imported image file name

    private string tempImagePath = ""; // Temporarily store the image path before saving

    void Start()
    {
        // Load existing data when the game starts
        LoadFromJson();

        // Add listener to the ImportImageButton
        //ImportImageButton.onClick.AddListener(OnImportImageButtonClicked);
    }

    public void Run()
    {
        ProcessTheConversation();
    }

    public void ProcessTheConversation()
    {
        // Append the user's input to the predefined string
        //string prompt = $"Give me 3 names similar to {InputText.text} in this format: Name1_Name2_Name3";
        //string prompt = $"Provide exactly 3 names similar to {InputText.text}, separated by underscores, in this exact format: Name1_Name2_Name3. Do not include any additional text, explanations, or formatting.";
        string prompt = $"Provide exactly 3 unique names similar to {InputText.text}, separated by underscores, in this exact format: Name1_Name2_Name3. Do not include any additional text, explanations, or formatting. Do not repeat the input name or use variations like {InputText.text}1, {InputText.text}2, {InputText.text}3.";

        Debug.Log("Generated prompt: " + prompt);

        // Create an instance of the chat client
        IChatClient chatClient = new OllamaChatClient(new Uri("http://localhost:11434/api/generate"), "openchat");
        Debug.Log("Running...");

        // Call the synchronous method with the modified prompt
        string response = chatClient.Complete(prompt);
        Debug.Log("Done!");

        // Process the response to extract the names
        string[] names = ExtractNamesFromResponse(response);

        if (names.Length == 3) // Only save the image if 3 valid names were generated
        {
            if (!string.IsNullOrEmpty(tempImagePath)) // Ensure an image was selected
            {
                string folderPath = Application.dataPath + "/ImportedImages";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Path.GetFileNameWithoutExtension(tempImagePath) + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(tempImagePath);
                string destPath = Path.Combine(folderPath, fileName);
                File.Copy(tempImagePath, destPath, true);

                imageFileName = fileName; // Assign the saved image name
                Debug.Log("Image saved to: " + destPath);

                #if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
                #endif
            }

            SaveUserData(InputText.text, names, imageFileName);
        }
        else
        {
            Debug.LogError("AI did not return 3 valid names. Image will not be saved.");
        }

        // Save the input and names
        // SaveUserData(InputText.text, names, imageFileName);

        // Update the output text (optional)
        OutputText.text = response;
    }

    public void OnImportImageButtonClicked()
    {
        // Open file dialog to import image
        string path = UnityEditor.EditorUtility.OpenFilePanel("Import Image", "", "png,jpg,jpeg");
        if (path.Length != 0)
        {
            // Load the image as a texture
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            // Apply the texture to the UI Image component
            displayImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // Store the path but do NOT save it yet
            tempImagePath = path;
            Debug.Log("Image selected but not saved yet: " + tempImagePath);

            //// Create the ImportedImages folder if it doesn't exist
            //string folderPath = Application.dataPath + "/ImportedImages";
            //if (!Directory.Exists(folderPath))
            //{
            //    Directory.CreateDirectory(folderPath);
            //}

            //// Copy the imported image to the ImportedImages folder
            ////string fileName = Path.GetFileName(path);
            //string fileName = Path.GetFileNameWithoutExtension(path) + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(path);
            //string destPath = Path.Combine(folderPath, fileName);
            //File.Copy(path, destPath, true);

            //// Save the image file name
            //imageFileName = fileName;

            //Debug.Log("Image imported and saved to: " + destPath);

            //// Force Unity to refresh assets in the Editor (only applies while running in Unity Editor)
            //#if UNITY_EDITOR
            //    UnityEditor.AssetDatabase.Refresh();
            //#endif
        }
    }

    private string[] ExtractNamesFromResponse(string response)
    {
        Debug.Log("Original response: " + response); // Test1

        // Remove unwanted text (e.g., explanations, newlines)
        response = response.Replace("\n", ""); // Remove newlines
        response = response.Replace("Here are three names similar to", ""); // Remove common prefix
        response = response.Replace("formatted as requested:", ""); // Remove common suffix

        Debug.Log("Cleaned response: " + response); // Test2

        // Find the part of the response that matches the expected format (Name1_Name2_Name3)
        int underscoreIndex = response.IndexOf('_');
        if (underscoreIndex == -1)
        {
            Debug.LogError("Invalid response format: No underscores found.");
            return new string[0];
        }

        // Extract the substring containing the names
        string namesString = response.Substring(0); // Start from the beginning of the response
        namesString = namesString.Split('.')[0]; // Remove any trailing text after the names

        Debug.Log("Names string: " + namesString); // Test3

        // Split the names by '_'
        string[] names = namesString.Split('_');

        // Trim any extra spaces or unwanted characters
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = names[i].Trim();
        }

        Debug.Log("Extracted names: " + string.Join(", ", names)); // Test4

        // Ensure we have exactly 3 names
        if (names.Length != 3)
        {
            Debug.LogError("Invalid response format: Expected 3 names separated by '_'.");
            return new string[0];
        }

        return names;
    }

    private void SaveUserData(string input, string[] names, string imageFileName)
    {
        // Create a new UserData object
        UserData userData = new UserData(input, names, imageFileName);

        // Add it to the list
        userDataList.Add(userData);

        // Save the list to a JSON file
        SaveToJson();
    }

    private void SaveToJson()
    {
        // Convert the list to JSON
        string json = JsonUtility.ToJson(new UserDataWrapper(userDataList), true);

        // Save the JSON to a file
        string filePath = Application.persistentDataPath + "/userData.json";
        System.IO.File.WriteAllText(filePath, json);

        Debug.Log("Data saved to: " + filePath);
    }

    private void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/userData.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            UserDataWrapper wrapper = JsonUtility.FromJson<UserDataWrapper>(json);
            userDataList = wrapper.userDataList;
            Debug.Log("Data loaded successfully.");
        }
        else
        {
            Debug.LogWarning("No data file found. Initializing empty list.");
            userDataList = new List<UserData>();
        }
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

    [System.Serializable]
    private class UserDataWrapper
    {
        public List<UserData> userDataList;

        public UserDataWrapper(List<UserData> list)
        {
            userDataList = list;
        }
    }
}

public interface IChatClient
{
    string Complete(string input); // Synchronous method
}

public class OllamaChatClient : IChatClient
{
    private readonly Uri _endpoint;
    private readonly string _model;

    public OllamaChatClient(Uri endpoint, string model)
    {
        _endpoint = endpoint;
        _model = model;
    }

    public string Complete(string input)
    {
        using (var client = new HttpClient())
        {
            var content = new StringContent($"{{\"model\": \"{_model}\", \"prompt\": \"{input}\"}}", System.Text.Encoding.UTF8, "application/json");
            Debug.Log("Sending request to: " + _endpoint);
            Debug.Log("Request body: " + $"{{\"model\": \"{_model}\", \"prompt\": \"{input}\"}}");

            var response = client.PostAsync(_endpoint, content).Result; // Synchronously wait for the response
            var responseStream = response.Content.ReadAsStreamAsync().Result;

            using (var reader = new StreamReader(responseStream))
            {
                string line;
                StringBuilder fullResponse = new StringBuilder();

                while ((line = reader.ReadLine()) != null)
                {
                    Debug.Log("Received chunk: " + line);

                    // Parse the JSON chunk
                    var jsonResponse = JsonUtility.FromJson<OllamaResponse>(line);
                    fullResponse.Append(jsonResponse.response);

                    if (jsonResponse.done)
                    {
                        Debug.Log("Response complete: " + fullResponse.ToString());
                        return fullResponse.ToString();
                    }
                }
            }

            return "Error: No response received.";
        }
    }

    [System.Serializable]
    public class OllamaResponse
    {
        public string model;
        public string created_at;
        public string response;
        public bool done;
        public string done_reason;
        public long total_duration;
        public long load_duration;
        public int prompt_eval_count;
        public long prompt_eval_duration;
        public int eval_count;
        public long eval_duration;
    }
}