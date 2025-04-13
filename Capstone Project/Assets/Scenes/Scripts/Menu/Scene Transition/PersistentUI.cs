using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    void Awake()
    {
        // Ensure only one instance of the inventory persists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // This ensures the object doesn't get destroyed when loading a new scene
        }
        else
        {
            Destroy(gameObject);  // If there's already an instance, destroy the new one
        }
    }
}

