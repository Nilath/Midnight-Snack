﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Tao.Sdl;
#endregion

namespace Midnight_Snack
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameRunner : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Cursor cursor;
        Controls controls;
        Map map;
        GameManager gst;
        MiniMenu actionMenu;
        SleepingVillager villager;
        Text endText;
        Text turnText;
        Text goalText;

        SelectionScene levelSelectScene;

        /*
        //Tracks what state the game is in (i.e. main menu, gameplay, game over, etc.)
        int gameState;
        const int levelSelect = 0, mainGame = 1;
         * */

        public static int ScreenWidth;
        public static int ScreenHeight;

        public GameRunner()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScreenHeight = GraphicsDevice.Viewport.Height;

            IsMouseVisible = true;
            this.Window.Title = "Midnight Snack";

            //Create map
            map = new Map(4, 6, 0, 0);
            //Create test obstacles
            MapTile obstacle1 = map.GetTile(2, 1);
            obstacle1.SetPassable(false);
            map.SetTile(2, 1, obstacle1);
            MapTile obstacle2 = map.GetTile(1, 3);
            obstacle2.SetPassable(false);
            map.SetTile(1, 3, obstacle2);

            //Set up player stuff
            cursor = new Cursor(10, 10, 100, 100, map);
            player = Player.GetInstance();
            //player.SetMap(map);
            player.SetRow(map.GetLairRow());
            player.SetCol(map.GetLairCol());
            player.SetPosition(new Vector2(10, 10));
            
            //Set up villager stuff
            villager = new SleepingVillager(new Vector2(410, 210), 100, 100, 2, 4);
            //Mark villager tile as occupied
            MapTile villagerTile = map.GetTile(2, 4);
            villagerTile.SetOccupant(villager);
            map.SetTile(2, 4, villagerTile);

            gst = GameManager.GetInstance();

            actionMenu = new MiniMenu(player.GetPosition(), 70, 70);
            
            turnText = new Text("Turn: 1", new Vector2(700, 20));
            goalText = new Text("Goal: Get blood from villager and get back to start in 5 turns \nMove with arrow keys and select with space. Cancel out of an action with F", new Vector2(20, 420));
            endText = new Text("", new Vector2(700, 60));
            endText.SetVisible(false);

            //Level Select Screen
            Text startText = new Text("Select a Level", new Vector2(300, 250));
            List<Text> levelSelectText = new List<Text>();
            levelSelectText.Add(startText);
            List<Text> levelSelectOptions = new List<Text>();
            Text option1 = new Text("Tutorial", new Vector2(0, 0));
            Text option2 = new Text("Level 1", new Vector2(0, 0));
            levelSelectOptions.Add(option1);
            levelSelectOptions.Add(option2);
            Menu levelSelectMenu = new Menu(new Vector2(300, 300), 100, 100, levelSelectOptions);
            levelSelectScene = new SelectionScene(levelSelectText, levelSelectMenu);

            //Start the game on the level select screen
            //gameState = levelSelect;
            gst.SetGameState(0);

            base.Initialize();

            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            player.LoadContent(this.Content);
            villager.LoadContent(this.Content);
            map.LoadContent(this.Content);
            cursor.LoadContent(this.Content);
            actionMenu.LoadContent(this.Content);
            goalText.LoadContent(this.Content);
            endText.LoadContent(this.Content);
            turnText.LoadContent(this.Content);

            levelSelectScene.LoadContent(this.Content);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            controls.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScreenHeight = GraphicsDevice.Viewport.Height;

            switch(gst.GetGameState())
            {
                //Level Select Screen
                case 0:
                    levelSelectScene.Update(controls);
                    if(Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        //Set gameState to Main Game
                        //gameState = mainGame;
                        //gst.SetGameState(1);
                    }
                break;
                
                //Main Game Screen
                case 1:
                    //Update turn counter
                    turnText.SetMessage("Turn: " + gst.GetTurn());

                    if (!gst.IsInActionMenu())
                    {
                        actionMenu.SetVisible(false);
                        cursor.Update(controls);
                    }
                    else
                    {
                        actionMenu.SetVisible(true);
                        actionMenu.Update(controls);
                    }

                    //Update player win progression status
                    if (gst.GetTurn() > gst.GetTurnLimit())
                    {
                        gst.SetPlayerAlive(false);
                    }
                    if (player.HasBlood() && player.GetRow() == map.GetLairRow() && player.GetCol() == map.GetLairCol())
                    {
                        gst.SetWon(true);
                    }
                    //Check if player has lost
                    if (!gst.IsPlayerAlive())
                    {
                        endText.SetMessage("You Lose!");
                        endText.SetVisible(true);
                    }
                    //Check if player has won
                    else if (gst.HasWon())
                    {
                        endText.SetMessage("You Win!");
                        endText.SetVisible(true);
                    }
                break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            switch (gst.GetGameState())
            {
                //Level Select Screen
                case 0:
                    levelSelectScene.Draw(spriteBatch);
                    //startText.Draw(spriteBatch);
                break;

                //Main Game Screen
                case 1:
                    map.Draw(spriteBatch);
                    cursor.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    villager.Draw(spriteBatch);
                    actionMenu.Draw(spriteBatch);
                    endText.Draw(spriteBatch);
                    turnText.Draw(spriteBatch);
                    goalText.Draw(spriteBatch);
                break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
