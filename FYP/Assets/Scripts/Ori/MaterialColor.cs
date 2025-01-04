using UnityEngine;

public class MaterialColor : MonoBehaviour
{
    public GameObject[] objectsToChange; // Array of objects to modify
    public Material[] levelsOfMaterials; // Materials for different levels

    public void ChangeMaterialByLevel(int levelIndex)
    {
        // Ensure materials exist
        if (levelsOfMaterials.Length == 0)
            return;

        // Clamp the level index to avoid out of range errors
        int materialIndex = Mathf.Clamp(levelIndex, 0, levelsOfMaterials.Length - 1);

        // Change material for each object
        foreach (GameObject obj in objectsToChange)
        {
            if (obj == null) continue;

            // Try to get renderer on the object
            Renderer renderer = obj.GetComponent<Renderer>();

            // If no renderer found, try in children
            if (renderer == null)
                renderer = obj.GetComponentInChildren<Renderer>();

            // Change material if renderer is found
            if (renderer != null)
            {
                renderer.material = levelsOfMaterials[materialIndex];
            }
        }
    }
}