using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DrawMaMon.Structs;
using System.IO;


namespace DrawMaMon
{
    //Bezig met tilemaps sinchronizeren 
    //stuurt identifier bytes en moet die vergelijken met identifier op server (voor een filecompare)
    //font moet verbeterd worden
    //newgame voor multiplayer moet serverSide ,client download map
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont GameFont;

        GameStateController gameState;
        bool OnlineMode = false;
 
        Camera cam;
        Hero hero;
        GameFiles gameFiles;
        Network network;

        MapEditor mapEditor;
        MainMenu mainMenu;
        EscapeMenu escapeMenu;
      
        Size numTilesScreen;
        Size tileScreenSize;
        Size windowSize; 
        Point screenOffset;

        KeyboardState prevKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            prevKeyboardState = Keyboard.GetState();
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            windowSize = new Size(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            numTilesScreen = new Size(0, 17);
            numTilesScreen.Width = (int)(numTilesScreen.Height * GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio);

            tileScreenSize = new Size(graphics.PreferredBackBufferWidth / numTilesScreen.Width, graphics.PreferredBackBufferHeight / numTilesScreen.Height);

            if (numTilesScreen.Width % 2 == 0)
                numTilesScreen.Width--;

            screenOffset = new Point((int)((windowSize.Width - tileScreenSize.Width * numTilesScreen.Width) / 2), (int)((windowSize.Height - tileScreenSize.Height * numTilesScreen.Height) / 2));

            this.Window.Title = "Drawmamon";
            graphics.ApplyChanges();
            graphics.ToggleFullScreen();
            this.IsMouseVisible = true;

            gameState = new GameStateController(GameState.MainMenu);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\DrawMamon";
            gameFiles = new GameFiles(Content.Load<Texture2D>("Textures\\EmptyTile"), windowSize, numTilesScreen, filePath);

            GameFont = Content.Load<SpriteFont>("Fonts\\Font");

            mainMenu = new MainMenu(Content.Load<Texture2D>("Textures\\UI\\MapEditor\\ToolBar"),
                Content.Load<Texture2D>("Textures\\UI\\EscapeMenu\\MenUKnop Blauw"),
                Content.Load<Texture2D>("Textures\\UI\\EscapeMenu\\MenUKnop Blauw"),
                Content.Load<Texture2D>("Textures\\UI\\MainMenu\\Menuknop play"),
                Content.Load<Texture2D>("Textures\\UI\\General\\textBoxBG"),
                Content.Load<Texture2D>("Textures\\UI\\General\\textBoxBG"),
                Content.Load<Texture2D>("Textures\\UI\\General\\SelectedBox"),
                Content.Load<Texture2D>("Textures\\UI\\General\\VScrollbarBG"),
                Content.Load<Texture2D>("Textures\\UI\\General\\VScrollbarButton"),
                Content.Load<Texture2D>("Textures\\UI\\MainMenu\\Menuknop play"),
                Content.Load<Texture2D>("Textures\\UI\\EscapeMenu\\MenUKnop Blauw"),
                Content.Load<Texture2D>("Textures\\UI\\General\\File"),
                windowSize, screenOffset, GameFont, gameFiles.GetSaveFolder(),gameFiles.GetWorldFolder());
            mainMenu.NewGameEvent += new NewGameHandler(NewGame);
            mainMenu.LoadGameEvent += new LoadGameHandler(LoadGame);
            mainMenu.JoinGameEvent += new JoinGameHandler(JoinGame);
          

            escapeMenu = new EscapeMenu(Content.Load<Texture2D>("Textures\\UI\\EscapeMenu\\Menuknop BLAUW SAVE"),
                Content.Load<Texture2D>("Textures\\UI\\EscapeMenu\\Menuknop BLAUW EXIT"),
                windowSize, screenOffset, GameFont);
            escapeMenu.ExitEvent += new ExitEventHandler(ExitGame);
            escapeMenu.SaveEvent += new SaveEventHandler(SaveGame);


            hero = new Hero();
            hero.player = new Player(new Point(160, 160), Content.Load<Texture2D>("Textures\\Boer16x32Animatie"), new Size(16, 32), gameFiles.GetTileMaps().TilePixSize());
            cam = new Camera(hero.player.GetPosition());
            hero.playerController = new PlayerController(gameFiles.GetTileMaps().TilePixSize(), 2);

            network = new Network(Content.Load<Texture2D>("Textures\\Boer16x32Animatie"), hero, gameFiles.GetTileMaps().TilePixSize(),gameFiles);
            network.ConnectionMadeEvent += new ConnectionMadeHandler(ConnectionMade);
            network.ConnectionLostEvent += new ConnectionLostHandler(LostConnection);
            network.AddTileMapEvent += new AddTileMapHandler(AddTileMap);

            mapEditor = new MapEditor(Content.Load<Texture2D>("Textures\\UI\\MapEditor\\ToolBar"), 
                Content.Load<Texture2D>("Textures\\UI\\MapEditor\\FloorLayerWhite"), 
                Content.Load<Texture2D>("Textures\\UI\\MapEditor\\RoofLayerWhite"), 
                Content.Load<Texture2D>("Textures\\UI\\MapEditor\\TileBoxbg"), 
                Content.Load<Texture2D>("Textures\\UI\\MapEditor\\PijlLinks"), 
                Content.Load<Texture2D>("Textures\\UI\\MapEditor\\PijlRechts"), 
                Content.Load<Texture2D>("Textures\\UI\\MapEditor\\OpenMap"),
                Content.Load<Texture2D>("Textures\\UI\\MapEditor\\Prullebak"),
                Content.Load<Texture2D>("Textures\\UI\\General\\OpenFileBG"),
                Content.Load<Texture2D>("Textures\\UI\\General\\closeButton"),
                Content.Load<Texture2D>("Textures\\UI\\General\\Map"),
                Content.Load<Texture2D>("Textures\\UI\\General\\File"),
                Content.Load<Texture2D>("Textures\\UI\\General\\SluitMap"),
                Content.Load<Texture2D>("Textures\\UI\\General\\HScrollbarBG"),
                Content.Load<Texture2D>("Textures\\UI\\General\\HScrollbarButton"),
                Content.Load<Texture2D>("Textures\\UI\\General\\VScrollbarBG"),
                Content.Load<Texture2D>("Textures\\UI\\General\\VScrollbarButton"),
                Content.Load<SpriteFont>("Fonts\\Font"),
                Content.Load<SpriteFont>("Fonts\\Font"), graphics,gameFiles.GetTileMaps(), 
                network ,windowSize, screenOffset);
            mapEditor.FinishedEvent += new FinishedHandler(closeMapEditor);

        }

