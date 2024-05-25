


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PoseidonsWrath{


    public class Button{

        private string _name;
        private MouseState _currentMouse;
        private MouseState _previousMouse;
        private SpriteFont _font;
        private Boolean _isHovering;
        private Texture2D _texture;
        private Rectangle _icon;
        public Vector2 position { get; set; }

        public String text {get; set;}

        public Button(String name, Texture2D texture, Rectangle icon, SpriteFont spriteFont){
            _name = name;
            _icon = icon;
            _texture = texture;
            _font = spriteFont;
        }

        public void Draw(SpriteBatch spriteBatch){
            Color c = Color.White;
            if(_isHovering)
                c = Color.Blue;

            spriteBatch.Draw(_texture, position, _icon, c);
        }

        public void Update(Game1 game, GameTime gameTime){
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            Rectangle mouseBox = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering = false;

            if(mouseBox.Intersects(new Rectangle((int)position.X, (int)position.Y, _icon.Width, _icon.Height))){
                _isHovering = true;
                if(_currentMouse.LeftButton == ButtonState.Pressed && _currentMouse != _previousMouse){
                    Console.WriteLine("next wave");
                    game.waveSFX.Play(0.1f, 0.0f, 0.0f);
                    game.changeState(Game1.GameState.WavePhase);
                    
                }
            }


        }
    }
}