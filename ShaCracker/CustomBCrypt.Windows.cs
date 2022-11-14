using System.Runtime.InteropServices;

namespace ShaCracker;

public static class CustomBCrypt
{
    [DllImport("BCrypt.dll", CharSet = CharSet.Unicode)]
    public static extern unsafe uint BCryptHash(
        nuint hAlgorithm,
        byte* pbSecret,
        int cbSecret,
        byte* pbInput,
        int cbInput,
        byte* pbOutput,
        int cbOutput);
}