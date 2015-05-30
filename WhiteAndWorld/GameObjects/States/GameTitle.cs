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
using TLSA.Engine;
using TLSA.Engine.Graphics;
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;
using WhiteAndWorld.GameObjects.Entities.MainMenu;

namespace WhiteAndWorld.GameObjects.States
{
    public class GameTitle : StateComponent
    {
        private Bitmap background;
        private Bitmap fog;
        private Sprite layer1, layer2;  // Scroll
        private Bitmap wall;
        private Sprite joss;
        private Bitmap shine;
        private Sprite fan;
        private Sprite lightLamp;
        private Sprite logo;

        private TextLabel footstep, footstepShadow, pressStart, pressStartShadow;

        private Timer timer, timer2;
        private int flag = 0;

        private bool scaleAnimationFinish;
        private bool start;

        private GlitchColor glitch;

        public override void Initialize()
        {            
            base.Initialize();

            Session.Settings = new GameDefinitions.GameSettings();

            scaleAnimationFinish = false;
            start = false;

            MusicPlayer.Play(MusicPlayer.MenuTheme);

            Manager.Graphics.EndShader();

            // Cargamos la lista de cadenas de texto del juego:
            Session.Strings = Serializers.DeserializeFromTitleStorage<SerializableDictionary<string, string>>("GameData/Strings/" + Session.Culture + ".xml").ToDictionary();

            Manager.Graphics.OffSet = Vector2.Zero;

            Manager.Graphics.LoadTexture("theGrid");
            background = new Bitmap("theGrid");
            background.Size = new Vector2(1280, 720);

            Manager.Graphics.LoadTexture("backgroundFog");
            fog = new Bitmap("backgroundFog");
            fog.Size = new Vector2(1280, 720);

            layer1 = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\backgroundLayer1"));
            layer1.Scale = -0.4f;

            layer2 = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\backgroundLayer1"));
            layer2.Location = new Vector2(1280, 0);
            layer2.Visible = false;

            Manager.Graphics.LoadTexture(@"GameUI\Gametitle\wall");
            wall = new Bitmap(@"GameUI\Gametitle\wall");

            fan = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\fan"));
            fan.Location = new Vector2(-25, -100);
            fan.Animations.AddSecuence("default", new Rectangle(0,0,400,400), 4, 64, true);
            fan.Animations.Play("default");
            fan.Update(new GameTime());
            
            lightLamp = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\lightLamp"));
            lightLamp.Location = new Vector2(-24, 400);
            
            Manager.Graphics.LoadTexture(@"GameUI\Gametitle\joss");
            joss = new Sprite(@"GameUI\Gametitle\joss");
            joss.Scale = 4;

            Manager.Graphics.LoadTexture(@"GameUI\Gametitle\shine");
            shine = new Bitmap(@"GameUI\Gametitle\shine");

            logo = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\Gametitle\logo"));
            logo.Location = new Vector2(800, 256);
            logo.Scale = 4;
            logo.Center = true;

            Manager.Graphics.LoadFont(@"Fonts\androidNation14");
            footstep = new TextLabel(@"Fonts\androidNation14");
            footstep.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X, Manager.Graphics.ScreenSafeArea.Bottom - 14);
            footstep.Text = Session.Strings["copyright"];
            footstep.Visible = false;
            footstep.Center = true;

            footstepShadow = new TextLabel(@"Fonts\androidNation14");
            footstepShadow.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X + 2, Manager.Graphics.ScreenSafeArea.Bottom - 14 + 2);
            footstepShadow.Text = Session.Strings["copyright"];
            footstepShadow.Color = Color.Black;
            footstepShadow.Visible = false;
            footstepShadow.Center = true;

