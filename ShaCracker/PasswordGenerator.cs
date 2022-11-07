namespace ShaCracker;

public static class PasswordGenerator
{
    public static char[] Chars =
    {
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z',
        'a',
        'b',
        'c',
        'd',
        'e',
        'f',
        'g',
        'h',
        'i',
        'j',
        'k',
        'l',
        'm',
        'n',
        'o',
        'p',
        'q',
        'r',
        's',
        't',
        'u',
        'v',
        'w',
        'x',
        'y',
        'z',
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
    };

    public static IEnumerable<(char first, char second)> Produce2Chars(int startFirst = 0, int startSecond = 0)
    {
        for (var i = startFirst; i < Chars.Length; i++)
        {
            var first = Chars[i];
            for (var i1 = startSecond; i1 < Chars.Length; i1++)
            {
                var second = Chars[i1];
                yield return (first, second);
            }
        }
    }

    public static IEnumerable<int> ProducePasswords(char first, char second, byte[] buffer)
    {
        if (buffer.Length != 6) throw new ArgumentException("buffer must have length of 6");

        buffer[0] = (byte)first;
        buffer[1] = (byte)second;

        foreach (var third in Chars)
        {
            foreach (var fourth in Chars)
            {
                foreach (var fifth in Chars)
                {
                    foreach (var sixth in Chars)
                    {
                        buffer[2] = (byte)third;
                        buffer[3] = (byte)fourth;
                        buffer[4] = (byte)fifth;
                        buffer[5] = (byte)sixth;
                        yield return 1;
                    }
                }
            }
        }
    }
}