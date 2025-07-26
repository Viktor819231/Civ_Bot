A.CIV_BOT: What my goals are functionalities BOT
1. Recognize where it is at in civ 5
2. Navigate Civ 5 And host lobbies
3. Post comments on loop
4. Post comments when new people join lobby
5. Give respose based on user input "A", "B" etc
6. Run in loop
7. Move window so pixels match up with AHK
extra.1 Get information from schedual bot and display
extra.2 Get infromation from 

A.BOT What is expected from user of bot:
1. Open civ 5. change aspect ratio to smallest 1024x768
2. Start bot. dont touch screen when active 


B.ToDo

1. SS_Navigation (REFACTOR TO USE TEMPLATE MATCH NOT OCR)
    11. Loading (if rest fail: wait)
    12. Main_menu (if second slot sais "Multiplayer" LOCATION 1:Menu ) 
    13. Multiplayer_menu 1 (if second slot  "Hot Seat" LOCATION 1:Menu) 
    14. Multiplayer_menu 2 (if second slot  "Local Network" LOCATION 1:Menu) 
    15. JoinLobbiesscreen (if header "INTERNET GAMES" LOCATION 2:Header)
    16. Lobbysetupscreen (If header "SETUP MULTIPLAYER GAME" LOCATION 2:Header)
    17. Loadgamesreen (if header "LOAD GAME" LOCATION 2:Header)
    18. Staging (Header: "STAGING ROOM" LOCATION 2:Header)

2. Actions : AutoHotkey scripts for every type of action
    21. Starscreen-> Press multiplayer
    22. Multiplayer screen -> Press standard
    23. Standard Screen -> Press Internet (same as 22)
    24. Lobby screen -> Press Host Game
    25. Host Screen -> Press Load Game
    26. LoadGame screen -> Press Game config lobby
    27. LoadGame screen -> Press Load Game (after 26)
    28. Lobby actions 1 : press chat input box
    29. Lobby action 2: Post Msg basic info 1
    30. Lobby action 3: Post Msg basic info 2
    31. (Low Priority) Lobby action 4: Respond to msg
3. Make premade msgs for AHK
4. Make the main loop
5. Extra: Adapt to screen/find and adjust pixles


B.WEBSITE Functionalieties Goals:
1.Intro: What is Democraciv
    Multiplayer No Quit
    Rules enforced Courthouse
    Display Leaderboard
    Display Upcoming games information
    Previous games information
    Video guide: How to Install bot
    Video guide: How to Join games
    Video guide: How to Schedual games
2. Rules information
3. How to join discord server

B.WEBSITE design Goals:
1. Clean and scrollable. mostly one page
    desgign similar to https://jp.marugame.com/en/
2. Have statue of liberty icon as background,
   alternativly gif of gameplay to make it feel alive

C. Discord Schedualar:
1. Pick date and time
2. Signup by going to game and press signup
    dispay intrest without joining
3. Dislay upcoming games in calander?
4. waiting list
5. notifications (optional?)
    when someone signs up for your game
    when someone drops out
6. punishment for not joining games that you signed up for
7. mark availablity?

D.Extra ideas and requirments
1. Communiation between the different programs
    schdular information API
2. discord scraper
    leaderboard, last games
3. Make bot downloadable and easy to setup
    include tessaract and AHK if someone in future want to continue and run it





