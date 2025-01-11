# MTCG (Monster Trading Card Game)

> [!NOTE]  
> This project is still under development

**MTCG** is a university project built in C#. It focuses on creating a Monster Trading Card Game where players engage in card-based battles. The project is designed with a layered architecture, emphasizing separation of concerns between data access, business logic, and user interaction.

## Features

- **Card Types**: Monster and Spell cards with unique attributes and abilities.
- **Battle System**: Execute card battles with strategic gameplay and damage calculations.
- **AI-Battle System**: A Single Player mode is also available.
- **Trading System**: Player can trade cards with other players.
- **Shop System**: Player can buy items like mystery packs from the shop.
- **Achievement System**: Player can unlock achievements to get rewards.
- **User Management**: Player registration, login, and account management.

## Project Structure

- **BusinessLayer**: Contains the core game logic, including controllers, interfaces, and models for cards, users, and battles.
- **DAL (DatabaseAccessLayer)**: Handles all data-related operations such as saving and retrieving user and card information from the database.
- **PresentationLayer**: This is the REST Server with defined endpoints that handle requests and responses.

## Project Setup
Open the MTCG Solution in Visual Studio. After Database Setup you should be ready to go and run in Visual Studio

## Database Setup
If you want to do CURL Scripts to them before using the API because you have to reset the database
Open a terminal and go into the **dbConfig** folder.
Then run **docker compose up**.

## CURL Scripts
It is possible to test the API with CURL scripts. Here is an instruction:
1. Create the Database and remove the old one if there is one
2. Start the API and the Database
3. Run **MTCG_authFeature.curl.bat** file first
4. Then run **MTCG_user_cards_shop.curl.bat** file
5. After that run **MTCG_battle1.curl.bat** and when the scripts stops it waits for another player so open up a second terminal and run **MTCG_battle2.curl.bat**
6. The last script is **MTCG_trading.curl.bat**

## UNIT Tests
There are also some Unit Tests which can be run in Visual Studio.


## License

This project is for educational purposes only

