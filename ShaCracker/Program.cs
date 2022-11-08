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

        // the first arg should be the hash
        var shouldBe = args[0];
        // convert the given hexadecimal hash into a byte array
        var shouldBeBytes = (from x in Enumerable.Range(0, shouldBe.Length)
            where x % 2 == 0
            select Convert.ToByte(shouldBe.Substring(x, 2), 16)).ToArray();

        // start time
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        // start a thread for each 2 character combination
        Parallel.ForEach(PasswordGenerator.Produce2Chars(), (c, state) =>
        {
            Console.WriteLine($"Starting Thread {Environment.CurrentManagedThreadId} trying {c.first}{c.second}????");
            // allocate memory
            var passwordBuffer = GC.AllocateUninitializedArray<byte>(6);
            var hashBuffer = GC.AllocateUninitializedArray<byte>(HashSizeBytes);

            // generate 4 more characters to test all 6 character sequences
            // and check if the SHA1 hash matches with the given hash from above
            foreach (var dummy in PasswordGenerator.ProducePasswords(c.first, c.second, passwordBuffer))
            {
                SHA1.HashData(passwordBuffer, hashBuffer);
                if (!hashBuffer.SequenceEqual(shouldBeBytes)) continue;

                // print out the password and kill the parallel operation
                Console.WriteLine($"Found password {Encoding.ASCII.GetString(passwordBuffer)}");
                Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");
                state.Stop();
                break;
            }
        });

        return 0;
    }
}