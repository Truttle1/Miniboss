using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Overworld,
    Battle,
}

public enum LastBattleStatus
{
    Victory,
    Defeat,
    Escape,
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [SerializeField] private int maxHP = 15;
    [SerializeField] private int currentHP = 15;

    [SerializeField] private GameObject encounter;
    [SerializeField] private GameObject encounterSource;

    [SerializeField] private GameState gameState = GameState.Overworld;

    [SerializeField] private int level = 1;
    [SerializeField] private int exp = 0;
    [SerializeField] private int[] expToNextLevel = {
        0,  // Level 0 (not used)
        25, // Level 1
        150, // Level 2
        300, // Level 3
        400, // Level 4
        600, // Level 5
    };

    [SerializeField] private int money = 0;

    [SerializeField] private GameObject fade;

    private Image fadeImage;

    [SerializeField] private float fadeDuration = .4f; // Duration of the fade effect

    [SerializeField] private LastBattleStatus lastBattleStatus = LastBattleStatus.Victory;

    [SerializeField] public Item[] itemList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
        }
        else
        {
            Destroy(gameObject);  // Kill duplicates!
        }
    }

    void Start()
    {
        // Initialize fade image
        if (fade != null)
        {
            fadeImage = fade.GetComponent<Image>();
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, 0);
            }
        }
        EventBus.Subscribe<FadeEvent>(OnFadeEvent);

    }

    public void setHPStats(int max, int current)
    {
        maxHP = max;
        currentHP = Mathf.Clamp(current, 0, maxHP);
    }
    
    public int getMaxHP()
    {
        return maxHP;
    }

    public int getCurrentHP()
    {
        return currentHP;
    }

    public void setEncounter(GameObject encounter)
    {
        this.encounter = encounter;
    }

    public GameObject getEncounter()
    {
        return encounter;
    }

    public void setGameState(GameState state)
    {
        gameState = state;
    }

    public GameState getGameState()
    {
        return gameState;
    }

    public void setEncounterSource(GameObject source)
    {
        encounterSource = source;
    }

    public GameObject getEncounterSource()
    {
        return encounterSource;
    }

    public int getLevel()
    {
        return level;
    }

    public int getEXP()
    {
        return exp;
    }

    public void addEXP()
    {
        exp++;
        while (exp >= expToNextLevel[level])
        {
            levelUp();
        }
    }

    private void levelUp()
    {
        // TODO: Implement level-up logic (e.g., increase max HP, grant new abilities, etc.)
        level++;
    }

    public int getMoney()
    {
        return money;
    }

    public void addMoney(int amount)
    {
        money += amount;
        if (money < 0)
        {
            money = 0; // Prevent negative money
        }
    }

    public bool payMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true; // Payment successful
        }
        return false; // Not enough money
    }

    private void OnFadeEvent(FadeEvent fadeEvent)
    {
        if (fadeImage != null)
        {
            if(fadeEvent.fadeIn)
            {
                StartCoroutine(FadeIn());
            }
            else
            {
                StartCoroutine(FadeOut());
            }
        }
    }

    private IEnumerator FadeIn() 
    {
        float duration = fadeDuration;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        EventBus.Publish(new FadeEndEvent(true)); // Notify that fade-in is complete
    }

    private IEnumerator FadeOut() 
    {
        float duration = fadeDuration;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        EventBus.Publish(new FadeEndEvent(false)); // Notify that fade-out is complete
    }

    public void setBattleStatus(LastBattleStatus status)
    {
        lastBattleStatus = status;
    }

    public LastBattleStatus getBattleStatus()
    {
        return lastBattleStatus;
    }

    public Item[] getItemList()
    {
        return itemList;
    }

    public Item GetItemFromName(string itemName)
    {
        foreach (Item item in itemList)
        {
            if (item.name.Equals(itemName, StringComparison.OrdinalIgnoreCase))
            {
                return item;
            }
        }
        return null; // Item not found
    }
}

public class FadeEvent
{
    public bool fadeIn;

    public FadeEvent(bool fadeIn)
    {
        this.fadeIn = fadeIn;
    }
}

public class FadeEndEvent
{
    public bool fadeIn;

    public FadeEndEvent(bool fadeIn)
    {
        this.fadeIn = fadeIn;
    }
}