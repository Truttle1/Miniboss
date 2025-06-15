using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldManager : MonoBehaviour
{
    public static OverworldManager Instance;
    private List<Freezable> cachedFreezables = new List<Freezable>();

    private GameObject overworldRoot;

    private bool startedEncounter = false;

    [SerializeField] private float fadeDelay = 0.5f; // Duration of the fade effect

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Kill duplicates!
        }
    }

    void Start()
    {
        EventBus.Subscribe<EncounterStartEvent>(OnEncounterStart);
    }

    public void FreezeAll()
    {
        cachedFreezables.Clear();
        cachedFreezables.AddRange(FindObjectsOfType<Freezable>());

        foreach (Freezable f in cachedFreezables)
        {
            f.Freeze();
        }
    }

    public void UnfreezeAll()
    {
        foreach (Freezable f in cachedFreezables)
        {
            f.Unfreeze();
        }
        cachedFreezables.Clear();
    }

    private void OnEncounterStart(EncounterStartEvent encounterEvent)
    {
        if(!startedEncounter) 
        {
            startedEncounter = true;
            StartCoroutine(StartEncounter(encounterEvent));
            EventBus.Publish(new FadeEvent(false));
        }
    }

    private IEnumerator StartEncounter(EncounterStartEvent encounterEvent)
    {
        startedEncounter = true;
        GameManager.Instance.setEncounterSource(encounterEvent.source);
        GameManager.Instance.setEncounter(encounterEvent.encounterPrefab);
        FreezeAll();
        yield return new WaitForSeconds(fadeDelay);
        SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
        overworldRoot = GameObject.Find("OverworldRoot");
        overworldRoot.SetActive(false);
        startedEncounter = false;
    }

    public void OnEncounterEnd()
    {
        overworldRoot.SetActive(true);
        SceneManager.UnloadSceneAsync("Battle");
        if(GameManager.Instance.getBattleStatus() == LastBattleStatus.Victory)
        {
            Destroy(GameManager.Instance.getEncounterSource());
        }
        else
        {
            PlayerMovement konrad = FindObjectOfType<PlayerMovement>();
            konrad.TriggerIFrames();
        }
        GameManager.Instance.setEncounter(null);
        UnfreezeAll();
        EventBus.Publish(new EncounterEndEvent());
        EventBus.Publish(new FadeEvent(true));
    }

}

public class EncounterStartEvent
{
    public GameObject encounterPrefab;
    public GameObject source;

    public EncounterStartEvent(GameObject encounterPrefab, GameObject source = null)
    {
        this.encounterPrefab = encounterPrefab;
        this.source = source;
    }
}

public class EncounterEndEvent
{
    // This class can be expanded if needed in the future
}