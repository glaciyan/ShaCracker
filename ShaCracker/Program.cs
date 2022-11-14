using System.Diagnostics;
using System.Runtime.InteropServices;
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
        Parallel.ForEach(PasswordGenerator.Chars, (c) =>
        {
            Console.WriteLine($"Starting Thread {Environment.CurrentManagedThreadId} trying {c}????");
            // allocate memory
            var passwordBuffer = GC.AllocateUninitializedArray<byte>(5);
            var hashBuffer = GC.AllocateUninitializedArray<byte>(HashSizeBytes);

            passwordBuffer[0] = (byte) c;

            // generate 4 more characters to test all 6 character sequences
            // and check if the SHA1 hash matches with the given hash from above
            foreach (var third in PasswordGenerator.Chars)
            {
                foreach (var fourth in PasswordGenerator.Chars)
                {
                    foreach (var fifth in PasswordGenerator.Chars)
                    {
                        foreach (var sixth in PasswordGenerator.Chars)
                        {
                            passwordBuffer[1] = (byte) third;
                            passwordBuffer[2] = (byte) fourth;
                            passwordBuffer[3] = (byte) fifth;
                            passwordBuffer[4] = (byte) sixth;

                            unsafe
                            {
                                // SHA1.HashData(passwordBuffer, hashBuffer);
                                fixed (byte* pSource = &MemoryMarshal.GetReference((ReadOnlySpan<byte>) passwordBuffer))
                                fixed (byte* pDest = &MemoryMarshal.GetReference((ReadOnlySpan<byte>) hashBuffer))
                                {
                                    var status = CustomBCrypt.BCryptHash(49U, null, 0, pSource, passwordBuffer.Length,
                                        pDest, 160 / 8);
                                    if (status != 0U)
                                    {
                                        throw new CryptographicException();
                                    }
                                }
                            }

                            var equal = true;
                            for (var i = 0; i < HashSizeBytes; i++)
                            {
                                if (hashBuffer[i] != shouldBeBytes[i])
                                {
                                    equal = false;
                                    break;
                                }
                            }

                            if (!equal) continue;

                            // print out the password and kill the parallel operation
                            Console.WriteLine($"Found password {Encoding.ASCII.GetString(passwordBuffer)}");
                            Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");
                            // state.Stop();
                            Environment.Exit(0);
                            break;
                        }
                    }
                }
            }
        });

        Console.WriteLine("Nothing found, idiot.");
        Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}");

        return 0;
    }
}