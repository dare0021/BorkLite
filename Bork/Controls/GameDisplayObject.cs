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

        protected double speed, rotationSpeed, maxSpeed, maxRotationSpeed;
        public double Speed
        {
            get
            {
                return speed;
            }
            set
            {
                if (value > MaxSpeed)
                    value = MaxSpeed;
                if (value < -MaxSpeed)
                    value = -MaxSpeed;
                speed = value;
            }
        }
        public double RotationSpeed
        {
            get
            {
                return rotationSpeed;
            }
            set
            {
                if (value > MaxRotationSpeed)
                    value = MaxRotationSpeed;
                if (value < -MaxRotationSpeed)
                    value = -MaxRotationSpeed;
                rotationSpeed = value;
            }
        }
        public double MaxSpeed { get; set; }
        public double MaxRotationSpeed { get; set; }

        protected LinkedList<string> tags;

        public bool isInvulnerable()
        {
            return tags.Contains("invulnerable");
        }
        public void isInvulnerable(bool v)
        {
            if (tags.Contains("invulnerable") && !v)
            {
                tags.Remove("invulnerable");
            }
            else if (!tags.Contains("invulnerable") && v)
            {
                tags.AddLast("invulnerable");
            }
        }
    }
}
