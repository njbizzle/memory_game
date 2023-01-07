using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class Card : ScriptableObject
{
    public Sprite itemSprite;
    public string itemName;

    public Vector3 targetPosition;
    public float moveSpeed;

    Sprite defaultSprite;
    Vector3 cardSize;

    // references and components
    public GameObject cardObject;
    CardManager cardManager;
    
    SpriteRenderer bgRenderer;
    SpriteRenderer itemRenderer;

    Transform cardTransform;
    BoxCollider2D cardHitbox;

    // swap animation
    string scaleAnimation;
    Sprite swapSprite;
    float swapSpeed;

    public void SetUp(GameObject cardObject_, Vector3 cardSize_, float swapSpeed_){
        // basic setup
        cardManager = FindObjectOfType<CardManager>();
        defaultSprite = cardManager.defaultImage;
        swapSpeed = swapSpeed_;

        // gameobject setup
        cardObject = cardObject_;

        cardTransform = cardObject.GetComponent<Transform>();
        cardHitbox = cardObject.GetComponent<BoxCollider2D>();

        bgRenderer = cardObject.transform.Find("CardBackground").GetComponent<SpriteRenderer>();
        itemRenderer = cardObject.transform.Find("CardImage").GetComponent<SpriteRenderer>();

        cardSize = cardSize_;
        cardTransform.localScale = cardSize;

        itemRenderer.sprite = defaultSprite;
    }

    public void cardUpdate(Vector2 mousePos, bool mouseClick){
        if (cardHitbox.OverlapPoint(mousePos) && mouseClick){
            swapFace();
        }

        // animation
        if (scaleAnimation == "shrink")
        {
            cardTransform.localScale -= new Vector3(1, 0, 0) * Time.deltaTime * swapSpeed;

            if (cardTransform.localScale.x < 0)
            {
                cardTransform.localScale = new Vector3(0, cardSize.y, cardSize.z);

                itemRenderer.sprite = swapSprite;
                swapSprite = null;

                scaleAnimation = "grow";
            }
        }

        else if (scaleAnimation == "grow")
        {
            cardTransform.localScale += new Vector3(1, 0, 0) * Time.deltaTime * swapSpeed;

            if (cardTransform.localScale.x > cardSize.x)
            {
                cardTransform.localScale = cardSize;
                scaleAnimation = null;
            }
        }

        if (cardTransform.position != targetPosition){
            cardTransform.position = Vector3.MoveTowards(cardTransform.position, targetPosition, moveSpeed);
        }
    }

    public void swapFace()
    {
        if (itemRenderer.sprite == defaultSprite)
        {
            scaleAnimation = "shrink";
            swapSprite = itemSprite;
        }
        else if (itemRenderer.sprite == itemSprite)
        {
            scaleAnimation = "shrink";
            swapSprite = defaultSprite;
        }
    }
}
