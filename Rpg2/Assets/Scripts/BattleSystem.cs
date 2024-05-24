using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject enemy = null;
    [SerializeField] private Slider playerHealth = null;
    [SerializeField] private Slider enemyHealth = null;
    [SerializeField] private Button attackBtn = null;
    [SerializeField] private Button healthBtn = null;
    private GameManager gameM;
    //
    public Text infoText;
    private bool isPlayerTurn = true;
    public int i = 0;
    private void Start()
    {
        gameM = FindObjectOfType<GameManager>();

        i = Random.Range(1, 3);
        if (i == 1)
        {
            infoText.text = "Twoja Tura...";
            isPlayerTurn = true;
            attackBtn.interactable = true;
            healthBtn.interactable = true;
        }
        else
        {
            infoText.text = "Tura wroga...";
            isPlayerTurn = false;
            attackBtn.interactable = false;
            healthBtn.interactable = false;
            StartCoroutine(EnemyTurn());
        }
        playerHealth.maxValue = 50;
        enemyHealth.maxValue = 50;
        playerHealth.value = 50;
        enemyHealth.value = 50;
    }
    private void Update()
    {
        if(enemyHealth.value <= 0)
        {
            StartCoroutine(win());
        }
        else if(playerHealth.value <= 0)
        {
            StartCoroutine(lose());
        }
    }
    public IEnumerator lose()
    {
        infoText.text = "Przegrana";
        attackBtn.interactable = false;
        healthBtn.interactable = false;
        yield return new WaitForSeconds(3);
        gameM.hit.gameObject.SetActive(false);
        gameM.battlePanel.SetActive(false);
        gameM.DeSelect();
    }
    public IEnumerator win()
    {
        infoText.text = "Zwyciêstwo";
        attackBtn.interactable = false;
        healthBtn.interactable = false;
        yield return new WaitForSeconds(3);
        gameM.hit.gameObject.SetActive(false);
        gameM.battlePanel.SetActive(false);
        gameM.DeSelect();
    }
    public void Attack(GameObject target, float damage)
    {
        if(target == enemy)
        {
            enemyHealth.value -= damage;
        }
        else
        {
            playerHealth.value -= damage;
        }

        ChangeTurn();
    }
    private void Heal(GameObject target, float amount)
    {
        if (target == enemy)
        {
            enemyHealth.value += amount;
        }
        else
        {
            playerHealth.value += amount;
        }

        ChangeTurn();
    }
    public void BtnAttack()
    {
        Attack(enemy, 10);
    }
    public void BtnHeal()
    {
        Heal(player, 5);
    }
    private void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        if(!isPlayerTurn)
        {
            attackBtn.interactable = false;
            healthBtn.interactable = false;

            StartCoroutine(EnemyTurn());
        }
        else
        {
            infoText.text = "Twoja tura...";
            attackBtn.interactable = true;
            healthBtn.interactable = true;
        }
    }
    private IEnumerator EnemyTurn()
    {
        infoText.text = "Tura wroga...";
        yield return new WaitForSeconds(3);
        int random = 0;
        random = Random.Range(1, 3);
        if(random == 1)
        {
            Attack(player, 10);
        }
        else
        {
            Heal(enemy, 5);
        }
    }
}