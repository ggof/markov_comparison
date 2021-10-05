
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Markov
{
  public class AuthorParser
  {
    public static AuthorParser ForAuthor(string name) => new AuthorParser { _dir = name };
    const string BadChars = ",./\\\"'?!:;[]{}}|()-_\n\t";
    private string _dir;
    private Dictionary<string, int> _ngrams = new();

    public Task Parse() => Task.Run(() =>
    {
      var author = _dir.Split('/').Last();
      var files = Directory.EnumerateFiles(_dir);
      foreach (var file in files)
      {
        ParseFile(file);
      }

      var max = new KeyValuePair<string, int>("", 0);
      foreach (var kv in _ngrams)
      {
        if (kv.Value > max.Value)
        {
          max = kv;
        }
      }

      Console.WriteLine($"Author {author}: \"{max.Key}\" with {max.Value} repetitions.");
    });

    void ParseFile(string file)
    {
      var lines = File.ReadLines(file);

      var ngram = new List<string>();

      foreach (var line in lines.Select(RemoveUnwantedChars))
      {
        foreach (var word in line.Split(' '))
        {
          if (IsTooShort(word))
          {
            continue;
          }

          ngram.Add(word);

          if (ngram.Count != 3)
          {
            continue;
          }

          Increment(string.Join(' ', ngram));
          ngram.RemoveAt(0);
        }
      }
    }

    private void Increment(string key)
    {
      var val = _ngrams.GetValueOrDefault(key, 0);
      _ngrams[key] = val + 1;
    }

    private static string RemoveUnwantedChars(string line) => new(line.Select(c => BadChars.Contains(c) ? ' ' : c).ToArray());
    private static bool IsTooShort(string word) => word.Length < 3;
  }
}