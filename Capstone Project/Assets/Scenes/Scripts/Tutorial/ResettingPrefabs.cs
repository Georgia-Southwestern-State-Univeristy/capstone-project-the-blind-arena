using System.Collections.Generic;
using UnityEngine;

public class ResettingPrefabs : MonoBehaviour
{
    [System.Serializable]
    public class PrefabData
    {
        public GameObject prefabInstance;
        [HideInInspector] public Vector3 originalPosition;
        [HideInInspector] public Quaternion originalRotation;
        [HideInInspector] public Vector3 originalScale;
    }

    [SerializeField] private List<PrefabData> prefabsToReset = new List<PrefabData>();

    void Start()
    {
        // Store the original transforms of each prefab instance
        foreach (var data in prefabsToReset)
        {
            if (data.prefabInstance != null)
            {
                data.originalPosition = data.prefabInstance.transform.position;
                data.originalRotation = data.prefabInstance.transform.rotation;
                data.originalScale = data.prefabInstance.transform.localScale;
            }
        }
    }

    public void ResetSpecificPrefab(GameObject prefab)
    {
        foreach (var data in prefabsToReset)
        {
            if (data.prefabInstance == prefab)
            {
                data.prefabInstance.transform.position = data.originalPosition;
                data.prefabInstance.transform.rotation = data.originalRotation;
                data.prefabInstance.transform.localScale = data.originalScale;
                return;
            }
        }

        Debug.LogWarning("Prefab not found in reset list: " + prefab.name);
    }

    public void ResetAllPrefabs()
    {
        foreach (var data in prefabsToReset)
        {
            if (data.prefabInstance != null)
            {
                data.prefabInstance.transform.position = data.originalPosition;
                data.prefabInstance.transform.rotation = data.originalRotation;
                data.prefabInstance.transform.localScale = data.originalScale;
            }
        }
    }
}
