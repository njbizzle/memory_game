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

    List<Card> ActiveCards = new List<Card>();

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
        print(deck.Count);
        return deck;
    }

    public void SetCards(List<Card> deck){

        int gridX = 0;
        int gridY = 0;

        foreach (Card card in deck){

            float xpos = (-gridSize.x/2 * cardPadding.x) + (gridX + 0.5f) * cardPadding.x;
            float ypos = (gridSize.y * cardPadding.y)/2 + (gridY - 0.5f) * cardPadding.y;
            
            GameObject cardObject = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);
            card.targetPosition = new Vector3(xpos, ypos, 0);
            /*
            card.targetPosition
            cardSpawnPoint
            xdif = Mathf.Abs(card.targetPosition.x - cardSpawnPoint.x)
            ydif = Mathf.Abs(card.targetPosition.y - cardSpawnPoint.y)
            Mathf.Sqrt(Mathf.Pow(xdif, 2f) + Mathf.Pow(ydif, 2f))
            */
            card.moveSpeed = Vector3.Distance(card.targetPosition, cardSpawnPoint.position) * cardSpeed;

            cardObject.GetComponent<CardPrefab>().card = card;

            card.SetUp(cardObject, cardSize, growSpeed);

            ActiveCards.Add(card);

            gridX += 1;

            if (gridX == gridSize.x){
                gridX = 0;
                gridY -= 1;
            }

            if (gridY == gridSize.y){break;}
        }
    }

    void Update()
    {
        foreach (Card card in ActiveCards){
            card.cardUpdate(mainCamera.ScreenToWorldPoint(Input.mousePosition), Input.GetKeyDown(KeyCode.Mouse0));
        }
    }
}
