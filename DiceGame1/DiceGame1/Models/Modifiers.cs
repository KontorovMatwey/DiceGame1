using System;
using System.Collections.Generic;

namespace DiceGame1
{
    // Интерфейс модификатора — модификатор меняет конкретный экземпляр Dice
    public interface IModifier
    {
        string Name { get; }
        string Description { get; }
        void Apply(Dice target);
    }

    // Простейшие модификаторы — +1 ко всем, -1 ко всем, все 6, случайно и т.д.
    public class AddOneModifier : IModifier
    {
        public string Name => "+1 ко всем (макс 6)";
        public string Description => "Прибавляет +1 ко всем граням (макс 6).";
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            for (int i = 0; i < target.Faces.Count; i++)
            {
                int v = target.Faces[i] + 1;
                if (v > 6) v = 6;
                target.Faces[i] = v;
            }
        }
    }

    public class SubOneModifier : IModifier
    {
        public string Name => "-1 ко всем (мин 1)";
        public string Description => "Вычитает 1 из каждой грани (мин 1).";
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            for (int i = 0; i < target.Faces.Count; i++)
            {
                int v = target.Faces[i] - 1;
                if (v < 1) v = 1;
                target.Faces[i] = v;
            }
        }
    }

    public class AllSixModifier : IModifier
    {
        public string Name => "Все 6";
        public string Description => "Устанавливает все грани в 6.";
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            for (int i = 0; i < target.Faces.Count; i++) target.Faces[i] = 6;
        }
    }

    public class RandomizeModifier : IModifier
    {
        public string Name => "Случайно";
        public string Description => "Заменяет все грани случайными числами 1..6.";
        private static readonly Random _rng = new Random();
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            for (int i = 0; i < target.Faces.Count; i++) target.Faces[i] = _rng.Next(1, 7);
        }
    }

    public class AllOneModifier : IModifier
    {
        public string Name => "Все 1";
        public string Description => "Устанавливает все грани в 1.";
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            for (int i = 0; i < target.Faces.Count; i++) target.Faces[i] = 1;
        }
    }

    // Более детерминированные варианты вместо "случайных трёх"
    public class AddToFirstThreeModifier : IModifier
    {
        public string Name => "+2 к первым трём";
        public string Description => "Добавляет +2 к первым трём граням (макс 6).";
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            int n = Math.Min(3, target.Faces.Count);
            for (int i = 0; i < n; i++)
            {
                int v = target.Faces[i] + 2;
                if (v > 6) v = 6;
                target.Faces[i] = v;
            }
        }
    }

    public class AddToLastThreeModifier : IModifier
    {
        public string Name => "+2 к последним трём";
        public string Description => "Добавляет +2 к последним трём граням (макс 6).";
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            int n = Math.Min(3, target.Faces.Count);
            int start = Math.Max(0, target.Faces.Count - n);
            for (int i = start; i < target.Faces.Count; i++)
            {
                int v = target.Faces[i] + 2;
                if (v > 6) v = 6;
                target.Faces[i] = v;
            }
        }
    }

    public class DoubleOneFaceModifier : IModifier
    {
        public string Name => "×2 к одной";
        public string Description => "Умножает одну (среднюю) грань на 2 (макс 6).";
        public void Apply(Dice target)
        {
            if (target?.Faces == null) return;
            int idx = target.Faces.Count / 2;
            int v = target.Faces[idx] * 2;
            if (v > 6) v = 6;
            target.Faces[idx] = v;
        }
    }

    // Пул всех модификаторов (UI берёт случайные 3)
    public static class ModifierPool
    {
        public static List<IModifier> All()
        {
            return new List<IModifier>
            {
                new AddOneModifier(),
                new SubOneModifier(),
                new AddToFirstThreeModifier(),
                new AddToLastThreeModifier(),
                new AllSixModifier(),
                new RandomizeModifier(),
                new AllOneModifier(),
                new DoubleOneFaceModifier()
            };
        }
    }
}