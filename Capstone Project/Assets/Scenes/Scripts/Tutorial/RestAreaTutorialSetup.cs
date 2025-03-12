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

    private int tutorialCounter;

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
        yield return new WaitForSeconds(3f);
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
            yield return new WaitForSeconds(0.5f);
            ShopMenuDialogue.SetActive(true);
            yield return new WaitForSeconds(5f);
            ShopMenuDialogue.SetActive(false);
            ItemShopMenu.SetActive(false);
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
            yield return new WaitForSeconds(0.5f);
            SkillMenuDialogue.SetActive(true);
            yield return new WaitForSeconds(5f);
            SkillMenuDialogue.SetActive(false);
            SkillShopMenu.SetActive(false);
            tutorialCounter++;
            PlayerPrefs.SetInt("RestAreaTutorial", tutorialCounter);
            PlayerPrefs.Save();
        }
    }
}
