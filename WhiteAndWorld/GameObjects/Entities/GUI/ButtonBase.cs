using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TLSA.Engine;
using TLSA.Engine.Scene;

namespace WhiteAndWorld.GameObjects.Entities.GUI
{
    public class ButtonBase : Entity
    {        
        internal ButtonGroup Group { get; set; }
        internal bool Selected { get; set; }

        private SoundPlayer confirm;

        public delegate void OnEnterHandler(ButtonGroup group);
        public delegate void OnChangeHandler(ButtonGroup group, int value);

        public OnEnterHandler OnEnter;
        public OnChangeHandler OnChange;

        public virtual string Caption { get; set; }

        private int _value = 10;
        public virtual int Value 
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_value > 10) _value = 10; else if (_value < 0) _value = 0;
                if (OnChange != null) OnChange(Group, _value);
            }
        }

        public bool ConfirmFXWaitToEnd { get; set; }


        public ButtonBase()
        {
            confirm = new SoundPlayer(@"Audio\FX\confirm");
            ConfirmFXWaitToEnd = false;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Selected)
            {
                if (Manager.UIInput.Hit("jump"))
                {
                    confirm.WaitToEnd = ConfirmFXWaitToEnd;
                    confirm.Play();
                    if (OnEnter != null) OnEnter(Group);
                }
                else if (Manager.UIInput.Hit("left") || Manager.UIInput.Hit("menu_left"))
                {
                    Value--;
                    if (OnChange != null) OnChange(Group, Value);
                }
                else if (Manager.UIInput.Hit("right") || Manager.UIInput.Hit("menu_right"))
                {
                    Value++;
                    if (OnChange != null) OnChange(Group, Value);
                }
            }

            base.Update(gameTime);
        }
    }
}