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


ToDO V1 ship: 
Fixa simpel script för en loop, 
lägg till event listner för escapekey
kolla efter escape key på logiska ställen i koden
    paketera den i bot scripts

ToDO V1.5
Simpel Winform Frontend: starta bot, display vad den gör
Ändra olika Botactions till olika implemewntationer av en botaction interface så vi kan kalla dem i en array av bot actions 

ToDo V2
1. Jsonserilazation class
        test the json class by initialize program
        deserilize with code from txtfile
            try code on again 
            delete data from Localization data
2. Refactor bot commands to be a interface with different functions so i can call an array of actions.
3. Fullfledge Winforms app
4. Could easily add "new" actions 


Todo V3 and beyond:
Why stop at civ. Could use a lot of this code/lessons to make something that act with bot scripts based on OCR reading text. Automatic script making by eventlistners, ocr checkpoints. Profiles etc. share scripts by json files easily with other people. add pixel color identification



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


