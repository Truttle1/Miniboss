using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonradEntrance : MonoBehaviour
{
    [SerializeField] private GameObject konradPrefab;

    [SerializeField] private string label;

    private SpriteRenderer sr;

    [SerializeField] private Direction direction = Direction.Right;


    void Start()
    {
        EventBus.Subscribe<KonradEntranceEvent>(OnKonradEntranceEvent);
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    private void OnKonradEntranceEvent(KonradEntranceEvent entranceEvent)
    {
        if (entranceEvent.entranceName.Equals(label))
        {
            GameObject konrad = Instantiate(konradPrefab, transform.position, Quaternion.identity);
            konrad.name = "Konrad";
            
            GameObject overworldRoot = GameObject.Find("OverworldRoot");
            konrad.transform.SetParent(overworldRoot.transform, false);
            konrad.transform.position += new Vector3(0, 0, 10f); // Bring to front

            // Set facing direction
            PlayerMovement playerMovement = konrad.GetComponent<PlayerMovement>();
            playerMovement.SetFacingLeft(direction == Direction.Left);
        }
    }
}

public class KonradEntranceEvent
{
    public string entranceName;
    public KonradEntranceEvent(string entranceName)
    {
        this.entranceName = entranceName;
    }
}

public enum Direction
{
    Left,
    Right
}