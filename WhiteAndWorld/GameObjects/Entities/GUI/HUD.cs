using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class HUD : Entity 
    {
        private Sprite window, weapon, infinite;
        private Sprite gamerPicture;        // Muestra la imagen de jugador.        

        private Rectangle rgbGun, cmykGun;        

        private TextLabel weaponAmmo;       // Muestra el arma y mucion en uso.
        private TextLabel gamerNick;        // Muestra el nickname del usuario.
        private TextLabel gameScore;        // Muestra la puntuacion.

        public override void Initialize()
        {
            base.Initialize();

            // Posicion de la HUD:
            this.Location = new Vector2(8, 8);

            rgbGun = new Rectangle(0, 0, 64, 24);
            cmykGun = new Rectangle(0, 24, 64, 24);

            window = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\HUD\hud_window"));
            window.Location = this.Location;
            window.Color = Color.White;

            weapon = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\HUD\hud_weapons"));
            weapon.Location = this.Location + new Vector2(92, 52);
            weapon.Color = Color.White;

            infinite = new Sprite(Manager.Graphics.LoadTexture(@"GameUI\HUD\hud_infinite_char"));
            infinite.Location = this.Location + new Vector2(174, 56);
            infinite.Scale = 2;
            infinite.Color = Color.White;
            infinite.Visible = false;

            // Inicializamos el marcador de arma y municion:
            Manager.Graphics.LoadFont(@"Fonts\androidNation14");

            Color textColor = new Color(0.99f, 0.99f, 0.79f);

            this.weaponAmmo = new TextLabel(@"Fonts\androidNation14");
            this.weaponAmmo.Location = this.Location + new Vector2(156, 58);
            this.weaponAmmo.Color = textColor;

            this.gamerNick = new TextLabel(@"Fonts\androidNation14");
            this.gamerNick.Location = this.Location + new Vector2(88, 8);
            this.gamerNick.Color = textColor;

            this.gameScore = new TextLabel(@"Fonts\androidNation14");
            this.gameScore.Location = this.Location + new Vector2(156, 34);
            this.gameScore.Color = textColor;

            this.gamerPicture = new Sprite(Manager.Graphics.LoadTexture("JossAvatar"));
            this.gamerPicture.Animations.AddSecuence("alive01", new Rectangle(0, 0, 68, 68), 20, 64, true);
            this.gamerPicture.Animations.AddSecuence("alive02", new Rectangle(0, 68, 68, 68), 20, 64, true);
            this.gamerPicture.Animations.AddSecuence("death", new Rectangle(0, 136, 68, 68), 9, 132, true);
            this.gamerPicture.Location = this.Location + new Vector2(13, 9);
            this.gamerPicture.Color = Color.White;
            this.gamerPicture.Update(Manager.GameTime);

            this.gamerNick.Text = "Joss Stress";

            this.ZOrder = -999;    // Necesitamos que se dibuje siempre encima de todo menos los menus.           
        }

        public override void Draw(GameTime gameTime)
        {
            this.gamerPicture.Draw(gameTime);
            this.window.Draw(gameTime);
            this.weapon.Draw(gameTime, (bool)Manager.Vars["playerSelectCMYKGun"] == true ? cmykGun : rgbGun);
            this.gamerNick.Draw(gameTime);
            this.gameScore.Draw(gameTime);
            this.weaponAmmo.Draw(gameTime);
            this.infinite.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            this.weaponAmmo.Text = "x";

            // Obtenemos la instancia del componente del jugador:
            Player p = (Player)Manager.Scene.FindByName("player");

            if (p.IsDead)
                gamerPicture.Animations.Play("death");
            else
                gamerPicture.Animations.Play("alive01");

            this.weaponAmmo.Text += " " + ((bool)Manager.Vars["playerSelectCMYKGun"] ? Manager.Vars["CMYK_Ammo"] : "&");
            this.gameScore.Text = string.Format("{0,10}", (int)Manager.Vars["score"]);
            
            this.gamerPicture.Update(gameTime);
            this.window.Update(gameTime);
        }
    }
}