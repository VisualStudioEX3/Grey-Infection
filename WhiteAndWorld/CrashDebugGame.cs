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

public class CrashDebugGame : Game
{
    private SpriteBatch spriteBatch;
    private SpriteFont font;
    private readonly Exception exception;
    private Vector2 origin;
    private GraphicsDeviceManager graphics;
    private string output;
    private string _spriteFont;

    /// <summary>
    /// Constructor de la clase.
    /// </summary>
    /// <param name="exception">Excepcion a mostrar en pantalla.</param>
    /// <param name="spriteFont">Asset de la fuente de texto que se usara en pantalla.</param>
    public CrashDebugGame(Exception exception, string spriteFont)
    {
        this.exception = exception;
        this._spriteFont = spriteFont;

        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        output = "*** PROGRAM EXCEPTION *** \n\n" +
                 "Press Back to Exit. \n\n" +
                 string.Format("Exception: {0}\n\n", exception.Message) +
                 string.Format("Stack Trace:\n{0}", exception.StackTrace);
    }

    protected override void Initialize()
    {
        origin = new Vector2(graphics.GraphicsDevice.DisplayMode.TitleSafeArea.X - 40, 
                             graphics.GraphicsDevice.DisplayMode.TitleSafeArea.Y - 20);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        font = Content.Load<SpriteFont>(_spriteFont);
        spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
            (GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed) ||
            (GamePad.GetState(PlayerIndex.Three).Buttons.Back == ButtonState.Pressed) ||
            (GamePad.GetState(PlayerIndex.Four).Buttons.Back == ButtonState.Pressed))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkBlue);

        spriteBatch.Begin();
        spriteBatch.DrawString(font, output, origin, Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }
}