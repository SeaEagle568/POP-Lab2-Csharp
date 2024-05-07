using System;

public class MinHolder
{
    private int min;
    private readonly object locker = new();

    public MinHolder()
    {
        this.min = int.MaxValue;
    }

    public int Get()
    {
        return min;
    }

    public void Update(int val)
    {
        lock (locker)
        {
            min = Math.Min(min, val);
        }
    }
}
