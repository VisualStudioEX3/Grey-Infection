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
using WhiteAndWorld.GameObjects.Entities;
using WhiteAndWorld.GameObjects.Entities.GUI;

namespace WhiteAndWorld.GameObjects.Entities.EndGame
{
    public class ConditionAlert : Entity
    {
        private RectangleOverlay redBox;
        private Label warning;
        private List<string> captions;
        private Timer timer;
        private int i;
        private int loops;

        private float alpha;
        private int flag;
        private float maxAlpha;

        private bool disable;

        public delegate void ExitEventHandler();

        public ExitEventHandler EndEvent;

        public override void Initialize()
        {
            base.Initialize();

            captions = new List<string>();
            captions.Add(@"          {0}          ");
            captions.Add(@"          {0}          ");
            captions.Add(@"/         {0}         \");
            captions.Add(@"//        {0}        \\");
            captions.Add(@"///       {0}       \\\");
            captions.Add(@" ///      {0}      \\\ ");
            captions.Add(@"  ///     {0}     \\\  ");
            captions.Add(@"   ///    {0}    \\\   ");
            captions.Add(@"    ///   {0}   \\\    ");
            captions.Add(@"     ///  {0}  \\\     ");
            captions.Add(@"      /// {0} \\\      ");
            captions.Add(@"       // {0} \\       ");
            captions.Add(@"        / {0} \        ");
            captions.Add(@"          {0}          ");
            captions.Add(@"      /// {0} \\\      ");
            captions.Add(@"          {0}          ");
            captions.Add(@"      /// {0} \\\      ");
            captions.Add(@"          {0}          ");
            captions.Add(@"/     {1}     \");
            captions.Add(@"      {1}      ");
            captions.Add(@"      {1}      ");
            captions.Add(@"//    {1}    \\");
            captions.Add(@"///   {1}   \\\");
            captions.Add(@" ///  {1}  \\\ ");
            captions.Add(@"  /// {1} \\\  ");
            captions.Add(@"   // {1} \\   ");
            captions.Add(@"    / {1} \    ");
            captions.Add(@"      {1}      ");
            captions.Add(@"      {1}      ");
            captions.Add(@"      {1}      ");
            captions.Add(@"  /// {1} \\\  ");
            captions.Add(@"      {1}      ");
            captions.Add(@"  /// {1} \\\  ");
            captions.Add(@"      {1}      ");
            //captions.Add(@"          WARNING          ");
            //captions.Add(@"          WARNING          ");
            //captions.Add(@"/         WARNING         \");
            //captions.Add(@"//        WARNING        \\");
            //captions.Add(@"///       WARNING       \\\");
            //captions.Add(@" ///      WARNING      \\\ ");
            //captions.Add(@"  ///     WARNING     \\\  ");
            //captions.Add(@"   ///    WARNING    \\\   ");
            //captions.Add(@"    ///   WARNING   \\\    ");
            //captions.Add(@"     ///  WARNING  \\\     ");
            //captions.Add(@"      /// WARNING \\\      ");
            //captions.Add(@"       // WARNING \\       ");
            //captions.Add(@"        / WARNING \        ");
            //captions.Add(@"          WARNING          ");
            //captions.Add(@"      /// WARNING \\\      ");
            //captions.Add(@"          WARNING          ");
            //captions.Add(@"      /// WARNING \\\      ");
            //captions.Add(@"          WARNING          ");
            //captions.Add(@"/     CONDITION ALERT     \");
            //captions.Add(@"      CONDITION ALERT      ");
            //captions.Add(@"      CONDITION ALERT      ");
            //captions.Add(@"//    CONDITION ALERT    \\");
            //captions.Add(@"///   CONDITION ALERT   \\\");
            //captions.Add(@" ///  CONDITION ALERT  \\\ ");
            //captions.Add(@"  /// CONDITION ALERT \\\  ");
            //captions.Add(@"   // CONDITION ALERT \\   ");
            //captions.Add(@"    / CONDITION ALERT \    ");
            //captions.Add(@"      CONDITION ALERT      ");
            //captions.Add(@"      CONDITION ALERT      ");
            //captions.Add(@"      CONDITION ALERT      ");
            //captions.Add(@"  /// CONDITION ALERT \\\  ");
            //captions.Add(@"      CONDITION ALERT      ");
            //captions.Add(@"  /// CONDITION ALERT \\\  ");
            //captions.Add(@"      CONDITION ALERT      ");

            redBox = new RectangleOverlay(Manager.Graphics.ScreenBounds);
            redBox.BackColor = Color.Red;
            redBox.ForeColor = Color.Red;
            redBox.Fixed = true;

            alpha = 1;
            maxAlpha = 1;
            flag = 0;

            warning = new Label();
            warning.Initialize();
            warning.Center = true;
            warning.Scale = 2;
            warning.Location = Helper.PointToVector2(Manager.Graphics.ScreenBounds.Center);

            i = 0;
            loops = 0;
            disable = false;

            timer = new Timer();

            base.ZOrder = -999999999;
        }

        public override void Terminate()
        {
            base.Terminate();
        }
                
        public override void Update(GameTime gameTime)
        {
            if (timer.Value > 100)
            {
                timer.Reset();
                
                if (!disable)
                {
                    warning.Caption = captions[i].Replace("{0}", Session.Strings["gameover_warning"]).Replace("{1}", Session.Strings["gameover_conditionalert"]);
                    i++;

                    if (i == captions.Count)
                    {
                        i = 0;
                        loops++;
                    }
                    if (loops == 1)
                    {
                        warning.Visible = false;
                        maxAlpha = 0.5f;
                        if (EndEvent != null) EndEvent();
                        disable = true;
                    } 
                }

                if (flag == 0)
                {
                    alpha -= 0.25f;
                    if (alpha < 0) 
                    {
                        alpha = 0;
                        flag = 1;
                    }
                }
                else
                {
                    alpha += 0.25f;
                    if (alpha > maxAlpha)
                    {
                        alpha = maxAlpha;
                        flag = 0;
                    }
                }

                redBox.BackColor = new Color(1, 0, 0, alpha);
            }            

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            redBox.Draw(gameTime);
            if (warning.Visible) warning.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}