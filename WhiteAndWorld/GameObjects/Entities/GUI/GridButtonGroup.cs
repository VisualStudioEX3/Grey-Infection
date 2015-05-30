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
using TLSA.Engine.Tools.XML;
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;
using WhiteAndWorld.GameObjects.Entities.MainMenu;

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    /// <summary>
    /// Grupo de botones.
    /// </summary>
    public class GridButtonGroup : Entity
    {
        private List<LevelSelectionButton> buttons;
        private int selected;
        private SoundPlayer selectSound;

        public delegate void OnExitHandler(GridButtonGroup group);

        public OnExitHandler OnExit;

        public int ButtonWidth { get; set; }
        public int ButtonHeight { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        private int currentRow, currentColumn;

        public LevelSelectionButton Selected { get { return buttons[selected]; } }

        public override void Initialize()
        {
            base.Initialize();
            selected = 0;
            buttons = new List<LevelSelectionButton>();

            selectSound = new SoundPlayer(@"Audio\FX\select01");

            Rows = 4;
            Columns = 8;

            currentRow = 0;
            currentColumn = 0;
        }

        /// <summary>
        /// Añade un boton y lo ubica debajo del siguiente.
        /// </summary>
        /// <param name="button">Boton que se agregara al grupo.</param>
        public void Add(LevelSelectionButton button)
        {
            button.Location = new Vector2(Location.X + (ButtonWidth * currentColumn), Location.Y + (ButtonHeight * currentRow));
            currentColumn++;

            if (currentColumn == Columns)
            {
                currentColumn = 0;
                currentRow++;
            }

            button.GridGroup = this;
            if (buttons.Count == 0) button.Selected = true;

            buttons.Add(button);

            button.Level = buttons.Count;
            button.Caption = button.Level.ToString();
            button.Locked = button.Level > (int)Manager.Vars["lastLevel"];
        }

        /// <summary>
        /// Actualiza los estados de los botones del grupo.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Boton de escape:
            if (Manager.UIInput.Hit("exit")) if (OnExit != null) OnExit(this);

            // Controles para desplazarse por los botones:
            if (Manager.UIInput.Hit("right") || Manager.UIInput.Hit("menu_right"))
            {
                selectSound.Play();
                buttons[selected].Selected = false;
                selected++;
                if (selected == buttons.Count) selected = 0;
                if (selected >= Session.GameProgress.CurrentLevel) selected = 0;
                buttons[selected].Selected = true;                
            }
            else if (Manager.UIInput.Hit("left") || Manager.UIInput.Hit("menu_left"))
            {
                selectSound.Play();
                buttons[selected].Selected = false;
                selected--;
                if (selected < 0) selected = buttons.Count - 1;
                if (selected >= Session.GameProgress.CurrentLevel) selected = Session.GameProgress.CurrentLevel - 1;
                buttons[selected].Selected = true;
            }
            else if (Manager.UIInput.Hit("up") || Manager.UIInput.Hit("menu_up"))
            {
                selectSound.Play();
                buttons[selected].Selected = false;
                selected -= Columns;
                if (selected < 0) selected = buttons.Count - 1;
                if (selected >= Session.GameProgress.CurrentLevel) selected = 0;
                buttons[selected].Selected = true;
            }
            else if (Manager.UIInput.Hit("down") || Manager.UIInput.Hit("menu_down"))
            {
                selectSound.Play();
                buttons[selected].Selected = false;
                selected += Columns;
                if (selected > buttons.Count) selected = 0;
                if (selected >= Session.GameProgress.CurrentLevel) selected = Session.GameProgress.CurrentLevel - 1;
                buttons[selected].Selected = true;
            }

            // Actualizamos todos los botones:
            foreach (ButtonBase b in buttons)
                b.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Dibuja los botones de la lista.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Dibujamos todos los botones:
            foreach (ButtonBase b in buttons)
                b.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}