Console.WriteLine(Solution.MaxVowels("abciiidef", 3));
public class Solution
{
    public static int MaxVowels(string s, int k)
    {
        HashSet<char> vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };

        int count = 0;
        for (int i = 0; i < k; i++)
        {
            if (vowels.Contains(s[i])) count++;
        }
        int maxCount = count;


        for (int i = k; i < s.Length; i++)
        {
            if (vowels.Contains(s[i])) count++;
            if (vowels.Contains(s[i - k])) count--;
            maxCount = Math.Max(maxCount, count);
        }

        return maxCount;
    }
}
