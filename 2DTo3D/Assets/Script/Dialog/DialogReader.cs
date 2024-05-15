using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class DialogReader : MonoBehaviour
{
    public DialogPref currentDialog;

    [Header("UI Elements")]
    public Image Player;
    public Image NPC;
    public Text dialogText, nameText;
    public Button NextButton;
    public GameObject dialogBox;
    public GameObject buttons;
    //public GameObject[] shopWin;
    //public bool shopOpen = false;
    private void Start()
    {
        NextButton.onClick.AddListener(HandleContinueClick);
        UpdateUI();
    }
    public void ActivePlayer()
    {
        if (currentDialog.activePlayer == 0)
        {
            Player.color = Color.white;
            NPC.color = Color.gray;
            nameText.text = currentDialog.NamePlayer;
        }
        else
        {
            Player.color = Color.gray;
            NPC.color = Color.white;
            nameText.text = currentDialog.NameNPC;
        }
    }
    public void StartDialog(DialogPref dialog)
    {
        currentDialog = dialog;
    }
    public void UpdateUI()
    {
        if (currentDialog == null) return;
        buttons.SetActive(false);
        Player.sprite = currentDialog.characterSprite;
        NPC.sprite = currentDialog.NPCSprite;
        StartCoroutine(DisplayText(currentDialog.message));
        ActivePlayer();
    }
    IEnumerator DisplayText(string line)
    {
        dialogText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            NextButton.gameObject.SetActive(false);
            dialogText.text += letter;
            yield return new WaitForSeconds(0.05f);
            NextButton.gameObject.SetActive(true);
        }
    }
    void HandleContinueClick()
    {
        if (currentDialog.nextMessage)
        {
            currentDialog = currentDialog.nextMessage;
            UpdateUI();
        }
        else
        {
            EndDialog();
        }
    }
    //public void openShop()
    //{
    //    Movement.move = false;
    //    shopWin[0].SetActive(true);
    //    shopWin[1].SetActive(true);
    //    shopWin[2].SetActive(false);
    //}
    public void EndDialog()
    {
        dialogBox.SetActive(false);
        Movement.move = true;
        buttons.SetActive(true);
        //if (shopOpen)
        //{
        //    openShop();
        //}
    }
}