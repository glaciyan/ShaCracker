using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ShaCracker;

public static class Program
{
    private const int HashSizeBits = 160;
    private const int HashSizeBytes = HashSizeBits / 8;

    public static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Needs a hash -> ./ShaCracker.exe hash");
            return 1;
        }

        var shouldBe = args[0];
        var shouldBeBytes = (from x in Enumerable.Range(0, shouldBe.Length)
            where x % 2 == 0
            select Convert.ToByte(shouldBe.Substring(x, 2), 16)).ToArray();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        Parallel.ForEach(PasswordGenerator.Produce2Chars(), (c, state) =>
        {
            var passwordBuffer = GC.AllocateUninitializedArray<byte>(6);
            var hashBuffer = (Span<byte>) GC.AllocateUninitializedArray<byte>(HashSizeBytes);

            foreach (var dummy in PasswordGenerator.ProducePasswords(c.first, c.second, passwordBuffer))
            {
                SHA1.HashData(passwordBuffer, hashBuffer);
                if (!passwordBuffer.SequenceEqual(shouldBeBytes)) continue;

                Console.WriteLine($"Found password {Encoding.ASCII.GetString(passwordBuffer)}");
                Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");
                state.Stop();
                break;
            }
        });

        return 0;
    }
}