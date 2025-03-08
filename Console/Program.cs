using I18NPuzzles;
using I18NPuzzles.Console.Controllers;

// Set any of these values to default them and bypass the console interface
int? mode = null;
int? day = null;
bool? send = null;
bool? example = null;

while (mode == null) {
    Console.Clear();
    System.Console.WriteLine("Select a mode:");
    System.Console.WriteLine("1) Run Solution");
    System.Console.WriteLine("2) Import Input");

    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input)) {
        continue;
    }

    if(int.TryParse(input, out int modeValue)) {
        if (modeValue < 1 || modeValue > 2) {
            continue;
        }
        mode = modeValue;
    }
}

if (mode == 1 || mode == 2) {
    while (day == null) {
        Console.Clear();
        System.Console.WriteLine($"Enter a day (1-{Globals.NUMBER_OF_PUZZLES}):");

        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input)) {
            continue;
        }

        if(int.TryParse(input, out int dayValue)) {
            if (dayValue < 1 || dayValue > Globals.NUMBER_OF_PUZZLES) {
                continue;
            }
            day = dayValue;
        }
    }
}

if (mode == 1) {
    while (example == null) {
        Console.Clear();
        System.Console.WriteLine("Run example? (Y/N):");

        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input)) {
            continue;
        }

        if(input.ToLower() == "y") {
            example = true;
        }
        else if (input.ToLower() == "n") {
            example = false;
        }
    }

    while (send == null) {
        Console.Clear();
        System.Console.WriteLine("Submit results? (Y/N):");

        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input)) {
            continue;
        }

        if(input.ToLower() == "y") {
            send = true;
        }
        else if (input.ToLower() == "n") {
            send = false;
        }
    }
}

Console.Clear();

Controller controller = new();

if (mode == 1) {
    await controller.GetSolution(day!.Value, send!.Value, example!.Value);
}
else if (mode == 2) {
    await controller.ImportInputFile(day!.Value);
}
else {
    System.Console.WriteLine($"Unknown mode: {mode}");
}