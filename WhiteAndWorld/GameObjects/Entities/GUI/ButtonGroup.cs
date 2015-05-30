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
    public class ButtonGroup : Entity
    {        
        private List<ButtonBase> buttons;
        private int selected;
        private SoundPlayer selectUp, selectDown;

        public delegate void OnExitHandler(ButtonGroup group);

        public OnExitHandler OnExit;

        public int ButtonHeight { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            selected = 0;
            buttons = new List<ButtonBase>();

            selectUp = new SoundPlayer(@"Audio\FX\select01");
            selectDown = new SoundPlayer(@"Audio\FX\select01");

            Update(Manager.GameTime);
        }

        /// <summary>
        /// Añade un boton y lo ubica debajo del siguiente.
        /// </summary>
        /// <param name="button">Boton que se agregara al grupo.</param>
        public void Add(ButtonBase button)
        {
            button.Location = new Vector2(Location.X, Location.Y + (ButtonHeight * buttons.Count));
            button.Group = this;
            if (buttons.Count == 0) button.Selected = true;
            buttons.Add(button);            
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
            if (Manager.UIInput.Hit("down") || Manager.UIInput.Hit("menu_down"))
            {
                selectDown.Play();
                buttons[selected].Selected = false;
                selected++;
                if (selected == buttons.Count) selected = 0;
                buttons[selected].Selected = true;
            }
            else if (Manager.UIInput.Hit("up") || Manager.UIInput.Hit("menu_up"))
            {
                selectUp.Play();
                buttons[selected].Selected = false;
                selected--;
                if (selected < 0) selected = buttons.Count - 1;
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