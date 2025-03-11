namespace I18NPuzzles.Services
{
    public static class FileUtility
    {
        # region File Processing
        /// <summary>
        /// Get the input file line by line
        /// </summary>
        /// <param name="day"></param>
        /// <param name="example">Defaults to false, if true will pull an example file you've manually added</param>
        /// <remarks>Ex. GetInputLines(2) reads the data from "/Inputs/02.txt". GetInputLines(5, true) reads the data from "/Inputs/05_example.txt"</remarks>
        /// <returns></returns>
        public static List<string> GetInputLines(int day, bool example = false)
        {
            string directoryPath = Directory.GetParent(Environment.CurrentDirectory)!.FullName;
            string filePath = Path.Combine(directoryPath, "Inputs", $"{day:D2}{(example ? "_example" : string.Empty)}.txt");
            return File.ReadAllLines(filePath).ToList();
        }
        #endregion
    }
}