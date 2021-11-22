using Compression;

var h = new Huffman();
var message = new[]
{
    "ABC",  "ABC",  "ABC",  "ABC",
    "CDE",  "CDE",
    "XYZ",  "XYZ", "XYZ",
    "A",
    "B",
    "XYZ",
    "ABC"
};
var (encoded, tree) = h.Encode(message);
encoded.Print();

string[] decoded = h.Decode(encoded, tree);
decoded.ToList().ForEach(Console.WriteLine);