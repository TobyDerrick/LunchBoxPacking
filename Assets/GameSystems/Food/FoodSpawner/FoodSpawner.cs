using System.Linq;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private Transform foodSpawnAnchor;
    private GameObject currentFood;

    public GameObject SpawnFood(FoodScriptableObject food)
    {
        foreach (Transform child in foodSpawnAnchor)
            Destroy(child.gameObject);

        if (food.FoodPrefab != null)
        {
            GameObject spawned = Instantiate(
                food.FoodPrefab,
                foodSpawnAnchor.position,
                Quaternion.identity
            );

            spawned.transform.SetParent(foodSpawnAnchor, true);
            spawned.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);


            spawned.AddComponent<FoodItem>().id = food.id;

            CombineChildMeshes(spawned);

            currentFood = spawned;
            return spawned;
        }

        return null;
    }

    public void ClearFood()
    {
        if (currentFood != null)
        {
            Destroy(currentFood);
            currentFood = null;
        }
    }

    private void CombineChildMeshes(GameObject parent)
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length == 0) return;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        }


        MeshFilter mf = parent.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.CombineMeshes(combine, false, true);

        MeshRenderer mr = parent.AddComponent<MeshRenderer>();
        Material[] allMaterials = meshFilters
            .Select(m => m.GetComponent<MeshRenderer>().sharedMaterial)
            .ToArray();
        mr.materials = allMaterials;

        MeshCollider mc = parent.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;
        mc.convex = true;

        Rigidbody rb = parent.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = parent.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

}
