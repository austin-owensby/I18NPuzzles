using System.Text;

namespace I18NPuzzles.Services
{
    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/12.txt
    public class Solution12Service : ISolutionDayService
    {
        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(12, example);
            List<(string name, int number)> phonebook = lines.Select(l => l.Split(": ")).Select(l => (l[0], int.Parse(l[1]))).ToList();
            int midpoint = (phonebook.Count - 1) / 2;

            List<(string name, int number)> englishPhonebook = phonebook.Select(line => (new string(line.name.Replace("æ", "ae").Replace("Æ", "AE").Normalize(NormalizationForm.FormD).Where(c => char.IsLetter(c) || c == ',').ToArray()), line.number)).ToList();
            englishPhonebook = englishPhonebook.OrderBy(l => l.name).Select(p => (phonebook.First(pb => pb.number == p.number).name, p.number)).ToList();
            long englishMidpoint = englishPhonebook[midpoint].number;

            List<(string name, int number)> swedishPhonebook = phonebook.Select(line => (new string(line.name.Replace("æ", "ä").Replace("Æ", "Ä").Replace("Ø", "Ö").Replace("ø", "ö").Select(c => char.IsAscii(c) || c == 'ä' || c == 'Ä' || c == 'Ö' || c == 'ö' || c == 'Å' || c == 'å' ? c : $"{c}".Normalize(NormalizationForm.FormD)[0]).Where(c => char.IsLetter(c) || c == ',').ToArray()), line.number)).ToList();
            swedishPhonebook = swedishPhonebook.Select(p => (p.name.Replace("å", "zza").Replace("Å", "ZZa").Replace("ä", "zzb").Replace("Ä", "ZZB").Replace("ö", "zzc").Replace("Ö", "ZZC"), p.number)).OrderBy(l => l.Item1).Select(p => (phonebook.First(pb => pb.number == p.number).name, p.number)).ToList();
            long swedishMidpoint = swedishPhonebook[midpoint].number;

            List<(string name, int number)> dutchPhonebook = phonebook.Select(line => (new string(line.name.SkipWhile(c => !char.IsUpper(c)).Where(c => char.IsLetter(c) || c == ',').ToArray()), line.number)).ToList();
            dutchPhonebook = dutchPhonebook.OrderBy(l => l.name).Select(p => (phonebook.First(pb => pb.number == p.number).name, p.number)).ToList();
            long dutchMidpoint = dutchPhonebook[midpoint].number;

            long answer = englishMidpoint * swedishMidpoint * dutchMidpoint;

            return answer.ToString();
        }
    }
}