        protected override void UnloadContent()
        {
            
        }

        void NewGame(string saveName,string worldName)
        {
            gameFiles.NewGame(graphics.GraphicsDevice,numTilesScreen,mapEditor.GetLayerNumber(),new Size(300,300),saveName,worldName);
            gameState.SetGameState(GameState.Play);
            this.IsMouseVisible = false;
        }

        void ExitGame()
        {
            if (network.IsConnected())
                network.CleanUp();
            
            this.Exit();
        }

        void LoadGame(string saveName)
        {
            gameFiles.LoadGame(graphics.GraphicsDevice,hero.player ,numTilesScreen, mapEditor.GetLayerNumber(), saveName);
            cam.MoveCamera(hero.player.GetPosition(), gameFiles.GetTileMaps().TilePixSize(), numTilesScreen.Width,numTilesScreen.Height, gameFiles.GetMap().Width(), gameFiles.GetMap().Height());
            gameState.SetGameState(GameState.Play);
            this.IsMouseVisible = false;
            mapEditor.Refresh();
        }

        void JoinGame(string ipAdress)
        {
            OnlineMode = true;
            network.Connect(ipAdress);
        }

        void ConnectionMade(string worldName)
        {
            gameFiles.NewGame(graphics.GraphicsDevice, numTilesScreen, mapEditor.GetLayerNumber(), new Size(300, 300), worldName, worldName);
            gameState.SetGameState(GameState.Play);
            this.IsMouseVisible = false;
        }

        void AddTileMap(string tilMapPath)
        {
            Stream iStream = new FileStream(tilMapPath, FileMode.Open);
            Texture2D tileMap = Texture2D.FromStream(graphics.GraphicsDevice, iStream);
            iStream.Close();
            gameFiles.GetTileMaps().AddTileMap(tileMap);
            mapEditor.Refresh();
        }
        
        void LostConnection()
        {
            System.Windows.Forms.MessageBox.Show("Lost Connection");
        }

        void SaveGame()
        {
            gameState.SetGameState(GameState.Play);
            this.IsMouseVisible = false;
            gameFiles.SaveWorld(mapEditor.GetLayerNumber());
            gameFiles.SaveGame(hero.player);
        }

