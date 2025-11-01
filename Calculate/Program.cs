namespace Calculate;
public class Program
{
    public Action<string> WriteLine { get; init; } = Console.WriteLine!;
    public Func<string> ReadLine { get; init; } = Console.ReadLine!;

    public Program() { }

    public static void Main()
    {
        Program program = new();
        program.WriteLine("Type something and press Enter:");
        string? input = program.ReadLine();
        program.WriteLine("You typed: " + input);
    }
}
