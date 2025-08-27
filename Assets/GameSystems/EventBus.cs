using System;

public static class EventBus
{
    public static event Action<TraitRequirements> OnNewRequest;
    public static void EmitNewRequest(TraitRequirements req)
    {
        OnNewRequest?.Invoke(req);
    }


    public static event Action<float> OnRequestValidated;
    public static void EmitRequestValidated(float score)
    {
        OnRequestValidated?.Invoke(score);
    }

    public static event Action<NPCData> OnNewNPC;

    public static void EmitNewNPC(NPCData data)
    {
        OnNewNPC?.Invoke(data);
    }
}