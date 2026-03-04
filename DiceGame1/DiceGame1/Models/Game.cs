using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiceGame1
{
    // Небольшая модель таргета и результата
    public class Target { public int Min { get; set; } public int Max { get; set; } public Target(int min, int max) { Min = min; Max = max; } }
    public class RoundResult { public int RoundNumber; public int SumOfRolls; public bool Success; public int PointsGained; }

    public class GameManager
    {
        public Dice OriginalPrototype { get; private set; }
        public List<Dice> AllDice { get; private set; } = new List<Dice>();
        public int RoundNumber { get; private set; } = 1;
        public int Score { get; private set; } = 0;
        public int BestScore { get; private set; } = 0;

        private readonly string bestFile = "bestscore.txt";
        private readonly Random _rng = new Random();

        private readonly Dictionary<int, Target> _targets = new Dictionary<int, Target>();
        private readonly Dictionary<int, int> _pointsForRound = new Dictionary<int, int>();

        private const int PerDieLeft = 2;
        private const int PerDieRight = 5;

        public GameManager()
        {
            LoadBest();
            // генерируем цель 1-го раунда ДО создания прототипа — чтобы UI показывал её
            GetOrCreateTarget(1);
        }

        public Target GetTargetForRound(int r) => GetOrCreateTarget(r);
        public int GetPointsForRound(int r) { if (_pointsForRound.ContainsKey(r)) return _pointsForRound[r]; GetOrCreateTarget(r); return _pointsForRound.ContainsKey(r) ? _pointsForRound[r] : 1; }

        // Устанавливаем прототип, создаём первый отображаемый экземпляр
        public void SetOriginalPrototype(Dice proto)
        {
            if (proto == null) throw new ArgumentNullException(nameof(proto));
            // создаём новый объект на основе переданных данных — deep copy
            OriginalPrototype = new Dice(proto.Name, proto.Faces);

            // сбрасываем поле и добавляем 1 экземпляр (отдельный объект)
            AllDice.Clear();
            var instance = new Dice(OriginalPrototype.Name, OriginalPrototype.Faces);
            instance.Name = OriginalPrototype.Name; // имя, введённое пользователем
            instance.LastRoll = null;
            AllDice.Add(instance);

            RoundNumber = 1;
            Score = 0;
            _pointsForRound.Clear();

            // цель для 1-го раунда остаётся та, что была сгенерирована при создании GameManager/Новой игры
            GetOrCreateTarget(1);
        }

        // Создать новые клоны для следующего раунда — применить модификатор только к новым клонам
        public int CreateNextRoundClones(IModifier chosenModifier)
        {
            RoundNumber++;
            int newCount = (int)Math.Pow(2, RoundNumber - 2); // 1,2,4,8...
            var newClones = new List<Dice>();
            for (int i = 0; i < newCount; i++)
            {
                // здесь мы **клонируем прототип**. Текущее выражение:
                var clone = OriginalPrototype.DeepCopy();       // <- deep copy
                clone.Name = $"Клон {OriginalPrototype.Name} R{RoundNumber}-{i + 1}";
                clone.LastRoll = null;
                chosenModifier?.Apply(clone);                  // применяем модификатор только к новым клонaм
                newClones.Add(clone);
            }
            AllDice.AddRange(newClones);

            // гарантируем, что таргет и очки для этого раунда созданы
            GetOrCreateTarget(RoundNumber);
            return newCount;
        }

        // бросаем все кубики и возвращаем результат
        public RoundResult ExecuteRound()
        {
            var t = GetOrCreateTarget(RoundNumber);
            int sum = 0;
            foreach (var d in AllDice) sum += d.RollSingleFace();

            bool success = sum >= t.Min && sum <= t.Max;
            int pts = _pointsForRound.ContainsKey(RoundNumber) ? _pointsForRound[RoundNumber] : 1;
            if (success) Score += pts;
            if (Score > BestScore) { BestScore = Score; SaveBest(); }

            return new RoundResult { RoundNumber = RoundNumber, SumOfRolls = sum, Success = success, PointsGained = success ? pts : 0 };
        }

        // алгоритм генерации таргетов по твоей логике (комментарии в коде)
        private Target GetOrCreateTarget(int round)
        {
            if (_targets.ContainsKey(round)) return _targets[round];

            if (round <= 1)
            {
                // раунд 1: базовые границы 1..6, затем сжимаем с обеих сторон на случай 0..1
                int baseMin = 1, baseMax = 6;
                int leftShrink = _rng.Next(0, 2);   // 0..1
                int rightShrink = _rng.Next(0, 2);  // 0..1
                int min = baseMin + leftShrink;
                int max = baseMax - rightShrink;
                if (min < 1) min = 1;
                if (max < min + 1) max = min + 1;
                var tgt = new Target(min, max);
                _targets[round] = tgt;
                _pointsForRound[round] = Math.Max(1, 1 + leftShrink + rightShrink); // очки = 1 + сжатия
                return tgt;
            }
            else
            {
                // для последующих раундов: увеличиваем границы от предыдущей цели
                var prev = GetOrCreateTarget(round - 1);
                int addedCount = (int)Math.Pow(2, round - 2); // 1,2,4,8...
                int baseMin = prev.Min + PerDieLeft * addedCount;     // прибавляется 2*addedCount
                int baseMax = prev.Max + PerDieRight * addedCount;    // прибавляется 5*addedCount
                int leftShrink = _rng.Next(0, addedCount + 1);   // 0..addedCount
                int rightShrink = _rng.Next(0, addedCount + 1);
                int min = baseMin + leftShrink;
                int max = baseMax - rightShrink;
                if (min < 1) min = 1;
                if (max < min + 1) max = min + 1;
                var tgt = new Target(min, max);
                _targets[round] = tgt;
                _pointsForRound[round] = Math.Max(1, 1 + leftShrink + rightShrink);
                return tgt;
            }
        }

        private void LoadBest()
        {
            try
            {
                if (File.Exists(bestFile))
                {
                    var s = File.ReadAllText(bestFile);
                    if (int.TryParse(s, out int v)) BestScore = v;
                }
            }
            catch { BestScore = 0; }
        }
        private void SaveBest()
        {
            try { File.WriteAllText(bestFile, BestScore.ToString()); } catch { }
        }
    }
}