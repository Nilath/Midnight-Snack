﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Midnight_Snack
{
    public class MobileUnit : Unit
    {
        protected int moveRange;  //Number of tiles the unit can move in one turn
        protected int maxHealth;  //The greatest amount of health this unit can have
        protected int currentHealth;  //The current amount of health this unit has
        protected int strength;     //How much does this unit attack for?
        protected bool movedThisTurn; //Has the unit moved this turn?
        protected bool usedAbilityThisTurn;   //Has the unit used an ability this turn?
        protected bool alive; //Is the unit alive?
        public HealthBar healthBar; //The unit's health bar
        public Text attackStr;   //The unit's stats

        Map map = Map.GetInstance();

        public MobileUnit(Vector2 pos, int width, int height, int row, int col, int range, int health) : base(pos, width, height, row, col)
        {
            moveRange = range;
            maxHealth = health;
            currentHealth = health;
            strength = 3;
            movedThisTurn = false;
            usedAbilityThisTurn = false;
            alive = true;
            unitsTurn = false;
            healthBar = new HealthBar(new Vector2(position.X, position.Y - 10), maxHealth);
            attackStr = new Text("Strength: " + strength, new Vector2(position.X, position.Y + 75));
            attackStr.SetVisible(false);
        }

        //Moves the unit to the given position
        public void Move(Vector2 pos, int row, int col)
        {
            MapTile prevTile = map.GetTile(this.GetRow(), this.GetCol());
            prevTile.SetPassable(true);
            prevTile.SetOccupant(null);
            map.SetTile(this.GetRow(), this.GetCol(), prevTile);

            SetPosition(pos);
            SetRow(row);
            SetCol(col);

            MapTile newTile = map.GetTile(row, col);
            newTile.SetPassable(false);
            newTile.SetOccupant(this);
            map.SetTile(row, col, newTile);

            movedThisTurn = true;
        }

        public void UseAbility(string ability)
        {
            //Override in subclasses
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public void SetCurrentHealth(int health)
        {
            currentHealth = health;
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }

        public void SetMaxHealth(int health)
        {
            maxHealth = health;
        }

        public int GetStrength()
        {
            return strength;
        }

        public void SetStrength(int str)
        {
            strength = str;
        }

        public void DisplayAttackStr(bool b)
        {
            attackStr.SetVisible(b);
        }

        public int GetMoveRange()
        {
            return moveRange;
        }

        public void SetMoveRange(int range)
        {
            moveRange = range;
        }

        public bool HasMovedThisTurn()
        {
            return movedThisTurn;
        }

        public void SetMovedThisTurn(bool b)
        {
            movedThisTurn = b;
        }

        public bool HasUsedAbilityThisTurn()
        {
            return usedAbilityThisTurn;
        }

        public void SetUsedAbilityThisTurn(bool b)
        {
            usedAbilityThisTurn = b;
        }

        public bool IsAlive()
        {
            return alive;
        }

        public void SetAlive(bool b)
        {
            alive = b;
        }

        public Map GetMap()
        {
            return map;
        }

        public void SetMap(Map m)
        {
            this.map = m;
        }

        public MapTile GetCurrentMapTile()
        {
            return map.GetTile(this.GetRow(), this.GetCol());
        }

        public override void Update()
        {
            healthBar.Update(position, currentHealth);
            attackStr.SetPosition(new Vector2(position.X, position.Y - 20));

            if(currentHealth <= 0)
            {
                alive = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            healthBar.Draw(spriteBatch);
            attackStr.Draw(spriteBatch);
        }

        public virtual void Attack(MobileUnit target)
        {
            //Target must still be alive
            if (target.IsAlive())
            {
                //Update the target's health
                int targetHealth = target.GetCurrentHealth() - 3;
                target.SetCurrentHealth(targetHealth);

                //Update that unit has used an ability this turn
                this.SetUsedAbilityThisTurn(true);
            }
        }
    }
}
