﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrewingStation : Countertop
{
    [HideInInspector] public List<IngredientData> ingredientDatas = new List<IngredientData>();
    public int maxIngredient;
    public float brewTime;
    [HideInInspector] public float brewElapsedTime;

    public GameObject uiBrewingTimeBar;

    public AntidoteRecipeBook antidoteRecipeBook;

    [HideInInspector] public bool brewing;

    private AntidoteRecipeData selectedRecipeData;
    private Coroutine brewingCoroutine;

    public float BrewingProgress
    {
        get
        {
            return brewElapsedTime / brewTime;
        }
    }

    public override void Interact(GameObject instigator)
    {
        var player = instigator.GetComponentInParent<PlayerController>();
        if (player == null) return;
        if (player.heldItem == null)
        {
            if (placedItem != null)
            {
                base.Interact(instigator);
            }
            else
            {
                ClearAllIngredients();
            }
        }
        else
        {
            AddIngredient(player.heldItem);
        }
    }

    private void ClearAllIngredients()
    {
        if (brewing)
            brewing = false;

        ingredientDatas.Clear();
    }

    private void AddIngredient(GenericItem item)
    {
        if (item == null) return;

        if (ingredientDatas.Count != maxIngredient)
        {
            var ingredient = item as IngredientObject;

            if (ingredient != null)
            {
                ingredientDatas.Add(ingredient.ingredientData);

                Destroy(item.gameObject);

                if (ingredientDatas.Count == maxIngredient)
                {
                    StartBrewing();
                }
            }
        }
    }

    private void StartBrewing()
    {
        if (CheckIfIngredientsMatchesRecipe() && !brewing)
        {
            Debug.Log("START BREWING");
            brewingCoroutine = StartCoroutine(BrewAntidote());

            if (uiBrewingTimeBar != null)
            {
                uiBrewingTimeBar.SetActive(true);
            }
        }
    }

    private IEnumerator BrewAntidote()
    {
        brewing = true;
        brewElapsedTime = 0;
        while(brewElapsedTime < brewTime)
        {
            brewElapsedTime += Time.deltaTime;
            yield return null;
        }

        StopBrewing();
        CreateAntidote();
    }

    private bool CheckIfIngredientsMatchesRecipe()
    {
        if (antidoteRecipeBook != null)
        {
            selectedRecipeData = antidoteRecipeBook.GetMatchingRecipe(ingredientDatas);           
            
            return selectedRecipeData != null;
        }

        return false;
    }

    private void StopBrewing()
    {
        if (brewingCoroutine != null)
        {
            StopCoroutine(brewingCoroutine);
        }

        brewing = false;

        if (uiBrewingTimeBar != null)
            uiBrewingTimeBar.SetActive(false);
    }

    private void CreateAntidote()
    {
        if (selectedRecipeData == null) return;

        var antidote = Instantiate(selectedRecipeData.antidotePrefab);

        antidote.ToggleColliderAndGravity(true);
        PlaceItemOnCounter(antidote);
    }
}