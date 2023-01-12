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

    public Transform cardTransform;
    BoxCollider2D cardHitbox;

    // swap animation
    string scaleAnimation;
    Sprite swapSprite;
    float swapSpeed;

    public bool flipped;

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
        if (cardHitbox.OverlapPoint(mousePos) && mouseClick && scaleAnimation == null){
            showItem();
        }

        // animation
        if (scaleAnimation == "shrink")
        {
            cardTransform.localScale -= new Vector3(1, 0, 0) * Time.deltaTime * swapSpeed;

            if (cardTransform.localScale.x < 0){
                cardTransform.localScale = new Vector3(0, cardSize.y, cardSize.z);

                itemRenderer.sprite = swapSprite;
                swapSprite = null;

                scaleAnimation = "grow";
            }
        }
        if (scaleAnimation == "die")
        {
            cardTransform.localScale -= new Vector3(1, 1, 0) * Time.deltaTime * swapSpeed;

            if (cardTransform.localScale.x < 0)
            {   
                cardManager.deleteCard(this);
                Destroy(cardObject);
                Destroy(this);
            }
        }
        if (scaleAnimation == "grow"){
            cardTransform.localScale += new Vector3(1, 0, 0) * Time.deltaTime * swapSpeed;

            if (cardTransform.localScale.x > cardSize.x)
            {
                scaleAnimation = null;
                cardTransform.localScale = cardSize;
            }
        }

        if (cardTransform.position != targetPosition){
            Vector2 moveVec = Vector2.MoveTowards((Vector2)cardTransform.position, (Vector2)targetPosition, moveSpeed);
            cardTransform.position = new Vector3(moveVec.x, moveVec.y, cardTransform.position.z);
        }
    }

    public void showItem(){
        if (!flipped){
            flipped = true;

            scaleAnimation = "shrink";
            swapSprite = itemSprite;
            
            cardManager.cardShown(this);
        }
    }
    public void hideItem(){
        if (flipped){
            flipped = false;

            scaleAnimation = "shrink";
            swapSprite = defaultSprite;
        }
    }
    
    public void cardDie(){
        scaleAnimation = "die";
    }
}
