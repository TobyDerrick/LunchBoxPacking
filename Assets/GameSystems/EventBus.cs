using System;
using System.Diagnostics;

public static class EventBus
{
    public static event Action<TraitRequirements> OnNewRequest;
    public static void EmitNewRequest(TraitRequirements req)
    {
        OnNewRequest?.Invoke(req);
    }


    public static event Action<float, Lunchbox> OnRequestValidated;
    public static void EmitRequestValidated(float score, Lunchbox box)
    {
        OnRequestValidated?.Invoke(score, box);
    }

    public static event Action<NPCData> OnNewNPC;

    public static void EmitNewNPC(NPCData data)
    {
        OnNewNPC?.Invoke(data);
    }

    public static event Action<Lunchbox> OnNewLunchbox;
    public static void EmitNewLunchBox(Lunchbox box)
    {
        OnNewLunchbox?.Invoke(box);
    }

    public static event Action<CharacterData> OnNewCharacterBuilt;
    public static void EmitNewCharacterBuilt(CharacterData data)
    {
        OnNewCharacterBuilt?.Invoke(data);
    }

    public static event Action<CharacterData> OnCharacterLoaded;
    public static void EmitCharacterLoaded(CharacterData data)
    {
        OnCharacterLoaded?.Invoke(data);
    }

}