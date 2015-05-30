using System;
using System.Collections.Generic;
using System.Linq;

// Referencias basicas a XNA:
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

// Referencias basicas al TLSA.Engine:
using TLSA.Engine;
using TLSA.Engine.Graphics;
using TLSA.Engine.Input;
using TLSA.Engine.Scene;

using WhiteAndWorld.GameObjects;

namespace WhiteAndWorld
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameBase : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public GameBase()
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

            //Guide.SimulateTrialMode = true;

            // Inicializamos el TLSA.Engine:
            Manager.Initialize(this, graphics);

            // Inicializamos el motor grafico:
            {
                Manager.Graphics.SetDisplayMode(PrefDisplayModes._720p);
                Manager.Graphics.SynchronizeWithVerticalRetrace = true;
                Manager.Graphics.ClearColor = Color.Black;
                Manager.Graphics.LoadShader(@"Shaders\GreyScale");
            }

            // Inicializamos la instancia global del motor de fisicas:
            {
                Rectangle workArea = Manager.Graphics.ScreenBounds;
                workArea.X -= 16; workArea.Y -= 16;
                workArea.Width += 32; workArea.Height += 32;
                Manager.PhysicEngine.WorkArea = workArea;
                Manager.PhysicEngine.Gravity = new Vector2(0, 9);
                //Manager.PhysicEngine.Visible = true;
            }

            // Inicializamos el gestor de eventos de entrada:
            Manager.UIInput.SelectedDevice = InputType.GamePad;

            // Mapa de input del juego, se usara tambien para la GUI:
            {
                Manager.UIInput.Actions.Add("left", new InputAction(Keys.Left, null, Buttons.LeftThumbstickLeft));
                Manager.UIInput.Actions.Add("right", new InputAction(Keys.Right, null, Buttons.LeftThumbstickRight));
                Manager.UIInput.Actions.Add("down", new InputAction(Keys.Down, null, Buttons.LeftThumbstickDown));
                Manager.UIInput.Actions.Add("up", new InputAction(Keys.Up, null, Buttons.LeftThumbstickUp));

                Manager.UIInput.Actions.Add("menu_left", new InputAction(Keys.Left, null, Buttons.DPadLeft));
                Manager.UIInput.Actions.Add("menu_right", new InputAction(Keys.Right, null, Buttons.DPadRight));
                Manager.UIInput.Actions.Add("menu_down", new InputAction(Keys.Down, null, Buttons.DPadDown));
                Manager.UIInput.Actions.Add("menu_up", new InputAction(Keys.Up, null, Buttons.DPadUp));

                Manager.UIInput.Actions.Add("jump", new InputAction(Keys.LeftControl, null, Buttons.A));
                Manager.UIInput.Actions.Add("crouch", new InputAction(Keys.Down, null, Buttons.LeftTrigger));
                Manager.UIInput.Actions.Add("switch", new InputAction(Keys.LeftShift, null, Buttons.X));
                Manager.UIInput.Actions.Add("shoot", new InputAction(Keys.Space, null, Buttons.RightTrigger));
                Manager.UIInput.Actions.Add("pause", new InputAction(Keys.Escape, null, Buttons.Start));

                Manager.UIInput.Actions.Add("ok", new InputAction(Keys.Enter, null, Buttons.Start));

                Manager.UIInput.Actions.Add("exit", new InputAction(Keys.Escape, null, Buttons.B));

                Manager.UIInput.Actions.Add("grey", new InputAction(Keys.F1, null, Buttons.LeftShoulder));
                Manager.UIInput.Actions.Add("phyx", new InputAction(Keys.F2, null, Buttons.RightShoulder));
            }

            // Variables globales:
            {
                Manager.Vars.Create("currentLevel", 1);
                Manager.Vars.Create("lastLevel", 1);
                Manager.Vars.Create("loadSaveGame", false);
                Manager.Vars.Create("firstGame", true);

                Manager.Vars.Create("CMYK_Ammo", 0);           	// Contador de municion del arma CMYK.
                Manager.Vars.Create("prev_CMYK_Ammo", 0);

                Manager.Vars.Create("errorBoard", "");

                Manager.Vars.Create("showMessageReady", true);  // Indica si se mostrara el mensaje de inicio de partida.
                Manager.Vars.Create("showMessageDead", false);  // Indica si se muestra el mensaje de muerte.
                Manager.Vars.Create("showMessagePause", false); // Indica si se muestra el mensaje de pausa.
                Manager.Vars.Create("score", 0);                // Contador de puntuacion.

                Manager.Vars.Create("setGameState", "");

                Manager.Vars.Create("creditsFromEndGame", false);
                Manager.Vars.Create("purchaseAndReturnMenu", false);
                Manager.Vars.Create("gamepadLost", false);
            }

            // Estados del juego:
            {
                Manager.GameStates.States.Add("splash", new GameObjects.States.SplashScreen());
                Manager.GameStates.States.Add("menu", new GameObjects.States.Menu());
                Manager.GameStates.States.Add("levelSelection", new GameObjects.States.LevelSelection());
                Manager.GameStates.States.Add("game", new GameObjects.States.Game());
                Manager.GameStates.States.Add("credits", new GameObjects.States.Credits());
                Manager.GameStates.States.Add("options", new GameObjects.States.Options());
                Manager.GameStates.States.Add("gametitle", new GameObjects.States.GameTitle());
                Manager.GameStates.States.Add("intro", new GameObjects.States.Intro());
                Manager.GameStates.States.Add("gameover", new GameObjects.States.GameOver());
                Manager.GameStates.States.Add("endtrial", new GameObjects.States.EndTrial());

                Manager.GameStates.ChangeState("splash");
            }

            StorageSession.Initialize();

            base.Initialize();
        }

        private void GenerateTestLevelXML()
        {
            GameDefinitions.LevelData level = new GameDefinitions.LevelData();

            GameDefinitions.Player player = new GameDefinitions.Player();
            player.Location = Vector2.Zero;
            level.Player = player;

            GameDefinitions.Tile tile = new GameDefinitions.Tile();
            tile.Active = true;
            tile.Location = Vector2.Zero;
            tile.Tag = "tag";
            tile.Texture = "textureAsset";
            level.Tiles.Add(tile);

            GameDefinitions.Clue clue = new GameDefinitions.Clue();
            clue.Location = Vector2.Zero;
            clue.Texture = "textureAsset";
            level.Clues.Add(clue);

            GameDefinitions.Background backg = new GameDefinitions.Background();
            backg.Texture = "textureAsset";

            GameDefinitions.Enemy enemy = new GameDefinitions.Enemy();
            enemy.Action = 0;
            enemy.Behavior = 0;
            enemy.Invincible = false;
            enemy.Location = Vector2.Zero;
            enemy.PathLength = 0;
            enemy.Respawn = false;
            enemy.RespawnDelay = 0;
            enemy.ReversePathAtStart = false;
            enemy.Step = 0;
            enemy.Target = "tag";
            enemy.Type = 0;
            level.Enemies.Add(enemy);

            GameDefinitions.ExitArea exit = new GameDefinitions.ExitArea();
            exit.Location = Vector2.Zero;
            level.ExitArea = exit;

            TLSA.Engine.Tools.XML.Serializers.SerializeToFile(level, "test.lev");
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
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
            // TODO: Add your update logic here
            Manager.Update(gameTime);   // Actualiza los estados del TLSA.Engine.

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            Manager.Draw(gameTime);     // Dibuja todos los componentes del TLSA.Engine.

            base.Draw(gameTime);
        }
    }
}