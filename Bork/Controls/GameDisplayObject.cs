using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bork.Helpers;

namespace Bork.Controls
{
    /// <summary>
    /// Parent class for all game objects (ships, misiles, etc)
    /// </summary>
    class GameDisplayObject : RichImage
    {
        public GameDisplayObject()
        {

        }

        public void Update(double dt)
        {
            var rotation = getRotation();
            var effectiveSpeed = Speed * dt;
            var dx = effectiveSpeed * Math.Sin(rotation * Math.PI / 180);
            var dy = effectiveSpeed * Math.Cos(rotation * Math.PI / 180);
            setPosition(getPosition() + new Vec2(dx, -dy)); //Why -ve? who knows?
            setRotation(rotation + RotationSpeed * dt);
        }

        protected double speed, rotationSpeed;
        public double Speed { get; set; }
        public double RotationSpeed { get; set; }
    }
}
