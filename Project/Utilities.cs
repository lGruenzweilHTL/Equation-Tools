using System.Xml;

namespace Utilities{

public static class Logger{
    public static void ColorInfo(){
        Console.WriteLine("\n");
        Console.WriteLine("*----------Color Info----------*");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("| Blue is for requesting INPUT |");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("|    Yellow is for WARNINGS    |");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("|       Red is for ERRORS      |");
        Console.ForegroundColor = defaultColor;
        Console.WriteLine("*------------------------------*");
        Console.WriteLine("\n");
    }

    private static ConsoleColor defaultColor = ConsoleColor.White;
    public static void SetConsoleDefaultColor(ConsoleColor color){
        Console.ForegroundColor = color;
        defaultColor = color;
    }
    public static void LogPrompt(string msg){
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Input Request: " + msg);
        Console.ForegroundColor = defaultColor;
    }
    public static void LogWarning(string msg){
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Warning: " + msg);
        Console.ForegroundColor = defaultColor;
    }
    public static void LogError(string msg){
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Error: " + msg);
        Console.ForegroundColor = defaultColor;
    }

    public static void Table(string[,] content){

        for (int i = 0; i < content.GetLength(1); i++)
        {
            for (int j = 0; j < content.GetLength(0); j++)
            {
                string p = content[i, j];
                Console.Write($"{p,10:f5}|");
            }
            Console.WriteLine();
        }
    }
    
    #region DottedFont

    private static char[] alphabet = new char[]{
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',/*Q missing*/ 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', ' '
    };
    private static string[][] dotFont = new string[][]{
        new string[]{
            "  . .  ",
            ".     .",
            ". . . .",
            ".     .",
            ".     ."
        },
        new string[]{
            ". . .  ",
            ".     .",
            ". . .  ",
            ".     .",
            ". . .  "
        },
        new string[]{
            "  . . .",
            ".      ",
            ".      ",
            ".      ",
            "  . . ."
        },
        new string[]{
            ". . .  ",
            ".     .",
            ".     .",
            ".     .",
            ". . .  "
        },
        new string[]{
            "  . . .",
            ".      ",
            ". . . .",
            ".      ",
            "  . . ."
        },
        new string[]{
            "  . . .",
            ".      ",
            ". . . .",
            ".      ",
            ".      "
        },
        new string[]{
            "  . . .",
            ".      ",
            ".   . .",
            ".     .",
            "  . . ."
        },
        new string[]{
            ".     .",
            ".     .",
            ". . . .",
            ".     .",
            ".     ."
        },
        new string[]{
            " . . . ",
            "   .   ",
            "   .   ",
            "   .   ",
            " . . . "
        },
        new string[]{
            "  . . .",
            "      .",
            "      .",
            "      .",
            ". . .  "
        },
        new string[]{
            ".     .",
            ".     .",
            ". . .  ",
            ".     .",
            ".     ."
        },
        new string[]{
            ".      ",
            ".      ",
            ".      ",
            ".      ",
            ". . . ."
        },
        new string[]{
            ".       .",
            ". .   . .",
            ".   .   .",
            ".       .",
            ".       ."
        },
        new string[]{
            ".       .",
            ". .     .",
            ".   .   .",
            ".     . .",
            ".       ."
        },
        new string[]{
            "   . .   ",
            " .     . ",
            " .     . ",
            " .     . ",
            "   . .   "
        },
        new string[]{
            " . . .   ",
            " .     . ",
            " . . .   ",
            " .        ",
            " .        "
        },
        //Q IS MISSING (HAVE TO MAKE IT ONE LINE TALLER TO WORK)
        new string[]{
            " . . .   ",
            " .     . ",
            " . . .   ",
            " .     . ",
            " .     . "
        },
        new string[]{
            "   . . . ",
            " .       ",
            "   . .   ",
            "       . ",
            " . . .   "
        },
        new string[]{
            ". . . . .",
            "    .    ",
            "    .    ",
            "    .    ",
            "    .    "
        },
        new string[]{
            " .     . ",
            " .     . ",
            " .     . ",
            " .     . ",
            "   . .   "
        },
        new string[]{
            ".       .",
            ".       .",
            "  .   .  ",
            "  .   .  ",
            "    .    "
        },
        new string[]{
            ".       .",
            ".       .",
            ".   .   .",
            ". .   . .",
            ".       ."
        },
        new string[]{
            ".       .",
            "  .   .  ",
            "    .    ",
            "  .   .  ",
            ".       ."
        },
        new string[]{
            ".       .",
            ".       .",
            "  . . .  ",
            "    .    ",
            "    .    "
        },
        new string[]{
            ". . . .",
            "    .  ",
            "  .    ",
            ".      ",
            ". . . ."
        },
        new string[]{
            "   ",
            "   ",
            "   ",
            "   ",
            "   "
        }
    };

    private static Dictionary<char, string[]> characters = new();
    private static bool initialzed = false;

    public static void DottedWriteLine(string msg){
        if(!initialzed){
            Init();
            initialzed = true;
        }
        msg = msg.ToLower();
        try{
            for (int i = 0; i < 5; i++)
            {
                foreach (char item in msg){
                    Console.Write(characters[item][i] + "  ");
                }
                Console.WriteLine();
            }
        }
        catch (Exception e){
            LogError(e.ToString());
        }
    }
    private static void Init(){
        //Console.WriteLine("Initializing Dictionary");
        if(alphabet.Length != dotFont.Length){
            Utilities.Logger.LogError("Length mismatch");
            #pragma warning restore format
            return;
        }
        for (int i = 0; i < alphabet.Length; i++){
            characters[alphabet[i]] = dotFont[i];
        }
        //Console.WriteLine("Done");
    }

