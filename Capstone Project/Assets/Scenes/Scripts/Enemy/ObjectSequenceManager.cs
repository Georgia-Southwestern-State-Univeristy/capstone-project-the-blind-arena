using UnityEngine;
using System.Collections;

public class ObjectSequenceManager : MonoBehaviour
{
    public static ObjectSequenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartObjectSequence(GameObject firstObject, GameObject secondObject, float delay)
    {
        VisibilityObjects firstVisibility = firstObject.GetComponent<VisibilityObjects>();
        VisibilityObjects secondVisibility = secondObject.GetComponent<VisibilityObjects>();

        if (firstVisibility != null && secondVisibility != null)
        {
            StartCoroutine(ShowObjectSequence(firstVisibility, secondVisibility, delay));
        }
    }

    private IEnumerator ShowObjectSequence(VisibilityObjects first, VisibilityObjects second, float delay)
    {
        first.SetVisibility(true);

        yield return new WaitForSeconds(delay);

        first.SetVisibility(false);
        second.SetVisibility(true);
    }
}