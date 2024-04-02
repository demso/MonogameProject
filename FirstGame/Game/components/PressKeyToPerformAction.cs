using Microsoft.Xna.Framework.Input;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstGame.Game.components
{
    public class PressKeyToPerformAction : Component, IUpdatable
    {
        Keys _key;
        Action<Entity> _action;


        public PressKeyToPerformAction(Keys key, Action<Entity> action)
        {
            _key = key;
            _action = action;
        }


        void IUpdatable.Update()
        {
            if (Input.IsKeyPressed(_key))
                _action(Entity);
        }
    }
}
