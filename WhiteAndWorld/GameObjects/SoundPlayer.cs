using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using TLSA.Engine;
using TLSA.Engine.Tools;

namespace WhiteAndWorld.GameObjects
{
    public class SoundPlayer
    {
        private SoundEffect sound;
        private SoundEffectInstance player;

        public bool WaitToEnd { get; set; }

        public SoundPlayer(string assetName)
        {
            try
            {
                sound = Manager.Content.Load<SoundEffect>(assetName);                
            }
            catch (Exception)
            {
            }

            player = sound.CreateInstance();            
        }

        public SoundPlayer(string assetName, bool Wait)
            : this(assetName)
        {
            this.WaitToEnd = Wait;
        }

        public void Play()
        {
            player.Volume = Session.Settings.SoundVolume;
            if (!player.IsLooped) // && player.State == SoundState.Playing) 
                player.Stop();
            player.Play();
            while (this.WaitToEnd && player.State == SoundState.Playing) { }
        }
    }
}
