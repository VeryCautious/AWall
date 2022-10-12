using Google.OrTools.Sat;

namespace AWall;

internal class WallModel
{
    /// <summary>
    /// The maximum ammount of row you can build with n stones per row
    /// </summary>
    private readonly int MaxRows;
    private readonly int N;
    /// <summary>
    /// Gaps[r][i] contains the index of the last gap that is made from the first i+1 stones in row r
    /// </summary>
    private readonly IntVar[][] Gaps;
    /// <summary>
    /// Stones[r][i] contains the size of stone i in row r (zerobased)
    /// </summary>
    public readonly IntVar[][] Stones;

    public readonly CpModel Model;

    public static WallModel GenerateFor(int n)
    {
        return new(n);
    }

    public WallModel(int n)
    {
        N = n;
        MaxRows = (int)Math.Floor(n / 2.0f) + 1;

        Stones = new IntVar[MaxRows][];
        Gaps = new IntVar[MaxRows][];

        Model = new();

        Declare_StoneVariables();
        Add_AllDifferntStonesPerRow_Constraints();

        Declare_GapVariables();
        Bind_SubsetSummsToGaps();
        Declare_AllDifferentGaps_Constraint();

        Add_Normalization_Constraint();

        Model.AddDecisionStrategy(
            Stones.SelectMany(row => row),
            DecisionStrategyProto.Types.VariableSelectionStrategy.ChooseMaxDomainSize,
            DecisionStrategyProto.Types.DomainReductionStrategy.SelectMedianValue
        );
    }

    //Only for performance
    private void Add_Normalization_Constraint()
    {
        for (int u = 0; u < MaxRows; u++)
        {
            for (int i = u + 1; i < MaxRows; i++)
            {
                Model.Add(Stones[u][0] < Stones[i][0]);
            }
        }
    }

    private void Declare_StoneVariables()
    {
        for (int u = 0; u < MaxRows; u++)
        {
            Stones[u] = new IntVar[N];
            for (int i = 0; i < N; i++)
            {
                Stones[u][i] = Model.NewIntVar(1, N, $"s{u}{i}");
            }
        }
    }

    private void Declare_AllDifferentGaps_Constraint()
    {
        Model.AddAllDifferent(Gaps.SelectMany(row => row));
    }

    private void Declare_GapVariables()
    {
        var MaxSum = ((N + 1) * N) / 2;
        for (int u = 0; u < MaxRows; u++)
        {
            Gaps[u] = new IntVar[N - 1];
            for (int i = 0; i < N - 1; i++)
            {
                Gaps[u][i] = Model.NewIntVar(1, MaxSum, $"g{u}{i}");
            }
        }
    }

    private void Bind_SubsetSummsToGaps()
    {
        for (int u = 0; u < Stones.Length; u++)
        {
            for (int i = 0; i < Stones[u].Length - 1; i++)
            {
                var subsetSum = LinearExpr.Sum(Stones[u].Take(i + 1));
                Model.Add(Gaps[u][i] == subsetSum);
            }
        }
    }

    private void Add_AllDifferntStonesPerRow_Constraints()
    {
        for (int u = 0; u < Stones.Length; u++)
        {
            Model.AddAllDifferent(Stones[u]);
        }
    }

}
