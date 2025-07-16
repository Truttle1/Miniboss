using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BigTextController : MonoBehaviour
{
    [SerializeField] private AnimationCurve enter;
    [SerializeField] private BigTextType type;

    [SerializeField] private TMP_Text expText;
    [SerializeField] private TMP_Text levelUpText;
    
    private Vector2 scale = Vector2.zero;

    private void Start()
    {
        EventBus.Subscribe<BigTextStartEvent>(OnBigTextStart);
        gameObject.transform.localScale = scale;
    }

    private void OnBigTextStart(BigTextStartEvent bigTextEvent)
    {
        if (bigTextEvent.type == type)
        {
            if(bigTextEvent.type == BigTextType.Win)
            {
                if (bigTextEvent.EXPCount > 0)
                {
                    expText.SetText("Konrad recieved $" + bigTextEvent.money.ToString() + " and " + bigTextEvent.EXPCount.ToString() + " XP!");
                }
                else
                {
                    expText.SetText("");
                }

                if (bigTextEvent.levelUp)
                {
                    levelUpText.SetText("Konrad leveled up to Level " + GameManager.Instance.getLevel() + "!");
                }
                else
                {
                    levelUpText.SetText("");
                }
            }
            StartCoroutine(AnimateBigText());
        }
    }

    private IEnumerator AnimateBigText()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            float t = enter.Evaluate(time);
            gameObject.transform.localScale = Vector2.LerpUnclamped(scale, Vector2.one, t);
            yield return null;
        }
        gameObject.transform.localScale = Vector2.one;
        EventBus.Publish(new PermitLeaveBattleEvent());
    }
}

public enum BigTextType
{
    Win,
    GameOver,
    Escape
}

public class BigTextStartEvent
{
    public BigTextType type;

    public int EXPCount, money;

    public bool levelUp = false;

    public BigTextStartEvent(BigTextType type, int EXPCount = 0, int money = 0, bool levelUp = false)
    {
        this.type = type;
        this.EXPCount = EXPCount;
        this.levelUp = levelUp;
        this.money = money;
    }
}

