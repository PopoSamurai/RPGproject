using System.Collections;
using UnityEngine.UI;
using UnityEngine;
public enum Turn
{
    Player,
    Enemy
}
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public Turn currentTurn;
    public float enemyPlayDelay = 1f;
    public int turnNumber = 0;
    public bool IsPlayerTurn => currentTurn == Turn.Player;
    // Turn next effect
    public Camera mainCamera;

    [Header("Turn Zoom")]
    public float zoomInSize = 4.7f;
    public float defaultZoomSize = 5f;
    public float zoomDuration = 0.15f;

    [Header("Turn Text")]
    public GameObject turnText;
    public float turnTextScaleMultiplier = 1.2f;
    [Header("Screen")]
    public GameObject skipWind;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        StartPlayerTurn();
    }
    public void UpdateTurn()
    {
        turnNumber++;
        turnText.GetComponent<Text>().text = "Turn " + turnNumber.ToString();
        StopAllCoroutines();
        StartCoroutine(TurnTransitionFX());
    }
    IEnumerator TurnTransitionFX()
    {
        float time = 0f;
        while (time < zoomDuration)
        {
            time += Time.deltaTime;

            mainCamera.orthographicSize = Mathf.Lerp(
                defaultZoomSize,
                zoomInSize,
                time / zoomDuration
            );

            yield return null;
        }
        time = 0f;
        while (time < zoomDuration)
        {
            time += Time.deltaTime;

            mainCamera.orthographicSize = Mathf.Lerp(
                zoomInSize,
                defaultZoomSize,
                time / zoomDuration
            );

            yield return null;
        }
        RectTransform rt = turnText.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one;
        Vector3 targetScale = Vector3.one * turnTextScaleMultiplier;
        time = 0f;
        while (time < 0.1f)
        {
            time += Time.deltaTime;

            rt.localScale = Vector3.Lerp(
                originalScale,
                targetScale,
                time / 0.1f
            );
            yield return null;
        }
        time = 0f;
        while (time < 0.1f)
        {
            time += Time.deltaTime;

            rt.localScale = Vector3.Lerp(
                targetScale,
                originalScale,
                time / 0.1f
            );
            yield return null;
        }
        rt.localScale = originalScale;
    }
    public void StartPlayerTurn()
    {
        UpdateTurn();
        currentTurn = Turn.Player;
        FindObjectOfType<HandManager>().FillHandToMax();
    }
    public void EndPlayerTurn()
    {
        UpdateTurn();
        StartCoroutine(EnemyTurnRoutine());
    }
    IEnumerator EnemyTurnRoutine()
    {
        currentTurn = Turn.Enemy;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(BattleSystem.Instance.TurnRoutine());

        EndEnemyTurn();
    }
    void EndEnemyTurn()
    {
        StartPlayerTurn();
    }
}