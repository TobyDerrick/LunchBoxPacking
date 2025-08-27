using System;

public static class EventBus
{
    public static event Action<TraitRequirements> OnNewRequest;

    public static void EmitNewRequest(TraitRequirements req)
    {
        OnNewRequest?.Invoke(req);
    }
}