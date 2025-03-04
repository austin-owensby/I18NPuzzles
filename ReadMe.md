# I18N Puzzles
This repository is setup to assist with solving challenges from the [I18N Puzzles](https://i18n-puzzles.com/).

It allows you to easily run solutions in C#, submit answers and see the response, pull down your inputs, and run test inputs via a Web API or Console app described below.
This also includes some utilities to make some solutions easier. See the `Services/Utility.cs` file for more info.

The `main` branch contains a ready to use template to start your own solutions.
You may also reference the `aowensby-solutions` branch which contains my own solutions.

## Quick Start
1. Run the Program (See [Setup](#setup) below)
1. (Optional) Create a Cookie.txt file to enable puzzle input/submission (See [Puzzle Helper](#puzzle-helper) below)
1. (Optional) Make the `import-input-file` API call or use the equivalent in the console app to get your input for the day you're trying to solve (See [API](#post-apiimport-input-file) below)
1. Code your solution in one of the Service files
1. Make the `run-solution` API call or use the equivalent in the console app and optionally submit the solution (See [API](#get-apirun-solution) below)

## Setup
1. If not already installed, install [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)
1. Run the program
   - You have 2 options, either as a Web API with endpoints you can hit or a console app
   - If using Visual Studio or VSCode, use the play button to build and run the code after selecting your desired startup method
   - If using a CLI, run `dotnet run` from the repo's base folder
   - If using the console app, you will be prompted for inputs, you can bypass these by setting default values in the `Console/Program.cs` file.
1. Run API calls (Only needed if running the Wbe API project)
   - You can use [Swagger](https://swagger.io/), an API documentation and execution library, to execute the API calls, or use your own tool to call to the API
   - Visual Studio
      - The browser should open Swagger by default, to change this behavior, update the `Properties/launchSettings.json`
   - VSCode
      - Use the `Launch (web)` or `Launch (web - no browser)` configuration to toggle if you want the browser to open automatically
      - If you'd prefer to remain within VSCode to make the API calls, I've used the [Thunder Client](https://marketplace.visualstudio.com/items?itemName=rangav.vscode-thunder-client) extension. You can import the collection provided at `thunder_collection_I18NPuzzles.json`.
   - Other
      - Visit https://localhost:5001/swagger in your browser

## Puzzle Helper
This allows you to easily create the needed services as well as submitting answers.

In the `Shared/PuzzleHelper` folder, create a `Cookie.txt` file and add your own cookie that gets created when logging into the I18N Puzzles website. While on the I18N Puzzles website, if you open the Network tab in your browser's Dev Tools, you'll see the cookie in the header of API calls that are made while navigating the site. This typically expires after a month so you'll need to update it each year.

Ensure that the Cookie.txt is all 1 line.

If you would like to avoid this setup, you can always manually add you input and submit your solutions without having to create a Cookie.txt file.
The file is only required when interacting with the I18N Puzzles website.

### Automation
The Puzzle Helper does follow the automation guidelines on the [/r/adventofcode community wiki](https://www.reddit.com/r/adventofcode/wiki/faqs/automation) since I18N is inspired by it.

Specifically:
* Outbound calls are throttled to every 3 minutes in the I18NPuzzlesGateway's `ThrottleCall()` function
* Once inputs are downloaded, they are cached locally (PuzzleHelper's `WriteInputFile(int year, int day)` function) through the `api/import-input-file` endpoint described below.
* If you suspect your input is corrupted, you can get a fresh copy by deleting the old file and re-running the `api/import-input-file` endpoint.
* The User-Agent header in the Program.cs's gateway configuration is set to me since I maintain this tool :)

## API

### GET `api/run-solution`
- Query parameters
   - year (Ex. 2025) (Defaults to 2025)
   - day (Ex. 14) (Defaults to 1)
   - send (Ex. true) (Defaults to false) Submit the result to I18N Puzzles
   - example (Ex. true) (Defaults to false) Use an example file instead of the regular input, you must add the example at `Inputs/<YYYY>/<DD>_example.txt`
- Ex. `GET api/run-solution?year=2025&day=14&&send=true`

Runs a specific day's solution, and optionally posts the answer to I18N Puzzles and returns the result.

### POST `api/import-input-file`
- Query parameters
   - year (Ex. 2025) (Defaults to 2025)
   - day (Ex. 14) (Defaults to 1)
- Ex. `POST api/import-input-file?year=2025&day=14`

Imports the input from I18N Puzzles for a specific day.

The program is idempotent (You can run this multiple times as it will only add a file if it is needed.)

### POST `api/generate-service-files`

Creates missing daily solution service files.
Useful when a new year has started to preemptively generate the service files for the calendar year before the advent starts.

You'll likely only need to use this once per year and only if either your source code has gotten out of sync from the `main` branch or I haven't kept it up to date.

The program is idempotent (You can run this multiple times as it will only add files if they are needed.)

## Extra Notes
- The admin of I18N Puzzles has said that using this code for automation is OK:
> provided the automation is for personal use and you don't do more than a handful requests per day. ... I reserve the right to limit or block users that aren't treating the site gently.