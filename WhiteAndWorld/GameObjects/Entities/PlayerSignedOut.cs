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
using TLSA.Engine.Tools;
using TLSA.Engine.Tools.XML;
using WhiteAndWorld.GameObjects.Entities.GUI;

namespace WhiteAndWorld.GameObjects.Entities
{
    public class PlayerSignedOut : Entity
    {
        private bool showMessage = false;

        public override void Initialize()
        {
            base.Initialize();
            Priority = 999999999;
        }

        // Indica si el jugador tiene la sesion iniciada:
        private bool IsPlayerSignedIn(PlayerIndex index)
        {
            foreach (SignedInGamer player in Manager.GamerServices.SignedInGamers)
                if (player.PlayerIndex == index) return true;

            return false;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Guide.IsVisible && !IsPlayerSignedIn(Manager.UIInput.Player) && !showMessage)
            {
                showMessage = true;

                Manager.Vars["showMessagePause"] = true;

                List<string> MBOPTIONS = new List<string>();
                MBOPTIONS.Add(Session.Strings["signedout_button"]);
                Guide.BeginShowMessageBox(Session.Strings["signedout_title"], Session.Strings["signedout_message"], MBOPTIONS, 0, MessageBoxIcon.Warning, GetMBResult, null);
            }

            base.Update(gameTime);
        }

        protected void GetMBResult(IAsyncResult r)
        {
            int? b = Guide.EndShowMessageBox(r);
            Manager.Vars["showMessagePause"] = false;
            StorageSession.Release();
            Manager.GameStates.ChangeState("gametitle");
        }
    }
}