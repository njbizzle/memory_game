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

    [SerializeField] List<Card> cards;
    [SerializeField] GameObject cardPrefab;

    List<GameObject> ActiveCards = new List<GameObject>();

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
            
            deck.Insert(UnityEngine.Random.Range(0, deck.Count), cardsCopy[index]);
            deck.Insert(UnityEngine.Random.Range(0, deck.Count), cardsCopy[index]);

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
            
            GameObject cardObject = Instantiate(cardPrefab, new Vector3(xpos, ypos, 0), Quaternion.identity);

            cardObject.GetComponent<CardPrefab>().card = card;
            cardObject.GetComponent<Transform>().localScale = cardSize;
            cardObject.transform.Find("CardImage").GetComponent<SpriteRenderer>().sprite = card.itemSprite;
            print(card.itemSprite);

            ActiveCards.Add(cardObject);

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
        
    }
}
