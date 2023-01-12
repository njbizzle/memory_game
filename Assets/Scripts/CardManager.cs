using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Sprite defaultImage;

    [SerializeField] Vector2Int gridSize;
    [SerializeField] Vector3 cardSize;
    [SerializeField] Vector2 cardPadding;

    [SerializeField] float growSpeed;

    [SerializeField] List<Card> cards;
    [SerializeField] GameObject cardPrefab;
    
    [SerializeField] Transform cardSpawnPoint;
    [SerializeField] float cardSpeed;

    List<Card> activeCards = new List<Card>();
    List<Vector3> cardPositions = new List<Vector3>();
    public List<Card> cardsToDelete = new List<Card>();

    public List<Card> flippedCards = new List<Card>();

    [SerializeField] Camera mainCamera;

    void Start()
    {
        mainCamera = FindObjectOfType<Camera>().GetComponent<Camera>();
        List<Card> gameDeck = GenDeck(gridSize.x * gridSize.y);
        SetCards(gameDeck);
    }

    public List<Card> GenDeck(int deckLen){

        int uniqueCardCount = deckLen/2;

        List<Card> deck = new List<Card>();

        if (uniqueCardCount > cards.Count){
            uniqueCardCount = cards.Count;
        }

        List<Card> cardsCopy = new List<Card>(cards);

        for(int i = 0; i < uniqueCardCount; i++){

            int index = UnityEngine.Random.Range(0, cardsCopy.Count);
            
            deck.Insert(UnityEngine.Random.Range(0, deck.Count), Instantiate(cardsCopy[index]));
            deck.Insert(UnityEngine.Random.Range(0, deck.Count), Instantiate(cardsCopy[index]));

            cardsCopy.RemoveAt(index);
        }
        return deck;
    }

    public void SetCards(List<Card> deck){

        int gridX = 0;
        int gridY = 0;

        int iter = 0;

        foreach (Card card in deck){

            float xpos = (-gridSize.x/2 * cardPadding.x) + (gridX + 0.5f) * cardPadding.x;
            float ypos = (gridSize.y * cardPadding.y)/2 + (gridY - 0.5f) * cardPadding.y;

            cardPositions.Add(new Vector3(xpos, ypos, 0));

            Vector3 spawn = cardSpawnPoint.position;
            spawn.z = iter;

            GameObject cardObject = Instantiate(cardPrefab, spawn, Quaternion.identity);
            card.targetPosition = new Vector3(xpos, ypos, 0);
            card.moveSpeed = Vector3.Distance(card.targetPosition, spawn) * cardSpeed;

            cardObject.GetComponent<CardPrefab>().card = card;

            card.SetUp(cardObject, cardSize, growSpeed);

            activeCards.Add(card);

            gridX += 1;
            iter += 1;

            if (gridX == gridSize.x){
                gridX = 0;
                gridY -= 1;
            }

            if (gridY == gridSize.y){break;}
        }
    }

    void Update()
    {
        foreach (Card card in cardsToDelete){
            activeCards.Remove(card);
        }
        cardsToDelete = new List<Card>();

        foreach (Card card in activeCards){
            card.cardUpdate(mainCamera.ScreenToWorldPoint(Input.mousePosition), Input.GetKeyDown(KeyCode.Mouse0));
        }
    }

    public void cardShown(Card cardFlipped){
        if (flippedCards.Count >= 2){
            foreach (Card card in flippedCards){
                card.hideItem();
            }
            flippedCards = new List<Card>();
        }

        flippedCards.Add(cardFlipped);
        
        if (flippedCards.Count >= 2){
            if (flippedCards[0].itemName == flippedCards[1].itemName){
                foreach (Card card in flippedCards){
                    card.hideItem();
                    card.cardDie();
                }
                flippedCards = new List<Card>();
                if (activeCards.Count == 0){
                    Debug.Log("you won");
                }
            }
        }

        if (flippedCards.Count == 2){
        }
    }
    public void deleteCard(Card cardFlipped){
        int indexOfCardFlipped = cardPositions.FindIndex(pos => pos == (Vector3)(Vector2)cardFlipped.targetPosition);

        int currentIndex = 0;
        int cardIndexInPos;

        cardsToDelete.Add(cardFlipped);

        foreach (Card card in activeCards){
            
            if (currentIndex > indexOfCardFlipped){
                cardIndexInPos = cardPositions.FindIndex(pos => pos == card.targetPosition);

                try{
                    Vector2 newTarget = cardPositions[cardIndexInPos-1];
                    card.moveSpeed = Vector3.Distance(newTarget, card.cardTransform.position) * cardSpeed;
                    card.targetPosition = newTarget;
                }
                catch{}
            }
            currentIndex++;
        }
    }
}
