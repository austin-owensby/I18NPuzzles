namespace I18NPuzzles.Services
{
    public interface ISolutionDayService
    {
        /// <summary>
        /// Execute this day's solution
        /// </summary>
        /// <param name="example"></param>
        /// <returns></returns>
        string RunSolution(bool example);
    }
}