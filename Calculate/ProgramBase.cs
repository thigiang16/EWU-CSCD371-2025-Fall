using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate;

public class ProgramBase
{
    public Action<string> WriteLine { get; init; } = Console.WriteLine!;
    public Func<string?> ReadLine { get; init; } = Console.ReadLine!;
    public ProgramBase() { }
}
