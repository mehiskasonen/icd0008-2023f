UNO card game with hard coded AI that was part of a course project in Taltech C# course.

Toggle GameRepositoryFileSystem or GameRepositoryEF in Program.cs to manually change if you want to
play in console or in the web.


## Running the App with Docker

1. Clone the repository:
   ```sh
   git clone https://github.com/mehiskasonen/icd0008-2023f.git
   cd yourrepository

2. Build the docker image:
    docker build -t uno:latest .

3. Run the docker container:
    docker run -p 8080:80 yourappname:latest
4. Access the app at http://localhost:8080


TODO:
~~1. Fix how the discard pile is visualized.~~
2. Refactor Game.cs so that GameController is used for the main loop instead.
~~3. Fix AI, so that it doesn't keep drawing cards in a loop.~~
~~4. Fix toString method to represent Wild cards.~~
~~5. Refactor program.cs to not repeat deck creation.~~
6. Make the menu system use levels to implement 'back' button.
7. Fix color bleeding issues.
8. Fix quit->resume->discardPile last card duplication cycle.
9. Fix need to double save for saving to work.

Mängu ID -> db ask game-> view.
Draw page for user. Update get for user ID.
PlayerID, MänguID, kaardiID. State has which player turn it is.
Küsid mängu ja saad mängu state-i.
Statiga initialise gameEngine.

GameEngine.makeMove(CardID)

If user cant play - throw error OR wrap message in a response. 

Käik->State to _db -> draw new state.


## EF 

~~~bash
dotnet tool update --global dotnet-ef
dotnet ef migrations add --project DAL --startup-project ConsoleApp1 initialCreate
dotnet ef migrations add --project DAL --startup-project WebApp initialCreate

~~~

~~~bash
dotnet ef migrations --project DALEF --startup-project WebApp add initial
dotnet ef database update --project DALEF --startup-project WebApp
~~~

## WebPage
***NB! Cd to web app directory for pages scaffolding. Input has to be one line***

~~~bash
dotnet aspnet-codegenerator razorpage \
    -m Person \
    -dc AppDbContext \
    -udl \
    -outDir Pages/Persons \
    --referenceScriptLibraries
    
dotnet aspnet-codegenerator razorpage \
-m Contact \
-dc AppDbContext \
-udl \
-outDir Pages/Contacts \
--referenceScriptLibraries
~~~


## WebPage
~~~bash
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet tool update --global dotnet-aspnet-codegenerator
dotnet aspnet-codegenerator razorpage \
    -m Domain.Database.Game \
    -dc AppDbContext \
    -udl \
    -outDir Pages/Games \
    --referenceScriptLibraries
    
dotnet aspnet-codegenerator razorpage \
    -m Domain.Database.Player \
    -dc AppDbContext \
    -udl \
    -outDir Pages/Players \
    --referenceScriptLibraries
~~~