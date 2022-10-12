using System.Diagnostics;
using AWall;

const int maxSeconds = 60;
Stopwatch s = new();
int n = 2;

while (true)
{
    Console.WriteLine($"Building wall with n={n}...");

    s.Restart();
    if (!WallModelSolver.SolveableFor(n, out var wall, maxSeconds))
    {
        Console.WriteLine($"Could not find a solution for n={n} in under {maxSeconds}sec :(");
        break;
    }
    s.Stop();

    if (wall != null && !wall.IsValid())
        throw new Exception("The result is not correct!");

    Console.WriteLine(wall);
    Console.WriteLine($"Solved in {s.ElapsedMilliseconds}ms");
    Console.WriteLine();
    Console.WriteLine();

    n++;
}