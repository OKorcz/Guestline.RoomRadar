# Configuration
`bookings.json` and `hotels.json` have to be placed inside Data folder before building the project or after building the project those files can be updated.

Application is also configured using `config.json` file which contains paths to bookings and hotels data files (*json*). `config.json` is loaded only once when config is used for the first time.

# Usage
Navigate to root project folder ***Guestline.Roomradar*** (This folder contains this *readme.md*) and run command `dotnet run --project ./src`.

# Tests
As above but run command `dotnet test`.