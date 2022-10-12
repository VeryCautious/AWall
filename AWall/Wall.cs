namespace AWall;

internal class Wall
{
    public int[][] Stones { get; }

    private Wall(int[][] stones)
    {
        Stones = stones;
    }

    public static Wall From(IEnumerable<IEnumerable<int>> seed) => new(seed.Select(row => row.ToArray()).ToArray());

    public bool IsValid()
    {
        var gaps = new HashSet<int>();
        for (int u = 0; u < Stones.Length; u++)
        {
            var sum = 0;
            for (int i = 0; i < Stones[u].Length - 1; i++)
            {
                sum += Stones[u][i];

                if (gaps.Contains(sum))
                    return false;
                gaps.Add(sum);
            }
        }
        return true;
    }

    public override string? ToString()
    {
        var rowStrings = Stones.Select(nbrs => string.Join(",", nbrs));
        var decoration = new string(Enumerable.Repeat('-', rowStrings.First().Length).ToArray());
        return string.Join(
            "\n",
            rowStrings.Append(decoration).Prepend(decoration)
        );
    }
}