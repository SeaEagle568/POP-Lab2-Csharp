using System;
using System.Collections.Generic;
using System.Threading;

public class ArrayMinFinder
{
    private readonly int threadCount;
    private readonly List<int> array;
    private readonly MinHolder result = new();

    public ArrayMinFinder(int threadCount, int arraySize)
    {
        this.threadCount = threadCount;
        this.array = GenerateArray(arraySize);
    }

    public int FindMin()
    {
        Thread[] threads = StartThreads();
        try
        {
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
        catch (ThreadInterruptedException)
        {
            Console.Error.WriteLine("ArrayMinFinder was rudely interrupted while waiting for result!");
            Thread.CurrentThread.Interrupt();
            throw new InvalidOperationException("Unable to find minimum - interrupted");
        }
        return result.Get();
    }

    private List<int> GenerateArray(int size)
    {
        List<int> result = new(size);
        Random random = new();
        for (int i = 0; i < size; i++)
        {
            int part = FindPart(size, i);
            result.Add(random.Next(part, 1000)); 
        }
        int minPos = random.Next(size);
        result[minPos] = -1;
        Console.WriteLine($"Finished generation of random array of size {size}, with minimum hidden at position: {minPos}");
        return result;
    }

    private int FindPart(int size, int i)
    {
        return (i < size % threadCount * size / threadCount)
                ? i / (size / threadCount + 1)
                : (i - size % threadCount * (size / threadCount + 1)) / (size / threadCount) + (size % threadCount);
    }

    private Thread[] StartThreads()
    {
        Thread[] threads = new Thread[threadCount];
        int blockSz = array.Count / threadCount;
        int remainder = array.Count % threadCount;
        int start = 0;
        for (int i = 0; i < threadCount; i++)
        {
            int fin = start + blockSz + (remainder > 0 ? 1 : 0);
            var calc = new Calculator(start, fin, array, result);
            threads[i] = new Thread(new ThreadStart(calc.Run))
            {
                Name = "Calculator " + i
            };
            start = fin;
            remainder--;
        }
        Console.WriteLine($"Starting {threadCount} threads with uniformly distributed segments:\n");
        foreach (var thread in threads)
        {
            thread.Start();
        }
        return threads;
    }

    private class Calculator(int from, int to, List<int> array, MinHolder result)
    {
        private readonly int from = from;
        private readonly int to = to;
        private readonly List<int> array = array;
        private readonly MinHolder result = result;

        public void Run()
        {
            int min = array[from];
            for (int i = from; i < to; i++)
            {
                min = Math.Min(min, array[i]);
            }
            Console.WriteLine($"{Thread.CurrentThread.Name} finished on segment [{from} - {to}] with minimum: {min}");
            result.Update(min);
        }
    }
}
