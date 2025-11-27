using System;

[Serializable]
public class ClassOption
{
    // Localized name shown to the player (can contain non-English text)
    public string displayName;
    // Internal key used to lookup class data in the class database
    public string internalName;
}