    #endregion
}

public static class Operations{

    /// <summary>
    /// Converts an array of strings to a single string
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static string StringArrayToString(string[] array, string seperator = ""){
        string output = "";
        foreach(string item in array) output += item + seperator;
        return output;
    }
    public static string ArrayToString(this object[] objects, string seperator = "")
    {
        string output = "";
        foreach (var item in objects){
            output += item.ToString() + seperator;
        }
        return output;
    }

    /// <summary>
    /// Linearly interpolates between a and b based on t
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static double Lerp(double a, double b, double t){
        return a  * (1f - t) + b * t;
    }

    /// <summary>
    /// Adds an array of doubles together
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static double Add(params double[] array){
        double output = default;
        foreach(double item in array) output += item;
        return output;
    }
    /// <summary>
    /// Subtracts an array of doubles
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static double Subtract(params double[] array){
        double output = default;
        foreach(double item in array) output -= item;
        return output;
    }
    /// <summary>
    /// Multiplies an array of doubles together
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static double Multiply(params double[] array){
        double output = default;
        foreach(double item in array) output *= item;
        return output;
    }
    /// <summary>
    /// Divides an array of doubles
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static double Divide(params double[] array){
        double output = default;
        foreach(double item in array) output /= item;
        return output;
    }
}

public static class Runtime{
    /// <summary>
    /// Displays a short animation
    /// </summary>
    public static void Start(){
        Console.WriteLine("Made by");
        Logger.DottedWriteLine("Lukas");
        Logger.DottedWriteLine("Grunzweil");
        Logger.ColorInfo();
    }

    public static void Exit(){
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n\nProgram finished\nPress any key to close");
        Console.ReadKey(true);
    }
}

public static class Random{
    /// <summary>
    /// A pseudo-Random double
    /// </summary>
    public static double Value => System.Random.Shared.NextDouble();

    /// <summary>
    /// A pseudo-Random int32
    /// </summary>
    public static int Int32Value => System.Random.Shared.Next();

    /// <summary>
    /// A pseudo-Random int64
    /// </summary>
    public static long LongValue => System.Random.Shared.NextInt64();

    /// <summary>
    /// A psudo-Random int32 between a min and a max value (both are included)
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Range(int min, int max){
        return System.Random.Shared.Next(min, max);
    }
}

public static class Input{
    public static double GetDoubleInput(string prompt = ""){
        Logger.LogPrompt(prompt);
        double output;
        while(!double.TryParse(Console.ReadLine(), out output)){
            Logger.LogError("\nInvalid Input. Try again (Use comma for decimals)");
        }
        return output;
    }

    public static int GetIntInput(string prompt = ""){
        Logger.LogPrompt(prompt);
        int output;
        while(!int.TryParse(Console.ReadLine(), out output)){
            Logger.LogError("\nInvalid Input. Try again");
        }
        return output;
    }

    public static string GetValidStringInput(string prompt = ""){
        Logger.LogPrompt(prompt);
        string? input = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(input)){
            Logger.LogError("\nInvalid Input. Try again");
            input = Console.ReadLine();
        }
        return input;
    }

    public static void AwaitKey(ConsoleKey key, string prompt = ""){
        Logger.LogPrompt(prompt);
        if(Console.ReadKey(true).Key != key){
            Logger.LogError("\nNot the right key. Try again");
            AwaitKey(key, prompt);
        }
    }
}

namespace Multidimensional{
    public class Vector2{
        public Vector2(double x = 0, double y = 0){
            this.x = x;
            this.y = y;
        }

        public double x = 0;
        public double y = 0;

        public double magnitude{
            get => Math.Sqrt((x*x)+(y*y));
        }

        public static Vector2 zero {
            get => new(0, 0);
        }
        public static Vector2 one{
            get => new(1, 1);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) {
            return new Vector2(a.x + b.x, a.y + b.y);
        }
        public static Vector2 operator -(Vector2 a, Vector2 b) {
            return new Vector2(a.x - b.x, a.y - b.y);
        }
        public static Vector2 operator *(Vector2 a, Vector2 b) {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        public static Vector2 operator /(Vector2 a, Vector2 b) {
            return new Vector2(a.x / b.x, a.y / b.y);
        }
        public static bool operator ==(Vector2 a, Vector2 b) {
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Vector2 a, Vector2 b) {
            return !(a.x == b.x && a.y == b.y);
        }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        } 
    }

    public class Vector3{
        public Vector3(double x = 0, double y = 0, double z = 0){
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double x = 0;
        public double y = 0;
        public double z = 0;

        public double magnitude{
            get => Math.Sqrt((x*x)+(y*y)+(z*z));
        }

        public static Vector3 zero {
            get => new(0, 0, 0);
        }
        public static Vector3 one{
            get => new(1, 1, 1);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b) {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static Vector3 operator *(Vector3 a, Vector3 b) {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }
        public static Vector3 operator /(Vector3 a, Vector3 b) {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }
        public static bool operator ==(Vector3 a, Vector3 b) {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(Vector3 a, Vector3 b) {
            return !(a.x == b.x && a.y == b.y && a.z == b.z);
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        } 
    }
}

/// <summary>
/// DO NOT USE IN STANDARD FORMAT. ONLY USED TO EXTEND OTHER CLASSES
/// </summary>
public static class Extensions{
    
}
}