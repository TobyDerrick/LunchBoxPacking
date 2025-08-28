using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//SHOULD NOT HANDLE BOX SPAWNING IDEALLY. SEPERATE TO A "BOX SPAWNER" CLASS
public class RequestManager : MonoBehaviour
{
    [SerializeField] private Lunchbox lunchbox;
    [SerializeField] private Transform boxTarget;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform boxSpawn;

    [SerializeField] private Button submitButton;

    [SerializeField] private GameObject[] boxDividers; //SHOULDNT BE HERE. REWORK BOX SPAWNING LOGIC.

    private GameObject currentLunchbox;

    public TraitRequirements currentRequest { get; private set; }
    private void Awake()
    {
        EventBus.OnNewNPC += SpawnNewLunchbox;
    }
    public void GenerateNewRequest()
    {
        currentRequest = TraitRequirements.GenerateRandom(0f, 1f);
        EventBus.EmitNewRequest(currentRequest);
    }

    public void ValidateRequest()
    {
        submitButton.interactable = false;
        if (lunchbox == null || currentRequest == null)
        {
            Debug.LogWarning("No lunchbox or request available for validation.");
            return;
        }

        float score = CalculateBoxScore(lunchbox, currentRequest);
        foreach(GameObject go in lunchbox.foodInBox)
        {
            Rigidbody rb = go.GetComponentInChildren<Rigidbody>();
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            go.transform.SetParent(lunchbox.transform, true);
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(lunchbox.lid.transform.DOMove(lunchbox.lidTarget.position, 0.3f).SetEase(Ease.OutBack));
        seq.Append(lunchbox.transform.DOMove(boxTarget.position, 0.3f).SetEase(Ease.InOutQuad));
        seq.OnComplete(() =>
        {
            EventBus.EmitRequestValidated(score, lunchbox);
        });

        Debug.Log($"Request validated with score: {score}");
    }

    public void SpawnNewLunchbox(NPCData npc)
    {

        currentRequest = npc.request;
        currentLunchbox = Instantiate(boxPrefab, boxSpawn.position, boxSpawn.rotation);

        Vector3 offScreenPos = boxSpawn.position + Vector3.left * 4f;
        currentLunchbox.transform.position = offScreenPos;

        currentLunchbox.transform.DOMove(boxSpawn.position, 0.5f).SetEase(Ease.InOutBack);
        lunchbox = currentLunchbox.GetComponent<Lunchbox>();
        lunchbox.lid.transform.localPosition = new Vector3(-2f, 0.8f, 3.1f);

        GameObject dividerToSpawn = boxDividers[UnityEngine.Random.Range(0, boxDividers.Length)];

        GameObject divider = Instantiate(dividerToSpawn, currentLunchbox.transform.position, Quaternion.identity, currentLunchbox.transform);
        divider.transform.localRotation = Quaternion.identity;

        Debug.Log("HAHAHAHAHA");
        EventBus.EmitNewLunchBox(lunchbox);
        submitButton.interactable = true;
    }



    private float CalculateBoxScore(Lunchbox lunchbox, TraitRequirements request)
    {
        float totalDiff = 0f;
        int traitCount = 0;

        foreach (FoodTrait trait in Enum.GetValues(typeof(FoodTrait)))
        {
            float value = lunchbox.TraitValues.GetTraitValue(trait);
            float min = request.minTraits.GetTraitValue(trait);
            float max = request.maxTraits.GetTraitValue(trait);

            float diff = 0f;

            //its good enough to just be in the bar
            if (value < min) diff = min - value;
            else if (value > max) diff = value - max;

            totalDiff += diff;
            traitCount++;
        }

        float avgDiff = totalDiff / traitCount; // should try to match each trait, rather than just 1 hugely
        float score = Mathf.Clamp01(1f - avgDiff); // 1 = perfect match

        return score * 100;
    }
}
