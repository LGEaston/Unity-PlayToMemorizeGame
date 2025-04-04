using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCard : MonoBehaviour
{
    [SerializeField] private SceneController controller;
    [SerializeField] private GameObject Card_Back;

    public void OnMouseDown()
    {
        if (Card_Back.activeSelf && controller.canReveal)
        {
            Card_Back.SetActive(false);
            controller.CardReveal(this);
        }
    }

    // Randomize the Cards
    private int _id;
    public int id
    {
        get { return _id; }
    }

    public void ChangeSprite(int id, Sprite image)
    {
        _id = id;
        GetComponent<SpriteRenderer>().sprite = image; // Gets the Sprite Rendere Component and changes the property of the sprite
    }

    public void Unreveal()
    {
        Card_Back.SetActive(true); // Everytime this is called, it will cover the face of the cards again
    }
}