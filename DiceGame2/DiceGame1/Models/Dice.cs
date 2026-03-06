using System;
using System.Collections.Generic;

namespace DiceGame1
{
    // Класс кубика: хранит имя, список граней и последний бросок.
    public class Dice
    {
        public string Name { get; set; }
        public List<int> Faces { get; set; } // список значений граней (обычно длина 6)
        public int? LastRoll { get; set; } // последняя выпавшая грань, nullable

        private static readonly Random _rng = new Random();

        public Dice()
        {
            Name = "Оригинал";
            Faces = new List<int> { 1, 2, 3, 4, 5, 6 };
            LastRoll = null;
        }

        public Dice(string name, IEnumerable<int> faces)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Оригинал" : name;
            Faces = faces == null ? new List<int> { 1, 2, 3, 4, 5, 6 } : new List<int>(faces);
            LastRoll = null;
        }

        // бросок: возвращает одно случайное значение из Faces и запоминает его
        public int RollSingleFace()
        {
            if (Faces == null || Faces.Count == 0) return 0;
            int idx = _rng.Next(Faces.Count);
            int val = Faces[idx];
            LastRoll = val;
            return val;
        }

        // Prototype: создаёт новый объект кубика с копией граней
        public Dice Clone()
        {
            var clone = new Dice(this.Name, new List<int>(this.Faces));
            clone.LastRoll = null; // сбрасываем последний бросок для нового объекта
            return clone;
        }
    }
}