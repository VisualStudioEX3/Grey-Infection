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
using TLSA.Engine.Graphics.Primitives;
using TLSA.Engine.Input;
using TLSA.Engine.Physics.V1Engine;
using TLSA.Engine.Scene;
using TLSA.Engine.Tools.XML;
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;
using WhiteAndWorld.GameObjects.Entities.Options;

namespace WhiteAndWorld.GameObjects.States
{
    public class Options : StateComponent
    {
        private ButtonGroup menu;
        private SliderButton sliderSound, sliderMusic;

        private bool hasChanges;
        private bool storageRequest;

        private List<SoundPlayer> sounds;

        public override void Initialize()
        {
            base.Initialize();

            Manager.Scene.AddEntity(new PlayerSignedOut());

            hasChanges = false;
            storageRequest = false;

            Manager.Scene.AddEntity(new GreyInfectiontitleScene());

            menu = new ButtonGroup();
            Manager.Scene.AddEntity(menu);
            menu.Location = new Vector2(640, 374);
            menu.ButtonHeight = 40;
            menu.OnExit += OnExitState;

            sliderSound = new SliderButton();
            sliderSound.Caption = Session.Strings["optmenu_sound"];
            sliderSound.OnChange += OnChangeSoundVolume;
            menu.Add(sliderSound);

            sliderMusic = new SliderButton();
            sliderMusic.Caption = Session.Strings["optmenu_music"];
            sliderMusic.OnChange += OnChangeMusicVolume;
            menu.Add(sliderMusic);

            TextButton button;

            button = new TextButton();
            button.Caption = Session.Strings["optmenu_gamepad"];
            button.OnEnter += OnEnterControls;
            menu.Add(button);

            button = new TextButton();
            button.Caption = Session.Strings["optmenu_changestorage"];
            button.OnEnter += OnEnterStorage;
            menu.Add(button);

            button = new TextButton();
            button.Caption = Session.Strings["optmenu_restore"];
            button.OnEnter += OnEnterRestore;
            menu.Add(button);

            // Indicaciones de input:
            InputHintLabel inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.A;
            inputHint.Caption = Session.Strings["button_select"];
            inputHint.Location = new Vector2(220, Manager.Graphics.ScreenSafeArea.Bottom - 14);

            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.B;
            inputHint.Caption = Session.Strings["button_back"];
            inputHint.Location = new Vector2(475, Manager.Graphics.ScreenSafeArea.Bottom - 14);

            inputHint = new InputHintLabel();
            Manager.Scene.AddEntity(inputHint);
            inputHint.Button = InputHintLabel.GamepadButtonChar.LeftThumb;
            inputHint.Caption = Session.Strings["button_move2"];
            inputHint.Location = new Vector2(700, Manager.Graphics.ScreenSafeArea.Bottom - 14);

            // Sonidos de muestra para evaluar volumen de los efectos:
            sounds = new List<SoundPlayer>();
            string assetName;
            foreach (string file in System.IO.Directory.GetFiles(Manager.Content.RootDirectory + @"\Audio\FX\", "*.*"))
            {
                assetName = file.Replace(".xnb", "").Replace(Manager.Content.RootDirectory + @"\", "");
                sounds.Add(new SoundPlayer(assetName));
            }

            SetParams();       
        }

        private void OnExitState(ButtonGroup group)
        {
            // Al salir del estado guardamos los cambios de configuracion si los hubiera:
            if (hasChanges)
            {
                hasChanges = false;
                StorageSession.SaveData(Session.SettingsFilename, Session.Settings);
            }

            Manager.GameStates.ChangeState("menu");
        }

        private void SetParams()
        {
            sliderSound.Value = (int)(Session.Settings.SoundVolume * 10);
            sliderMusic.Value = (int)(Session.Settings.MusicVolume * 10);
        }

        private void OnChangeSoundVolume(ButtonGroup group, int value)
        {
            sounds[TLSA.Engine.Tools.MathTools.RandomNumberGenerator.Next(0, sounds.Count)].Play();
            Session.Settings.SoundVolume = value * 0.1f;            
            hasChanges = true;
        }

        private void OnChangeMusicVolume(ButtonGroup group, int value)
        {
            Session.Settings.MusicVolume = value * 0.1f;
            MusicPlayer.Volume = Session.Settings.MusicVolume;
            hasChanges = true;
        }

        private void OnEnterControls(ButtonGroup group)
        {
            GameInputMap controls = new GameInputMap();
            Manager.Scene.AddEntity(controls);
            controls.Sender = group;
            group.Enabled = false;
        }
        
        private void OnEnterStorage(ButtonGroup group)
        {
            StorageSession.SelectDevice();
            storageRequest = true;
        }

        public override void Update(GameTime gameTime)
        {            
            // Si el dispositivo esta listo:
            if (StorageSession.IsDeviceInitialize && storageRequest)
            {
                storageRequest = false;

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

                SetParams();
            }
        }

        public void OnEnterRestore(ButtonGroup group)
        {
            group.Enabled = false;
            ConfirmationScreen dialog = new ConfirmationScreen();
            Manager.Scene.AddEntity(dialog);
            dialog.Caption = Session.Strings["confirm_restore"];
            dialog.Group = menu;
            dialog.OnOkEvent += OnRestoreEvent;
        }

        public void OnRestoreEvent()
        {
            Session.Settings = new GameDefinitions.GameSettings();
            SetParams();
        }
    }
}
