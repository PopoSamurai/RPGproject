using UnityEngine;
using UnityEngine.UI;

public class DialogReader : MonoBehaviour
{
    public Dialog currentDialog, end;

    [Header("UI Elements")]
    public Image Player, effectP;
    public Image NPC, effectN;
    public Text dialogText, nameText;
    public Button NextButton;
    public float typingSpeed = 0.05f;
    //
    public GameObject dialogBox;

    private void Start()
    {
        NextButton.onClick.AddListener(HandleContinueClick);
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
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (currentDialog == null) return;

        Player.sprite = currentDialog.characterSprite;
        effectP.sprite = currentDialog.playerEffect;
        NPC.sprite = currentDialog.NPCSprite;
        effectN.sprite = currentDialog.npcEffect;
        dialogText.text = currentDialog.message;
        ActivePlayer();
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
        currentDialog = end.endDialog;
        Player.sprite = end.characterSprite;
        effectP.sprite = end.playerEffect;
        NPC.sprite = end.NPCSprite;
        effectN.sprite = end.npcEffect;
        dialogText.text = end.message;
        ActivePlayer();

        dialogBox.SetActive(false);
        Movement.move = true;
    }
}