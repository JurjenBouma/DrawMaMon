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
using DrawMaMon.Structs;

namespace DrawMaMon
{
    class Animation
    {
        bool m_loop;
        bool m_wayBack = false;
        int m_maxSteps;
        Point m_animationState;
        int m_speed;

        public Animation(Point startState,bool loop,int maxSteps,int speed)
        {
            if(startState.X >=0 && startState.X < 3 && startState.Y >=0)
                m_animationState = startState;

            if (speed >= 0 && speed < 21)
                m_speed = 21 - speed;
            else
                m_speed = 21;
            m_loop = loop;
            m_maxSteps = maxSteps;
        }
        public Point GetState() { return m_animationState; }
        public void SetState(int state) { m_animationState.X = state; }
        public int GetSpeed() { return m_speed; }
        public void SetMaxSteps(int steps)
        {
            if(steps >=0)
                m_maxSteps = steps;
        }
        public void SetLoop(bool loop) { m_loop = loop; }


        public void Progress()
        {
            if (m_wayBack == false)
            {
                if (m_animationState.X + 1 <= m_maxSteps)
                    m_animationState.X++;
                else
                {
                    if (m_loop == true)
                        m_animationState.X = 0;
                    else
                    {
                        m_wayBack = true;
                        m_animationState.X--;
                    }
                }
            }

            if (m_wayBack == true)
            {
                if (m_animationState.X - 1 >= 0)
                    m_animationState.X--;
                else
                {
                    m_wayBack = false;
                    m_animationState.X++;
                }
            }
        }
    }
}
