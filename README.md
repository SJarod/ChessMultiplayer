# TCP Network in a Chess Game
by Lancelot Bernard-Lepine and Jarod Sengkeo

# Presentation
This project is meant to make a multiplayer Chess Game, using the singleplayer Chess Game template provided by Florian Wolf.

# Getting started
First clone the repository [git@gitlabstudents.isartintra.com:projets/2022_gp_2025_rts_ai_gp_2025_chessmulti-groupe_04.git](git@gitlabstudents.isartintra.com:projets/2022_gp_2025_rts_ai_gp_2025_chessmulti-groupe_04.git).
Then, the project should be opened with Unity 2021.3.5f1.
The project contains 2 Unity game scenes :
- Menu.unity
    - The first scene to load when launching the game
    - This scene is used to choose the game profile (server or client)
- ChessGame.unity
    - The game itself
    - The user may not be able to test in singleplayer

# Play chess with a friend on LAN
To play with a friend, the users must be connected to the same network. Both of them can launch the game and play as white or black. To play as white, the player must be hosting the game by clicking the "Host" button. The one that clicks "Join" will have to enter the host's local ip address (cmd.exe → ipconfig → IPv4 address) in order to connect.

# Implemented features
- Creation of a TCP server
    - Sending messages to every clients
    - Receiving messages from a client then sending it to the other one
- Creation of a TCP client
    - Sending messages to the server
- Serialization
    - Converting custom classes to byte arrays
- Packing messages with a header
- Server/Client communication process :
    - When 2 clients have joined the hosting socket, the server sends a string (the name of the scene to load : ChessGame) to both of the clients, then it sends a boolean to tell which is the first player
    - If a client can play, the PlayTurn() function in the ChessGameMgr sends the Move to the server. The server then sends to the other client
    - When the client receives a message (most likely a Move), it updates the board according to the receved Move.

# Problems/Bugs
- Sometimes, when closing the application (either server or client), socket.EndReceive() causes a segmentation fault.

# Future implementations
- Replace non-blocking socket.Accept() with an asynchronous accept (BeginAccept() or AcceptAsync())