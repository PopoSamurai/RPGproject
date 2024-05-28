using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private Slider playerHealth = null;
    [SerializeField] private Slider enemyHealth = null;
    [SerializeField] private Button attackBtn = null;
    [SerializeField] private Button healthBtn = null;
    public Character player1;
    public Character enemy1;
    [SerializeField] private Image player1Sprite;
    [SerializeField] private Image enemy1Sprite;
    private GameManager gameM;
    //
    public Text infoText;
    private bool isPlayerTurn = true;
    public int i = 0;
    private void Awake()
    {
        player1Sprite.sprite = player1.portrait;
        enemy1Sprite.sprite = enemy1.portrait;
        playerHealth.maxValue = player1.maxHp;
        enemyHealth.maxValue = enemy1.maxHp;
        //wylecz
        enemy1.hpValue = enemy1.maxHp;
        player1.hpValue = player1.maxHp;
    }
    public void Start()
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
    }
    private void Update()
    {
        playerHealth.value = player1.hpValue;
        enemyHealth.value = enemy1.hpValue;

        if (enemy1.hpValue <= 0)
        {
            StartCoroutine(win());
        }
        else if(player1.hpValue <= 0)
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
        gameM.hit.GetComponent<Interact>().coll.enabled = false;
        gameM.hit.GetComponent<Interact>().active = false;
        gameM.battlePanel.SetActive(false);
        gameM.hit.GetComponent<Interact>().active = true;
        player1.hpValue = 1;
        enemy1.hpValue = enemy1.maxHp;
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
        enemy1.hpValue = enemy1.maxHp;
        gameM.DeSelect();
    }
    public void Attack(GameObject target, int damage)
    {
        if(target == enemy1Sprite.gameObject)
        {
            enemy1.hpValue -= damage;
        }
        else
        {
            player1.hpValue -= damage;
        }

        ChangeTurn();
    }
    private void Heal(GameObject target, int amount)
    {
        if (target == enemy1Sprite.gameObject)
        {
            enemy1.hpValue += amount;
        }
        else
        {
            player1.hpValue += amount;
        }

        ChangeTurn();
    }
    public void BtnAttack()
    {
        Attack(enemy1Sprite.gameObject, 10);
    }
    public void BtnHeal()
    {
        Heal(player1Sprite.gameObject, 5);
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
    public IEnumerator EnemyTurn()
    {
        infoText.text = "Tura wroga...";
        yield return new WaitForSeconds(3);
        int random = 0;
        random = Random.Range(1, 3);
        if(random == 1)
        {
            Attack(player1Sprite.gameObject, 10);
        }
        else
        {
            Heal(enemy1Sprite.gameObject, 5);
        }
    }
}