using System.Collections;
using UnityEngine;

public class RestAreaTutorialSetup : MonoBehaviour
{
    [SerializeField] private GameObject RestAreaDialogue;
    [SerializeField] private GameObject ShopMenuDialogue;
    [SerializeField] private GameObject SkillMenuDialogue;
    [SerializeField] private GameObject attributeMenu;
    [SerializeField] private GameObject ItemShopMenu;
    [SerializeField] private GameObject SkillShopMenu;
    [SerializeField] private GameObject xbuttonshop;
    [SerializeField] private GameObject xbuttonskill;

    private int tutorialCounter;

    public SinglePlayerAttack playerAttack;

    void Start()
    {
        tutorialCounter = PlayerPrefs.GetInt("RestAreaTutorial", 0); // Load progress



        if (tutorialCounter == 0)
        {
            StartCoroutine(RestAreaTutorialDialogue());
        }
    }

    private IEnumerator RestAreaTutorialDialogue()
    {
        RestAreaDialogue.SetActive(true);
        yield return StartCoroutine(ShowDialogAndWait(RestAreaDialogue));
        RestAreaDialogue.SetActive(false);
        tutorialCounter++;
        PlayerPrefs.SetInt("RestAreaTutorial", tutorialCounter); // Save progress
        PlayerPrefs.Save();
        StartCoroutine(TutorialItemShop());
    }

    private IEnumerator TutorialItemShop()
    {
        if (tutorialCounter == 1)
        {
            ItemShopMenu.SetActive(true);
            xbuttonshop.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            ShopMenuDialogue.SetActive(true);
            yield return StartCoroutine(ShowDialogAndWait(ShopMenuDialogue));
            ShopMenuDialogue.SetActive(false);
            ItemShopMenu.SetActive(false);
            xbuttonshop.SetActive(true);
            tutorialCounter++;
            PlayerPrefs.SetInt("RestAreaTutorial", tutorialCounter);
            PlayerPrefs.Save();
            StartCoroutine(TutorialSkillShop());
        }
    }

    private IEnumerator TutorialSkillShop()
    {
        if (tutorialCounter == 2)
        {
            SkillShopMenu.SetActive(true);
            xbuttonskill.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            SkillMenuDialogue.SetActive(true);
            yield return StartCoroutine(ShowDialogAndWait(SkillMenuDialogue));
            SkillMenuDialogue.SetActive(false);
            SkillShopMenu.SetActive(false);
            xbuttonskill.SetActive(true);
            tutorialCounter++;

            PlayerPrefs.SetInt("RestAreaTutorial", tutorialCounter);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator ShowDialogAndWait(GameObject dialogBox)
    {
        DialogBox dialog = dialogBox.GetComponent<DialogBox>();
        if (dialog != null)
        {
            while (dialog.gameObject.activeSelf)
            {
                yield return null; // Wait until dialog is closed
            }
        }
    }

}
