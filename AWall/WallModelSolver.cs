using Google.OrTools.Sat;

namespace AWall;

internal class WallModelSolver
{

    public static bool SolveableFor(int n, out Wall? solvingWall, int MaxSecondsToSolve = 10)
    {
        CpSolver solver = new();
        var wallModel = WallModel.GenerateFor(n);
        solver.StringParameters = $"max_time_in_seconds:{MaxSecondsToSolve}.0";

        CpSolverStatus status = solver.Solve(wallModel.Model);

        var success = status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible;
        solvingWall = success ? WallFrom(wallModel, solver) : null;
        return success;
    }

    private static Wall WallFrom(WallModel wallModel, CpSolver solver)
    {
        var resultStones = wallModel.Stones.Select(row => row.Select(solver.Value).Select(v => (int)v));
        return Wall.From(resultStones);
    }

}
