using Compression;

var a = new Arithmetic();
var message = new[]
{
    "a", 
};
var (encoded, probablities) = a.Encode(message);
Console.WriteLine(encoded.ToString("C"));

string[] decoded = a.Decode(encoded, probablities);

//var h = new Huffman();
//var message = new[]
//{
//    "ABC",  "ABC",  "ABC",  "ABC",
//    "CDE",  "CDE",
//    "XYZ",  "XYZ", "XYZ",
//    "A",
//    "B",
//    "XYZ",
//    "ABC"
//};
//var (encoded, tree) = h.Encode(message);
//encoded.Print();

//string[] decoded = h.Decode(encoded, tree);
decoded.ToList().ForEach(Console.WriteLine);