        void closeMapEditor()
        {
            gameState.SetGameState(GameState.Play);
            cam.MoveCamera(hero.player.GetPosition(), gameFiles.GetTileMaps().TilePixSize(), numTilesScreen.Width, numTilesScreen.Height, gameFiles.GetMap().Width(), gameFiles.GetMap().Height());
            this.IsMouseVisible = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !prevKeyboardState.IsKeyDown(Keys.Escape))
            {
                if (gameState.GetGameState() == GameState.Play)
                {
                    gameState.SetGameState(GameState.EscapeMenu);
                    this.IsMouseVisible = true;
                }
                else if (gameState.GetGameState() == GameState.EscapeMenu)
                {
                    gameState.SetGameState(GameState.Play);
                    this.IsMouseVisible = false;
                }
            }

            if (gameState.GetGameState() == GameState.Play)
            {
                int tilePixSize = gameFiles.GetTileMaps().TilePixSize();
                hero.player.Update(tilePixSize, numTilesScreen.Width, numTilesScreen.Height, gameFiles.GetMap().Width(), gameFiles.GetMap().Height());
                hero.playerController.HandleInput(hero.player, cam, tilePixSize, numTilesScreen, gameFiles.GetMap().Width(), gameFiles.GetMap().Height(), network);
                network.UpdatePlayers(tilePixSize,numTilesScreen,gameFiles.GetMap().GetMapSize());
                if (Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    gameState.SetGameState(GameState.TileEdit);
                    this.IsMouseVisible = true;
                }
            }

            if (gameState.GetGameState() == GameState.TileEdit)
            {
                int tilePixSize = gameFiles.GetTileMaps().TilePixSize();
                if (network.IsConnected())
                {
                    for (int i = 0; i < network.GetNumPlayers(); i++)
                    {
                        Player player = network.GetPlayer(i);
                        player.Update(tilePixSize, numTilesScreen.Width, numTilesScreen.Height, gameFiles.GetMap().Width(), gameFiles.GetMap().Height());
                    }
                }
                mapEditor.HandleInput(screenOffset,
                    gameFiles.GetMap(), windowSize, tileScreenSize, cam, gameFiles.GetTileMaps().TilePixSize(), numTilesScreen, gameFiles.GetTileMaps());
            }
            if (gameState.GetGameState() == GameState.MainMenu)
                mainMenu.HandleInput();

            if (gameState.GetGameState() == GameState.EscapeMenu)
            {
                int tilePixSize = gameFiles.GetTileMaps().TilePixSize();
                if (network.IsConnected())
                {
                    for (int i = 0; i < network.GetNumPlayers(); i++)
                    {
                        Player player = network.GetPlayer(i);
                        player.Update(tilePixSize, numTilesScreen.Width, numTilesScreen.Height, gameFiles.GetMap().Width(), gameFiles.GetMap().Height());
                    }
                }
                escapeMenu.HandleInput();
            }
            prevKeyboardState = Keyboard.GetState();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            int FPS = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);

            if (gameState.GetGameState() == GameState.Play || gameState.GetGameState() == GameState.TileEdit || gameState.GetGameState() == GameState.EscapeMenu)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);
                gameFiles.GetMap().Draw(spriteBatch, numTilesScreen, cam.Position(), gameFiles.GetTileMaps(), screenOffset, tileScreenSize);
                DrawPlayers();
                spriteBatch.End();
            }
            if (gameState.GetGameState() == GameState.TileEdit)
                mapEditor.DrawUI(spriteBatch, gameFiles.GetTileMaps());

            if (gameState.GetGameState() == GameState.MainMenu)
                mainMenu.Draw(spriteBatch);

            if (gameState.GetGameState() == GameState.EscapeMenu)
                escapeMenu.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.DrawString(GameFont, FPS.ToString(), new Vector2(0, 0), Color.White);
            spriteBatch.End();
       

            base.Draw(gameTime);
        }

        void DrawPlayers()
        {
            int tilePixSize = gameFiles.GetTileMaps().TilePixSize();
            hero.player.Draw(spriteBatch, tileScreenSize, tilePixSize, mapEditor.GetLayerNumber(), windowSize, cam);
            network.DrawPlayers(spriteBatch, tileScreenSize, tilePixSize, mapEditor.GetLayerNumber(), windowSize, cam);
        } 
    }
}
