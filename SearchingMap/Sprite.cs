﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel.Design;

namespace SearchingMap
{
    class Sprite
    {
        private Texture2D _texture;
        public Vector2 _position;
        public int dir = 1;

        private int[] dir_row = new int[8] { -1, -1, 0, 1, 1, 1, 0, -1 };
        private int[] dir_col = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };

        public Sprite(Texture2D texture)
        {
            this._texture = texture;
            _position = new Vector2(100, 100);
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && 2 <= _position.Y)
            {
                // up
                _position.Y -= 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && _position.Y + 100 <= Global._graphics.PreferredBackBufferHeight)
            {
                // down
                _position.Y += 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left) && 2 <= _position.X)
            {
                // left
                _position.X -= 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right) && _position.X + 100 <= Global._graphics.PreferredBackBufferWidth)
            {
                // right
                _position.X += 2;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this._texture, _position, Color.White);
        }
    }
}
