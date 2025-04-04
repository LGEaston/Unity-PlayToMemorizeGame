using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Debug = UnityEngine.Debug;

using UnityEngine.UI; // To use the Image 

public class ImageGenerationManager : MonoBehaviour
{

    string myPersistentDataPath;

    public TMP_InputField promptText;

    //public Image GeneratedImageDislay; // Assign the Image component for display in the Inspector

    public void GenerateImage()
    {
        if (promptText.text == "" || promptText.text == "Please enter a prompt")
        {
            promptText.text = "Please enter a prompt";
        }

        else
        {
            StopAllCoroutines(); // Ensure previous coroutine isn't running
            StartCoroutine(MakeRequest());
        }
    }

    private void Awake()
    {
        myPersistentDataPath = Application.persistentDataPath + "\\";
        Debug.Log(myPersistentDataPath);
    }


    IEnumerator MakeRequest()
    {
        #region promptPayloadExample

        #endregion

        //other methods
        //string json =($"{{\"prompt\": \"{promptText.text}\"}}");
        //string json1 = @"{""prompt"": ""girafe""}"; 


        string paramKey_prompt = "\"prompt\":";
        string paramValue_prompt = "\"" + promptText.text + "\"";


        //string json = "{" + paramKey_prompt + paramValue_prompt + "," + paramKey_width + paramValue_width +  "}";

        string json = "{" + paramKey_prompt + paramValue_prompt + "}";


        Debug.Log(json);

        var jsonBytes = Encoding.UTF8.GetBytes(json);

        var www = new UnityWebRequest("http://127.0.0.1:7860/sdapi/v1/txt2img", "POST");

        www.uploadHandler = new UploadHandlerRaw(jsonBytes);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", " text/plain");

        yield return www.SendWebRequest();
        //if (www.isNetworkError || www.isHttpError)
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string imageData = www.downloadHandler.text;

            Debug.Log("Response JSON: " + imageData); // Log the full response

            ImageData myImageData = JsonConvert.DeserializeObject<ImageData>(imageData);

            // Add this check immediately after deserialization ------------------
            if (myImageData.images == null || myImageData.images.Count == 0)
            {
                Debug.LogError("No image received from Stable Diffusion.");
                yield break; // Exit the coroutine if no image was returned
            }

            // Then, check if restore_faces or tiling are null and log accordingly
            Debug.Log("Restore Faces: " + myImageData.parameters.restore_faces);
            Debug.Log("Tiling: " + myImageData.parameters.tiling);

            // HERE!
            if (myImageData.parameters.restore_faces == null)
            {
                myImageData.parameters.restore_faces = false;  // Set to default false
            }

            // Here
            if (myImageData.parameters.tiling == null)
            {
                myImageData.parameters.tiling = false;  // Set default value to false
            }

            Debug.Log(myImageData.images[0]);


            // Define the folder path within the Unity Assets directory
            string folderPath = Path.Combine(Application.dataPath, "imageGenAi");

            // Create the folder if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Generate a new file name for the image
            //string newImageFileNumber = GetNextImageNumberForFileName(folderPath, true);
            //Debug.Log(newImageFileNumber);

            //// Construct the full file name
            //string newImageFileName = "image_" + newImageFileNumber + ".png";

            string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string newImageFileName = $"image_{timestamp}.png";
            Debug.Log(newImageFileName);

            // Save the image to the specified folder
            string fullPath = Path.Combine(folderPath, newImageFileName);
            File.WriteAllBytes(fullPath, Convert.FromBase64String(myImageData.images[0]));

            //string newImageFileNumber = GetNextImageNumberForFileName(true);
            //Debug.Log(newImageFileNumber);

            //string newImageFileName = "image_" + newImageFileNumber + ".png";


            //File.WriteAllBytes(Path.Combine(Application.persistentDataPath, newImageFileName), Convert.FromBase64String(myImageData.images[0]));
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            Debug.Log($"Image saved to: {fullPath}");

            www.Dispose();


            //Process.Start(myPersistentDataPath);

            // Refresh the Unity Asset Database to make the new image visible in the Editor
            //UnityEditor.AssetDatabase.Refresh();


        }

    }

    string GetNextImageNumberForFileName(string folderPath, bool isNewImageFile)
    {
        List<int> imageNumbers = new List<int>();
        int maxImageNumber = 0;

        // Get all files in the specified folder
        string[] files = Directory.GetFiles(folderPath);

        // Extract image numbers from existing files
        foreach (string file in files)
        {
            FileInfo fi = new FileInfo(file);
            string justFileName = fi.Name;
            string extn = fi.Extension;

            if (extn == ".png" && justFileName.StartsWith("image_"))
            {
                string fileNumberString = justFileName.Substring(6, 2); // Assumes file names are in the format "image_XX.png"
                if (int.TryParse(fileNumberString, out int fileNumber))
                {
                    imageNumbers.Add(fileNumber);
                }
            }
        }

        // Determine the next file number
        string strMaxImageNumber;
        if (imageNumbers.Count > 0)
        {
            maxImageNumber = imageNumbers.Max();
            if (isNewImageFile) // If this is a new image file, increment the max number
            {
                maxImageNumber++;
            }

            strMaxImageNumber = maxImageNumber.ToString("D2"); // Ensure two-digit format (e.g., "01", "02")
        }
        else
        {
            strMaxImageNumber = "01"; // Default to "01" if no images exist
        }

        return strMaxImageNumber;
    }
}


public class Parameters
{
    public bool enable_hr { get; set; }
    public int? denoising_strength { get; set; } = 0; // Default value
    public int? firstphase_width { get; set; } = 0; // Default value
    public int? firstphase_height { get; set; } = 0; // Default value
    public string prompt { get; set; }
    public object styles { get; set; }
    public int? seed { get; set; } = -1; // Default value
    public int? subseed { get; set; } = -1; // Default value
    public int? subseed_strength { get; set; } = 0; // Default value
    public int? seed_resize_from_h { get; set; } = -1; // Default value
    public int? seed_resize_from_w { get; set; } = -1; // Default value
    public object sampler_name { get; set; }
    public int? batch_size { get; set; } = 1; // Default value
    public int? n_iter { get; set; } = 1; // Default value
    public int? steps { get; set; } = 50; // Default value
    public double? cfg_scale { get; set; } = 7.0; // Default value
    public int? width { get; set; } = 512; // Default value
    public int? height { get; set; } = 512; // Default value
    public bool? restore_faces { get; set; } = false; // Default value
    public bool? tiling { get; set; } = false; // Default value
    public object negative_prompt { get; set; }
    public object eta { get; set; }
    public double? s_churn { get; set; } = 0; // Default value
    public object s_tmax { get; set; }
    public double? s_tmin { get; set; } = 0; // Default value
    public double? s_noise { get; set; } = 1; // Default value
    public object override_settings { get; set; }
    public string sampler_index { get; set; } = "Euler"; // Default value
}


public class ImageData
{
    public List<string> images { get; set; }
    public Parameters parameters { get; set; }
    public string info { get; set; }
}