            Manager.Graphics.LoadFont(@"Fonts\androidNation20b");
            pressStart = new TextLabel(@"Fonts\androidNation20b");
            pressStart.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X, Manager.Graphics.ScreenSafeArea.Center.Y + 150);
            pressStart.Text = Session.Strings["pressStart"];
            pressStart.Visible = false;
            pressStart.Center = true;

            pressStartShadow = new TextLabel(@"Fonts\androidNation20b");
            pressStartShadow.Location = new Vector2(Manager.Graphics.ScreenSafeArea.Center.X + 2, Manager.Graphics.ScreenSafeArea.Center.Y + 150 + 2);
            pressStartShadow.Text = Session.Strings["pressStart"];
            pressStartShadow.Color = Color.Black;
            pressStartShadow.Visible = false;
            pressStartShadow.Center = true;

            timer = new Timer();
            timer2 = new Timer();

            glitch = new GlitchColor();
            Manager.Scene.AddEntity(glitch);
        }

        int playerIndex = -1;

        // Obtiene el gamepad activo y lo asocia al mapa de input:
        private bool GetPlayerIndex()
        {
            GamePadState state;

            for (int i = 0; i < 4; i++)
            {
                state = GamePad.GetState((PlayerIndex)i);
                if (state.IsButtonDown(Buttons.Start) || state.IsButtonDown(Buttons.A))
                {
                    playerIndex = i;
                    return true;
                }
            }

            return false;
        }

        // Indica si el jugador tiene la sesion iniciada:
        private bool IsPlayerSignedIn(PlayerIndex index, bool showSignInDialog)
        {
            foreach (SignedInGamer player in Manager.GamerServices.SignedInGamers)
                if (player.PlayerIndex == index) return true;

            // Mostramos la ventana de seleccion de perfil de jugador para iniciar sesion:
            if (showSignInDialog && !Guide.IsVisible)
            {
                Guide.ShowSignIn(1, false);
                requestSignin = true;
            }

            return false;
        }

        bool requestSignin = false;
        public override void Update(GameTime gameTime)
        {
            if (GetPlayerIndex())
            {
                // Si el perfil no tiene iniciada sesion:
                if (!IsPlayerSignedIn((PlayerIndex)playerIndex, false) && !Guide.IsVisible)
                {
                    Guide.ShowSignIn(1, false);
                    requestSignin = true;
                }
                else if (IsPlayerSignedIn((PlayerIndex)playerIndex, false)) // Si el perfil tiene iniciada sesion:
                    StorageSession.SelectDevice((PlayerIndex)playerIndex);

                start = true;
            }

            if (IsPlayerSignedIn((PlayerIndex)playerIndex, false) && requestSignin && !Guide.IsVisible) // Si el perfil tiene iniciada sesion:
            {
                StorageSession.SelectDevice((PlayerIndex)playerIndex);
                requestSignin = false;
            }

            // Si el dispositivo esta listo:
            if (StorageSession.IsDeviceInitialize && start)
            {
                {
                    Manager.UIInput.Player = (PlayerIndex)playerIndex;

                    // Cargamos el progreso del jugador:
                    if (!Guide.IsTrialMode && StorageSession.FileExists(Session.SaveGameFilename))
                        Session.GameProgress = StorageSession.LoadData<GameDefinitions.SaveGame>(Session.SaveGameFilename);
                    else // Si no hubiera archivo de progresos creamos progresos nuevos para el usuario:
                        Session.GameProgress = new GameDefinitions.SaveGame();

                    // Cargamos la configuracion del usuario:
                    if (StorageSession.FileExists(Session.SettingsFilename))
                        Session.Settings = StorageSession.LoadData<GameDefinitions.GameSettings>(Session.SettingsFilename);
                    else
                        Session.Settings = new GameDefinitions.GameSettings();

                    // Aplicamos la configuracion y restauramos progresos:
                    Session.Apply();

                    // Saltamos al menu principal:
                    Manager.GameStates.ChangeState("menu");
                }
            }

            // Codigo encargado de actualizar los elementos visuales del menu:
            if (flag == 0 && timer.Value > new Random().Next(1500, 5000))
            {
                timer.Reset();
                flag = 1;
                lightLamp.Color = Color.Gray;
            }
            else if (flag == 1 && timer.Value > 50)
            {
                timer.Reset();
                flag = 0;
                lightLamp.Color = Color.White;
            }

            fan.Update(gameTime);

            if (!scaleAnimationFinish)
            {
                if (logo.Scale > 1)
                {
                    joss.Scale -= 0.05f;
                    logo.Scale -= 0.05f;
                    layer1.Scale += 0.025f;
                }
                else if (logo.Scale < 1)
                {
                    joss.Scale = 1;
                    logo.Scale = 1;
                    layer1.Scale = 1;
                    scaleAnimationFinish = true;
                    glitch.Now();
                }
            }
            else
            {
                layer1.Location -= new Vector2(1, 0);
                layer2.Location -= new Vector2(1, 0);
                layer2.Visible = true;
                if (layer1.Location.X == -1280) layer1.Location = new Vector2(1280, 0);
                if (layer2.Location.X == -1280) layer2.Location = new Vector2(1280, 0);

                footstep.Visible = true;
                footstepShadow.Visible = true;

                if (timer2.Value >= 500)
                {
                    timer2.Reset();
                    pressStart.Visible = !pressStart.Visible;
                    pressStartShadow.Visible = pressStart.Visible;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            background.Draw(gameTime);
            
            layer1.Draw(gameTime);
            layer2.Draw(gameTime);

            fog.Draw(gameTime);

            wall.Draw(gameTime);
            fan.Draw(gameTime);

            lightLamp.Location = new Vector2(-24, 400); lightLamp.Draw(gameTime);
            lightLamp.Location = new Vector2(250, 400); lightLamp.Draw(gameTime);
            lightLamp.Location = new Vector2(325, 400); lightLamp.Draw(gameTime);

            joss.Draw(gameTime);
            shine.Draw(gameTime);

            logo.Draw(gameTime);

            footstepShadow.Draw(gameTime);
            footstep.Draw(gameTime);

            pressStartShadow.Draw(gameTime);
            pressStart.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
