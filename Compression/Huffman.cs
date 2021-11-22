using System.Collections;

namespace Compression
{
    public class Huffman
    {
        public (BitArray, WordFrequency) Encode(string[] words)
        {
            if (words == null || words.Length == 0)
            {
                throw new ArgumentNullException(nameof(words));
            }

            var wordsList = words.GroupBy(k => k).Select(w => new WordFrequency
            {
                Word = w.Key,
                Frequency = w.Count()
            }).ToList();
            while (wordsList.Count > 1)
            {
                var firstLeastFrequent = wordsList.MinBy(w => w.Frequency)!;
                wordsList.Remove(firstLeastFrequent);
                var secondLeastFrequent = wordsList.MinBy(w => w.Frequency)!;
                wordsList.Remove(secondLeastFrequent);

                wordsList.Add(new WordFrequency
                {
                    Left = firstLeastFrequent,
                    Right = secondLeastFrequent,
                    Frequency = firstLeastFrequent.Frequency + secondLeastFrequent.Frequency
                });
            }

            var root = wordsList.First();
            var mappings = new Dictionary<string, BitArray>();
            AddLeafNodesToDictionary(root, mappings, new BitArray(0));

            var encoded = new BitArray(0);
            foreach (var word in words)
            {
                var wordCode = mappings[word];
                encoded = encoded.Append(wordCode);
            }

            return (encoded, root);
        }

        private void AddLeafNodesToDictionary(WordFrequency root, IDictionary<string, BitArray> mappings, BitArray prefix)
        {
            if (root.Word != null)
            {
                mappings[root.Word] = prefix;
                return;
            }

            var leftCode = prefix.Append(new BitArray(new[] { false }));
            AddLeafNodesToDictionary(root.Left, mappings, leftCode);
            var rightCode = prefix.Append(new BitArray(new[] { true }));
            AddLeafNodesToDictionary(root.Right, mappings, rightCode);
        }

        public string[] Decode(BitArray data, WordFrequency treeRoot)
        {
            var bools = new bool[data.Count];
            data.CopyTo(bools, 0);

            var decoded = new List<string>();
            var current = treeRoot;
            for (int bitIndex = 0; bitIndex < bools.Length; bitIndex++)
            {
                bool bitValue = bools[bitIndex];
                if (bitValue)
                {
                    current = current.Right;
                }
                else
                {
                    current = current.Left;
                }

                if (current.Word != null)
                {
                    decoded.Add(current.Word);
                    current = treeRoot;
                }
            }

            return decoded.ToArray();
        }
    }

    public class WordFrequency
    {
        public string Word;
        public int Frequency;
        public WordFrequency Left;
        public WordFrequency Right;
    }

    public static class BitHelper
    {
        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        public static void Print(this BitArray bitArray)
        {
            var bools = new bool[bitArray.Count];
            bitArray.CopyTo(bools, 0);
            foreach (var bit in bools)
            {
                Console.Write(bit ? "1" : "0");
            }
            Console.WriteLine();
        }
    }
}
