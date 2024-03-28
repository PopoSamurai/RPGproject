using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class DialogManager : MonoBehaviour
{
    public DialogReader currentDialog;

    [Header("UI Elements")]
    public Text dialogText, nameText;
    public Button NextButton;
    public GameObject dialogBox;
    public bool endDialog = false;
    public Camera cam;
    private void Start()
    {
        endDialog = false;
        UpdateUI();
    }
    public void StartDialog(DialogReader dialog)
    {
        cam.GetComponent<CameraMovement>().on = true;
        dialogBox.SetActive(true);
        currentDialog = dialog;
        UpdateUI();
    }
    public void UpdateUI()
    {
        if (currentDialog == null) return;
        StartCoroutine(DisplayText(currentDialog.message));
    }
    IEnumerator DisplayText(string line)
    {
        nameText.text = currentDialog.NamePlayer;
        dialogText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            NextButton.gameObject.SetActive(false);
            dialogText.text += letter;
            yield return new WaitForSeconds(0.05f);
            NextButton.gameObject.SetActive(true);
        }
    }
    public void HandleContinueClick()
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
        cam.GetComponent<CameraMovement>().off = true;
        dialogBox.SetActive(false);
        endDialog = true;
    }
}