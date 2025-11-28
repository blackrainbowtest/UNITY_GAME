using System;

[Serializable]
public class ActivityData
{
    public string id;
    public string category;
}

[Serializable]
public class ActivityDataCollection
{
    public ActivityData[] activities;
}
