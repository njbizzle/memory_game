using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Card : ScriptableObject
{
    public Sprite itemSprite;
    public string itemName;

    CardManager cardManager;
    Sprite currentImage;

    void Start()
    {
        cardManager = FindObjectOfType<CardManager>();

        currentImage = cardManager.defaultImage;
        swapToItem();
    }

    public void swapToItem(){
        currentImage = itemSprite;
    }

    public void swapToDefault(){
        currentImage = cardManager.defaultImage;
    }

    
}
