using UnityEngine;

public class SceneTrigger : MonoBehaviour
{
    [SerializeField] private string triggerTag = "Player";  // Tag of the object that can trigger the scene change

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag))
        {
            SceneController.Instance.LoadScene(1);
        }
    }
}