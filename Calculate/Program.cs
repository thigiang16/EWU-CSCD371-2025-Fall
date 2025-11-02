namespace Calculate;
public class Program:ProgramBase
{
    public static void Main()
    {
        Program program = new();
        Calculator calculator = new();

        program.WriteLine("Enter calculation (examples: 3 + 4 or 2 - 5)");
        program.WriteLine("Make sure there is a space between each input:");

        string? input = program.ReadLine();

        if (input is null)
        {
            program.WriteLine("There wasn't any input");
            return;
        }

        if (calculator.TryCalculate(input, out double result))
        {
            program.WriteLine($"The result is: {result}");
        }
        else
        {
            program.WriteLine("Something went wrong with the calculation");
        }
    }
}
