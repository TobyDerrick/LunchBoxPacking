using System;
using UnityEngine;

public class RequestManager : MonoBehaviour 
{
    [SerializeField] private Lunchbox lunchbox;

    public TraitRequirements currentRequest;

    private void Awake()
    {
        GenerateNewRequest();
    }
    public void GenerateNewRequest()
    {
        currentRequest = TraitRequirements.GenerateRandom(0f, 1f);
        EventBus.EmitNewRequest(currentRequest);
    }

    public void ValidateRequest()
    {
        //Gain points / change npc reaction based on how accurately lunchbox fills request
    }
}
