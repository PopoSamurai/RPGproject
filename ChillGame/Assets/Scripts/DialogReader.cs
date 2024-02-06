using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogReader : MonoBehaviour
{
    public Dialog currentDialog;

    [Header("UI Elements")]
    public Image Player, effectP;
    public Image NPC, effectN;
    public Text dialogText, nameText;
    public Button NextButton;
    public GameObject dialogBox;
    private void Start()
    {
        NextButton.onClick.AddListener(HandleContinueClick);
        UpdateUI();
    }
    public void ActivePlayer()
    {
        if(currentDialog.activePlayer == 0)
        {
            Player.color = Color.white;
            NPC.color = Color.gray; 
            effectP.color = Color.white;
            effectN.color = Color.gray;
            nameText.text = currentDialog.NamePlayer;
        }
        else
        {
            Player.color = Color.gray;
            NPC.color = Color.white;
            effectP.color = Color.gray;
            effectN.color = Color.white;
            nameText.text = currentDialog.NameNPC;
        }
    }
    public void StartDialog(Dialog dialog)
    {
        currentDialog = dialog;
    }
    public void UpdateUI()
    {
        if (currentDialog == null) return;

        Player.sprite = currentDialog.characterSprite;
        effectP.sprite = currentDialog.playerEffect;
        NPC.sprite = currentDialog.NPCSprite;
        effectN.sprite = currentDialog.npcEffect;
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
    public void EndDialog()
    {
        dialogBox.SetActive(false);
        Movement.move = true;
    }
}