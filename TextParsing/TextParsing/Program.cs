// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using TextParsing.Model;



Stopwatch stopWatch = new Stopwatch();
stopWatch.Start();
while (true)
{
    TimeSpan ts = stopWatch.Elapsed;
    if (ts.Seconds > 2)
    {
        Console.WriteLine("2 secs elapsed!");
        stopWatch.Stop();
        break;
    }
}
string operation = "(Location == Loc) And (SensorLocation == Sensor) And (Direction.StartWith(ABC)) And (Direction.EndWith(ABC)) Or (Location.Contains(ABC)))";

//string[] operands = Regex.Split(operation, @"\s+");
string[] operands = Regex.Split(operation, @"([A-Z]+)|(==)|(\()|(\))|(!=)", RegexOptions.IgnoreCase);
operands = operands.Where(c => c != "" && c != " " && c != ".").ToArray();

CultureInfo currCulture = CultureInfo.CurrentCulture;
string dateStr = "2025.01.18 4:16:22";
string[] dateStrArr = Regex.Split(dateStr, @"(-)|(:)|(\s)|(\.)");
dateStrArr = dateStrArr.Where(c => c != " " && c != "-" && c != ":" && c != ".").ToArray();
string inputStr = "((Location == Loc1) And (SensorLocation == Sensor1)) And ((Location != Loc) Or (Direction == YZ))";
inputStr = "((Location == Loc1) And (SensorLocation == Sensor1) And (Location != Loc)) Or (Direction.StartWith(ABC))";
inputStr = "((Location.Contains(ABC)) And (Direction.StartWith(YZ))) Or ((SensorLocation == ABC) And (Direction != Y))";
inputStr = "((Sum >= 3) Or (Date > 01.30.2025 4:16:22) Or (Location.StartWith(Loc))) And ((Direction != Y) Or (Location.StartWith(Loc2)))";
//inputStr = "(Location == Loc1) And ((Direction != Y) Or (Location == M))";
//List<Token> tokens = Demo.Lex(inputStr);
var parser = new Parser(new TextSplitter(inputStr), new CultureInfo("en-US"));

Channel ch = new Channel() { Location = "Loc", Direction = "YZ", SensorLocation = "aaa", Date = DateTime.Now, Sum = 5, DecimalSum = 3.01m };


var channels = new List<Channel>()
 {
        new Channel() { Location = "Loc", Direction = "Y", SensorLocation = "abc", Sum = 1, Date = new DateTime(2025,1,30), DecimalSum = 2m },
        new Channel() { Location = "Loc1", Direction = "Y", SensorLocation = "ABC", Sum = 2 , Date = new DateTime(2024,12,3)},
        new Channel() { Location = "Loc2", Direction = "YZ", SensorLocation = "CCC", Sum = 3, Date = new DateTime(2024,12,31) },
        new Channel() { Location = "Loc3", Direction = "Y", SensorLocation = "abc" , Sum = 5, Date = new DateTime(2025,1,31)},
        new Channel() { Location = "Loc4", Direction = "YZ", SensorLocation = "ABC", Sum = 6, Date = new DateTime(2024,11,8) }
 };

bool result = parser.Evaluate(ch);
var channelList = new List<Channel>(channels.Where(c => parser.Evaluate<Channel>(c)));

var persons = new List<Person>()
 {
        new Person() { Id = 1, Name = "Name1", Address ="Address1", Age = 30, Date= new DateTime(1995, 9, 3) },
        new Person() { Id = 2, Name = "Name2", Address ="Address2", Age = 28, Date= new DateTime(1997, 3, 1) },
        new Person() { Id = 3, Name = "Name3", Address ="Address3", Age = 4, Date= new DateTime(2021, 1, 30) },
        new Person() { Id = 4, Name = "NoName1", Address ="Address4", Age = 10, Date= new DateTime(2015, 8, 10) },
        new Person() { Id = 5, Name = "NoName2", Address ="Address5", Age = 15, Date= new DateTime(2010, 6, 20) },
        new Person() { Id = 6, Name = "FullName1", Address ="Address4", Age = 30, Date= new DateTime(1995, 10, 10) },
        new Person() { Id = 7, Name = "FullName2", Address ="Address5", Age = 25, Date= new DateTime(2000, 5, 29) },
        new Person() { Id = 8, Name = "LastName1", Address ="Address9", Age = 50, Date= new DateTime(1975, 12, 10) },
        new Person() { Id = 9, Name = "LastName2", Address ="Address10", Age = 60, Date= new DateTime(1965, 2, 28) },
 };

inputStr = "(((Age >= 10) And (Name.Contains(8))) Or (Name.StartWith(Full))) Or (Name.StartWith(Last))";
parser = new Parser(new TextSplitter(inputStr), new CultureInfo("en-US"));

var filteredPersons = new List<Person>(persons.Where(p => parser.Evaluate<Person>(p)));

persons = new List<Person>();
Random rnd = new Random();
DateTime startDateTime = new DateTime(1968, 12, 3);
for (int j = 1; j < 100; j++)
{
    int rndNumber = rnd.Next(100);
    startDateTime =startDateTime.AddMonths(rndNumber).AddDays(rndNumber);
    persons.Add(new Person() { Id = j, Name = $"Name{j.ToString()}", Address = $"Address{j.ToString()}", Age = rndNumber, Date = startDateTime });
}
filteredPersons = new List<Person>(persons.Where(p => parser.Evaluate<Person>(p)));
Console.WriteLine("Hello");