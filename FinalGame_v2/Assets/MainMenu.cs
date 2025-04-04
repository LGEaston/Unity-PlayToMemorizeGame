using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Import library to change scenes in Unity
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuizGameMenu()
    {
        // Either switch scene by adding the scenes to the File > Build Settings; this will assigning an idex to each scene
        // Then calling the scenes by current scene index + 1
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // Or calling the scenes using the Scene's Name
        SceneManager.LoadScene("QuizGameMenu");
        
    }

    // Loading "Start Quiz Menu" in the [ Play Quiz Menu ]
    public void StartQuizGame()
    {
        // Or calling the scenes using the Scene's Name
        SceneManager.LoadScene("StartQuizGame");
    }

    // Menu to input quiz questions and import the image associated with the inserted input
    public void AddQuizQuestions()
    {
        // Or calling the scenes using the Scene's Name
        SceneManager.LoadScene("AddQuizQuestions");
    }

    // Match Card Game Navigations
    public void MatchCardGameMenu()
    {
        // Or calling the scenes using the Scene's Name
        SceneManager.LoadScene("MatchCardGameMenu");
    }

    public void ImageGeneratorMenu()
    {
        // Or calling the scenes using the Scene's Name
        SceneManager.LoadScene("ImageGeneratorMenu");
    }

    public void MatchPairImgGame()
    {
        // Or calling the scenes using the Scene's Name
        SceneManager.LoadScene("MatchPairImgGame");
    }

    public void ReturnMainMenu()
         
    {
        // Or calling the scenes using the Scene's Name
        SceneManager.LoadScene("MainGameMenu");
    }

    //public void ReturnMatchCardMenu()
    //{
    //    // Or calling the scenes using the Scene's Name
    //    SceneManager.LoadScene("MatchCardGameMenu");
    //}



    //public void DisplayOptions()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    //}

    public void ExitGame()
    {
        Debug.Log("EXIT");
        Application.Quit();
    }
}
