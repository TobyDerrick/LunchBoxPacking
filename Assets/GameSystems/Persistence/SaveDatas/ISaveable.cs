using UnityEngine;

public interface ISaveable
{
    string UniqueID { get; }
    SaveData CaptureState();
    void RestoreState(SaveData state);
}

