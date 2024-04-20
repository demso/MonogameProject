using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Nez.Farseer;

namespace FirstGame.Game
{
    internal class MyFSWorld : FSWorld
    {
        private float accumulator = 0;
        public override void Update()
        {
            if (EnableMousePicking)
            {
                if (Input.LeftMouseButtonPressed)
                {
                    var pos = Core.Scene.Camera.ScreenToWorldPoint(Input.MousePosition);
                    var fixture = World.TestPoint(FSConvert.DisplayToSim * pos);
                    if (fixture != null && !fixture.Body.IsStatic && !fixture.Body.IsKinematic)
                        _mouseJoint = fixture.Body.CreateFixedMouseJoint(pos);
                }

                if (Input.LeftMouseButtonDown && _mouseJoint != null)
                {
                    var pos = Core.Scene.Camera.ScreenToWorldPoint(Input.MousePosition);
                    _mouseJoint.WorldAnchorB = FSConvert.DisplayToSim * pos;
                }

                if (Input.LeftMouseButtonReleased && _mouseJoint != null)
                {
                    World.RemoveJoint(_mouseJoint);
                    _mouseJoint = null;
                }
            }

            //World.Step(1/30f);

            float frameTime = Time.DeltaTime;
            accumulator += frameTime;
            while (accumulator >= TimeStep)
            {
                MasterScene masterScene = Scene as MasterScene;
                if (masterScene != null)
                    masterScene.FixedUpdate();

                World.Step(TimeStep);

                accumulator -= TimeStep;
            }
        }
    }
}
