using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTextController : MonoBehaviour
{
    [SerializeField] private AnimationCurve enter;
    [SerializeField] private BigTextType type;
    
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

    public BigTextStartEvent(BigTextType type)
    {
        this.type = type;
    }
}

