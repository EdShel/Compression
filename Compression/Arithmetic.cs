using Rationals;

namespace Compression
{
    public class Arithmetic
    {
        private const string END = "\0";

        public (Rational, IEnumerable<(string Word, Rational Probability)>) Encode(string[] message)
        {
            var zeroTerminatedMessage = message.Append(END).ToList();
            var wordsProbabilities = zeroTerminatedMessage
                .GroupBy(k => k)
                .ToDictionary(k => k.Key, v => new Rational(v.Count(), zeroTerminatedMessage.Count))
                .OrderByDescending(k => k.Value)
                .Select(k => (Word: k.Key, Probability: k.Value))
                .ToList();

            var wordsRanges = wordsProbabilities
                .Select((w, i) => (
                    Word: w.Word,
                    Left: wordsProbabilities.Take(i).Select(w => w.Probability).Aggregate(Rational.Zero, (Rational aggr, Rational x) => aggr + x),
                    Right: wordsProbabilities.Take(i + 1).Select(w => w.Probability).Aggregate(Rational.Zero, (Rational aggr, Rational x) => aggr + x)
                ))
                .ToList();

            Rational left = Rational.Zero;
            Rational right = Rational.One;

            foreach (var word in zeroTerminatedMessage)
            {
                var wordRange = wordsRanges.First(w => w.Word == word);
                Rational length = right - left;
                Rational newLeft = left + length * wordRange.Left;
                Rational newRight = left + length * wordRange.Right;
                left = newLeft;
                right = newRight;
            }

            return (left, wordsProbabilities);
        }

        public string[] Decode(Rational number, IEnumerable<(string Word, Rational Probability)> wordsProbabilities)
        {
            var wordsRanges = wordsProbabilities
                .Select((w, i) => (
                    Word: w.Word,
                    Left: wordsProbabilities.Take(i).Select(w => w.Probability).Aggregate(Rational.Zero, (Rational aggr, Rational x) => aggr + x),
                    Right: wordsProbabilities.Take(i + 1).Select(w => w.Probability).Aggregate(Rational.Zero, (Rational aggr, Rational x) => aggr + x)
                ))
                .ToList();

            var decoded = new List<string>();
            Rational left = Rational.Zero;
            Rational right = Rational.One;

            while(true)
            {
                Rational length = (right - left);
                Rational numberToCurrentRange = (number - left) / length;
                var wordRange = wordsRanges.First(w => w.Right > numberToCurrentRange);
                if (wordRange.Word == END)
                {
                    break;
                }
                decoded.Add(wordRange.Word);

                Rational newLeft = left + length * wordRange.Left;
                Rational newRight = left + length * wordRange.Right;
                left = newLeft;
                right = newRight;
            }

            return decoded.ToArray();
        }
    }
}
