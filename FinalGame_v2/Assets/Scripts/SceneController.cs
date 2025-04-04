using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using UnityEngine.UI;

using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class SceneController : MonoBehaviour
{
    public const int gridRows = 2;
    public const int gridColumns = 4;
    public const float offsetX = 350;
    public const float offsetY = 420;

    [SerializeField] private MainCard originalCard;
    private Sprite[] images;

    private MainCard _firstReveal;
    private MainCard _secondReveal;

    private int _score = 0;
    [SerializeField] private TextMeshProUGUI scoreLabel;

    private float timer = 20f; // Timer in seconds
    [SerializeField] private TextMeshProUGUI timerLabel;

    // References to ImagePairGame and Canvas
    [SerializeField] private GameObject imagePairGame; // The ImagePairGame object
    [SerializeField] private GameObject canvas; // The Canvas object
    [SerializeField] private TextMeshProUGUI finalScoreText; // Text to display the final score
    [SerializeField] private TextMeshProUGUI finalTimerText; // Text to display the remaining time
    //[SerializeField] private Button restartButton; // Restart button

    private int totalPairs; // Total number of card pairs
    private int matchedPairs = 0; // Number of matched pairs
    private bool isGameCompleted = false; // Flag to track if the game is completed

    private string imageFolderPath;
    private List<string> imagePaths = new List<string>();
    private List<Sprite> loadedSprites = new List<Sprite>();

    private void Start()
    {
        // Initialize UI
        canvas.SetActive(false); // Hide the Canvas at the start

        // Check for required components
        if (originalCard == null)
        {
            Debug.LogError("Original card reference not set in inspector!");
            return;
        }

        if (imagePairGame == null)
        {
            Debug.LogError("ImagePairGame reference not set in inspector!");
            return;
        }

        // Initialize UI
        if (canvas != null)
        {
            canvas.SetActive(false);
        }

        imageFolderPath = Path.Combine(Application.dataPath, "imageGenAi");

        // Load images from the folder
        LoadImagesFromFolder();

        // Check if we have enough images (CHANGED THIS LINE)
        if (loadedSprites.Count < gridRows * gridColumns / 2)
        {
            Debug.LogError($"Not enough images in folder. Need {gridRows * gridColumns / 2} but found {loadedSprites.Count}");
            return;
        }

        Vector3 startPosition = originalCard.transform.position; // Position of the 1st card. All other cards are offset from here

        // We need exactly 4 unique images (since grid is 2x4 = 8 cards = 4 pairs)
        List<Sprite> selectedSprites = SelectSpritesForGame();

        // Generate card IDs and shuffle them
        //int[] numbers = GenerateCardIds();

        // Generate card IDs ensuring each sprite has exactly two cards
        int[] numbers = GeneratePairedCardIds(selectedSprites.Count);
        numbers = ShuffleArray(numbers);

        for (int i = 0; i < gridColumns; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                MainCard card; // Create an instance of card

                if (i == 0 && j == 0)
                {
                    card = originalCard;
                }
                else
                {
                    //card = Instantiate(originalCard) as MainCard;
                    card = Instantiate(originalCard, imagePairGame.transform) as MainCard; // Parent to ImagePairGame
                }

                int index = j * gridColumns + i;
                int id = numbers[index];
                //card.ChangeSprite(id, images[id]);

                // Get a random sprite with weighted probability
                //Sprite randomSprite = GetWeightedRandomSprite();
                //card.ChangeSprite(id, randomSprite);

                Sprite sprite = selectedSprites[id];  // CHANGED THIS LINE
                card.ChangeSprite(id, sprite);

                float posX = (offsetX * i) + startPosition.x;
                float posY = (offsetY * j) + startPosition.y;
                card.transform.position = new Vector3(posX, posY, startPosition.z);
            }
        }

        // ADD DEBUG LOGS RIGHT HERE:
        Debug.Log("=== CARD ASSIGNMENTS ===");
        for (int i = 0; i < numbers.Length; i++)
        {
            int row = i / gridColumns;
            int col = i % gridColumns;
            Debug.Log($"Card at ({row},{col}) has ID: {numbers[i]} with sprite: {selectedSprites[numbers[i]].name}");
        }

        // Calculate total pairs
        totalPairs = (gridRows * gridColumns) / 2;

        // Set up the restart button
        //restartButton.onClick.AddListener(Restart);
    }

    private void LoadImagesFromFolder()
    {
        // Clear previous data
        imagePaths.Clear();
        loadedSprites.Clear();

        // Get all PNG files from the folder
        if (Directory.Exists(imageFolderPath))
        {
            imagePaths.AddRange(Directory.GetFiles(imageFolderPath, "*.png"));
            imagePaths.Sort(); // This sorts alphabetically, which works with your naming scheme
        }

        // Load each image as a Sprite
        foreach (string path in imagePaths)
        {
            Texture2D texture = LoadTextureFromFile(path);
            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                loadedSprites.Add(sprite);
            }
        }
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        return texture.LoadImage(fileData) ? texture : null;
    }

    private Sprite GetWeightedRandomSprite()
    {
        if (loadedSprites.Count == 0) return null;

        // Create weights - newer images (higher indices) get higher weights
        float[] weights = new float[loadedSprites.Count];
        float totalWeight = 0f;

        // Linear weighting - last image gets highest weight
        for (int i = 0; i < loadedSprites.Count; i++)
        {
            weights[i] = (i + 1) * 1f; // Simple linear weighting
            totalWeight += weights[i];
        }

        // Random selection with weights
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float weightSum = 0f;

        for (int i = 0; i < loadedSprites.Count; i++)
        {
            weightSum += weights[i];
            if (randomValue <= weightSum)
            {
                return loadedSprites[i];
            }
        }

        return loadedSprites[loadedSprites.Count - 1]; // Fallback to last image
    }

    private List<Sprite> SelectSpritesForGame()
    {
        // We need 4 unique sprites (for 4 pairs)
        int requiredSprites = gridRows * gridColumns / 2;
        List<Sprite> selected = new List<Sprite>();

        // First try to get the newest images with weighting
        while (selected.Count < requiredSprites && loadedSprites.Count > 0)
        {
            Sprite sprite = GetWeightedRandomSprite();
            if (!selected.Contains(sprite))
            {
                selected.Add(sprite);
            }

            // Fallback if we can't get enough unique sprites
            if (selected.Count < requiredSprites && selected.Count == loadedSprites.Count)
            {
                Debug.LogWarning("Not enough unique sprites, some will be repeated");
                break;
            }
        }

        return selected;
    }

    private int[] GeneratePairedCardIds(int uniqueSpriteCount)
    {
        int[] ids = new int[gridRows * gridColumns];
        int pairCount = 0;

        // Assign each sprite to exactly two cards
        for (int i = 0; i < ids.Length; i += 2)
        {
            int spriteId = pairCount % uniqueSpriteCount;
            ids[i] = spriteId;
            ids[i + 1] = spriteId;
            pairCount++;
        }

        return ids;
    }

    private void Update()
    {
        if (timer > 0 && !isGameCompleted) // Only update the timer if the game is not completed
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else if (!isGameCompleted) // Only restart if the game is not completed
        {
            // Ensure the timer doesn't go negative
            timer = 0;
            UpdateTimerDisplay(); // Update the display one last time to show 0:00
            Restart();
        }
    }

    private string GetFormattedTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(Mathf.Max(0, timeInSeconds) / 60);
        int seconds = Mathf.FloorToInt(Mathf.Max(0, timeInSeconds) % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private void UpdateTimerDisplay()
    {
        timerLabel.text = "Timer: " + GetFormattedTime(timer);
    }

    //private void UpdateTimerDisplay()
    //{
    //    // Ensure the timer doesn't go below 0
    //    int minutes = Mathf.FloorToInt(Mathf.Max(0, timer) / 60);
    //    int seconds = Mathf.FloorToInt(Mathf.Max(0, timer) % 60);
    //    timerLabel.text = string.Format("Timer: {0:00}:{1:00}", minutes, seconds);
    //}

    //private int[] GenerateCardIds()
    //{
    //    int[] ids = new int[gridRows * gridColumns];
    //    for (int i = 0; i < ids.Length; i++)
    //    {
    //        //ids[i] = i % (images.Length);
    //        ids[i] = i % (loadedSprites.Count); // Use loadedSprites.Count instead of images.Length
    //    }
    //    return ids;
    //}

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int temp = newArray[i];
            int r = UnityEngine.Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = temp;
        }
        return newArray;
    }

    public bool canReveal
    {
        get { return _secondReveal == null; }
    }

    public void CardReveal(MainCard card)
    {
        if (_firstReveal == null)
        {
            _firstReveal = card;
        }
        else
        {
            _secondReveal = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        Debug.Log($"Checking match between {_firstReveal.id} and {_secondReveal.id}");
        if (_firstReveal.id == _secondReveal.id)
        {
            _score++;
            scoreLabel.text = "Score: " + _score;

            // Increment matchedPairs when a match is found
            matchedPairs++;

            // Debug: Check if all pairs are matched
            Debug.Log($"Matched Pairs: {matchedPairs}, Total Pairs: {totalPairs}");

            // Check if all pairs are matched
            if (matchedPairs == totalPairs)
            {
                Debug.Log("All pairs matched. Showing popup.");
                ShowPopup();
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            // Wrong match, cover the cards again
            _firstReveal.Unreveal();
            _secondReveal.Unreveal();
        }

        _firstReveal = null;
        _secondReveal = null;
    }

    private void ShowPopup()
    {
        Debug.Log("ShowPopup called.");

        // FIRST capture the remaining time before modifying the timer
        float remainingTime = timer;

        // Stop the timer and mark the game as completed
        timer = 0;
        isGameCompleted = true;

        // Debug: Check if ImagePairGame is being hidden
        if (imagePairGame != null)
        {
            Debug.Log("Hiding ImagePairGame.");
            imagePairGame.SetActive(false);
        }
        else
        {
            Debug.LogError("ImagePairGame is not assigned.");
        }

        // Debug: Check if Canvas is being unhidden
        if (canvas != null)
        {
            Debug.Log("Unhiding Canvas.");
            canvas.SetActive(true);
        }
        else
        {
            Debug.LogError("Canvas is not assigned.");
        }

        // Update the pop-up text
        if (finalScoreText != null && finalTimerText != null)
        {
            Debug.Log("Updating pop-up text.");
            //finalScoreText.text = "Final Score: " + _score;
            //finalTimerText.text = "Time Left: " + timerLabel.text;
            finalScoreText.text = "Final Score: " + _score;
            finalTimerText.text = "Time Left: " + GetFormattedTime(remainingTime);
        }
        else
        {
            Debug.LogError("FinalScoreText or FinalTimerText is not assigned.");
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("MatchPairImgGame");
    }
}