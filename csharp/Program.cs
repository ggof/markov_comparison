using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Markov
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

      var texts = "../Texts/";
      var dirs = Directory.EnumerateDirectories(texts);
      var tasks = new Task[dirs.Count()];

      for (var i = 0; i < dirs.Count(); i++)
      {
        tasks[i] = AuthorParser.ForAuthor(texts + dirs.ElementAt(i)).Parse();
      }

      Task.WaitAll(tasks);

      var stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();

      Console.WriteLine($"Done, took {stop - start}ms");
    }
  }
}