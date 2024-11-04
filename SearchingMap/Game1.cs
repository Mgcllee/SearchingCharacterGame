using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;

namespace SearchingMap
{
    public static class Global
    {
        public static GraphicsDeviceManager _graphics;
    }

    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        private Texture2D texture;
        private Dictionary<int, Sprite> players;

        private TcpClient _client;
        private NetworkStream _stream;

        public Game1()
        {
            Global._graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            StartReceivingDataAsync();
            players = new Dictionary<int, Sprite>();
            base.Initialize();
        }
        private void SetupWindows()
        {
            // 화면 해상도 초기화
            Global._graphics.PreferredBackBufferWidth = 1000;
            Global._graphics.PreferredBackBufferHeight = 1000;
            Global._graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("character");
        }

        private async void StartReceivingDataAsync()
        {
            _client = new TcpClient("127.0.0.1", 7777);
            _stream = _client.GetStream();

            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        string[] pos = receivedData.Split(" ");

                        int player_ticket = int.Parse(pos[0]);
                        if (players.ContainsKey(player_ticket))
                        {
                            players[player_ticket]._position.X = int.Parse(pos[1]);
                            players[player_ticket]._position.Y = int.Parse(pos[2]);
                        }
                        else
                        {
                            players.Add(player_ticket, new Sprite(texture)); 
                            players[player_ticket]._position.X = int.Parse(pos[1]);
                            players[player_ticket]._position.Y = int.Parse(pos[2]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("오류 발생: " + ex.Message);
            }
            finally
            {
                // 스트림과 클라이언트 닫기
                _stream.Close();
                _client.Close();
            }
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(players.Count != 0)
            {
                foreach (var player in players)
                {
                    player.Value.Update();
                }
            }
           

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            if (players.Count != 0)
            {
                foreach (var player in players)
                {
                    player.Value.Draw(_spriteBatch);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
