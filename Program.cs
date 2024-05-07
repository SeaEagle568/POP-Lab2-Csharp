using System;

class Program
{
    static void Main(string[] args)
    {
        int arraySize = 10_000_000;
        int threadCount = 10;
        if (args.Length >= 1)
        {
            threadCount = Validate(args[0], 32, 10);
        }
        if (args.Length >= 2)
        {
            arraySize = Validate(args[1], 100_000_000, 10_000_000);
        }
        int min = new ArrayMinFinder(threadCount, arraySize).FindMin();
        Console.WriteLine("Execution completed, array minimum: " + min);
    }

    private static int Validate(string arg, int max, int def)
    {
        if (int.TryParse(arg, out int res) && res > 0 && res <= max)
        {
            return res;
        }
        else
        {
            Console.Error.WriteLine($"Unable to parse argument. Integer [1-{max}] expected");
            return def;
        }
    }
}
