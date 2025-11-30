using System.Collections.Generic;

public interface IHasPriority {
    float Priority { get; }
}

public class PriorityQueue<T> where T : IHasPriority
{
    private readonly List<T> items = new List<T>();

    public int Count => items.Count;

    public void Enqueue(T item)
    {
        items.Add(item);
        items.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }

    public T Dequeue()
    {
        T item = items[0];
        items.RemoveAt(0);
        return item;
    }

    public void UpdatePriority(T item)
    {
        items.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }
}
