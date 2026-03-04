using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceGame1
{
    // Класс кубика: хранит имя, список граней и последний бросок.
    public class Dice
    {
        public string Name { get; set; }
        public List<int> Faces { get; set; }   // список значений граней (обычно длина 6)
        public int? LastRoll { get; set; }     // последняя выпавшая грань, nullable

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
            // ВАЖНО: создаём **новый** список из входного — это уже глубокое копирование списка
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

        // DeepCopy: создаёт полностью новый объект и новый список граней
        public Dice DeepCopy()
        {
            return new Dice(this.Name, new List<int>(this.Faces));
        }

        // Clone: если хочешь реализовать Prototype, делай этот метод.
        // В текущем проекте можно просто вызвать DeepCopy().
        public Dice Clone()
        {
            // Здесь мы просто делегируем DeepCopy — но семантически это метод прототипа.
            return DeepCopy();
        }

        public override string ToString()
        {
            return $"{Name}: [{string.Join(",", Faces)}]";
        }
    }
}