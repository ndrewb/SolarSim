using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Stellaron
{
    public partial class Form1 : Form
    {
        
        private const double G = 6.67430e-11;

       
        private List<Planet> planets = new List<Planet>();
        private Timer timer;

        public Form1()
        {
            InitializeComponent();
            InitializeSimulation();
        }

        private void InitializeSimulation()
        {

            planets.Add(new Planet("Sun", 1.989e30, 0, 0, 0, 0, Color.Yellow, 2.0)); // Солнце увеличено в 2 раза
            planets.Add(new Planet("Earth", 5.972e24, 1.496e11, 0, 0, 29780, Color.Blue, 1.0)); // Земля
            planets.Add(new Planet("Mars", 6.39e23, 2.279e11, 0, 0, 24070, Color.Red, 0.53)); // Марс
            planets.Add(new Planet("Venus", 4.867e24, 1.082e11, 0, 0, 35020, Color.Orange, 0.95)); // Венера
            planets.Add(new Planet("Mercury", 3.285e23, 5.791e10, 0, 0, 47360, Color.Gray, 0.38)); // Меркурий
        


        timer = new Timer
            {
                Interval = 1
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SimulateStep(60 * 60*24/10); // шаг моделирования - 1 сутки
            Invalidate(); 
        }

        private void SimulateStep(double dt)
        {
            foreach (var planet in planets)
            {
                Vector2 force = new Vector2(0, 0);
                foreach (var other in planets)
                {
                    if (planet != other)
                    {
                        force += planet.CalculateGravitationalForce(other);
                    }
                }
                planet.UpdatePosition(force, dt);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (var planet in planets)
            {
                planet.Draw(e.Graphics, Width / 2, Height / 2, 2*1e9);
            }
        }
    }

    public class Planet
    {
        public string Name { get; }
        public double Mass { get; }
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        private Color Color { get; }
        private double RadiusScale { get; } // Новый параметр: размер планеты относительно Земли
        public Planet(string name, double mass, double x, double y, double vx, double vy, Color color, double radiusScale)
        {
            Name = name;
            Mass = mass;
            Position = new Vector2(x, y);
            Velocity = new Vector2(vx, vy);
            Color = color;
            RadiusScale = radiusScale;
        }

        public Vector2 CalculateGravitationalForce(Planet other)
        {
            Vector2 direction = other.Position - Position;
            double distance = direction.Length();
            double forceMagnitude = (6.67430e-11 * Mass * other.Mass) / (distance * distance);
            return direction.Normalize() * forceMagnitude;
        }

        public void UpdatePosition(Vector2 force, double dt)
        {
            Vector2 acceleration = force / Mass;
            Velocity += acceleration * dt;
            Position += Velocity * dt;
        }

        public void Draw(Graphics g, int centerX, int centerY, double scale)
        {
            int x = centerX + (int)(Position.X / scale);
            int y = centerY - (int)(Position.Y / scale);

            // Учитываем радиус планеты на основе масштаба
            int radius = (int)(10 * RadiusScale); // Земля — "стандартный" размер (5 пикселей)
            g.FillEllipse(new SolidBrush(Color), x - radius, y - radius, 2 * radius, 2 * radius);
        }
    }

    public struct Vector2
    {
        public double X { get; }
        public double Y { get; }

        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Length() => Math.Sqrt(X * X + Y * Y);

        public Vector2 Normalize()
        {
            double length = Length();
            return new Vector2(X / length, Y / length);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
        public static Vector2 operator *(Vector2 a, double scalar) => new Vector2(a.X * scalar, a.Y * scalar);
        public static Vector2 operator /(Vector2 a, double scalar) => new Vector2(a.X / scalar, a.Y / scalar);
    }
}
