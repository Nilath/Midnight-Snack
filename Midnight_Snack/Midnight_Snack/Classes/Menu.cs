﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midnight_Snack
{
    public class Menu : GameObject
    {
        protected bool visible;   //Is the menu currently visible?
        private MenuSelector selector;  //The selector associated with this menu
        protected List<Text> menuOptions; //The options of the menu

        Player player = Player.GetInstance();
        GameManager gameManager = GameManager.GetInstance();

        public Menu(Vector2 pos, int width, int height, List<Text> options) : base(pos, width, height)
        {
            visible = true;
            selector = new MenuSelector(pos, width, 15, options.Count);

            menuOptions = options;
            int xOffset = 10;
            int yOffset = 0;
            for(int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].SetPosition(new Vector2(GetX() + xOffset, GetY() + yOffset));
                yOffset += 20;
            }

        }

        public override void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("action_menu_background.png");
            selector.LoadContent(content);
            for(int i = 0; i < menuOptions.Count; i++)
            {
                menuOptions[i].LoadContent(content);
            }
        }

        public virtual void Update(Controls controls)
        {
            //Update Selector and text
            selector.Update(controls);

            int option = selector.SelectAction(controls);
            PerformAction(GetOptionSelected(option));
        }

        public string GetOptionSelected(int option)
        {
            if(option > -1 && option <= menuOptions.Count)
            {
                return menuOptions[option].GetMessage();
            }
            return "";
        }

        public virtual void PerformAction(string action)
        {
            if (action.Equals("Start"))
            {
                //Load the level 
                //gameManager.SetGameState(1);
            }
            else if (action.Equals("Basic Tutorial"))
            {
                //Show the Basic Tutorial's level briefing
                gameManager.SetCurrentLevel(0);
                gameManager.SetGameState(5);
                //gameManager.SetGameState(1);
            }
            else if (action.Equals("Enemy Tutorial"))
            {
                //Show the Enemy Tutorial's level briefing
                gameManager.SetCurrentLevel(1);
                gameManager.SetGameState(5);
                //gameManager.SetGameState(1);
            }
            else if (action.Equals("Forms Tutorial"))
            {
                //Show the Tutorial's level briefing
                gameManager.SetCurrentLevel(2);
                gameManager.SetGameState(5);
                //gameManager.SetGameState(1);
            }
            else if (action.Equals("Level 1"))
            {
                //Load level 1
                gameManager.SetCurrentLevel(3);
                gameManager.SetGameState(5);
                 
            }
            else if (action.Equals("Level 2"))
            {
                //Load level 2
                gameManager.SetCurrentLevel(4);
                gameManager.SetGameState(5);

            }
            else if (action.Equals("Level 3"))
            {
                //Load level 3
                gameManager.SetCurrentLevel(5);
                gameManager.SetGameState(5);

            }
            else if (action.Equals("Level 4"))
            {
                //Load level 4
                gameManager.SetCurrentLevel(6);
                gameManager.SetGameState(5);

            }
            else if (action.Equals("Try Again"))
            {
                gameManager.ResetGameState();
                gameManager.SetGameState(6);
            }
            else if (action.Equals("Level Select"))
            {
                gameManager.ResetGameState();
                gameManager.SetGameState(0);
            }
            else if (action.Equals("Next Level"))
            {
                gameManager.ResetGameState();
                //Go to next level
                int nextLevel = gameManager.GetCurrentLevel() + 1;
                if(nextLevel > 6)
                {
                    nextLevel = 0;
                }
                gameManager.SetCurrentLevel(nextLevel);
                gameManager.SetGameState(5);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                //spriteBatch.Draw(texture, position, Color.White);
                selector.Draw(spriteBatch);
                for (int i = 0; i < menuOptions.Count; i++)
                {
                    menuOptions[i].Draw(spriteBatch);
                }
            }
        }

        public void SetVisible(bool b)
        {
            visible = b;
        }

        public void SetOptions(List<Text> options)
        {
            menuOptions = options;
        }
    }
}
