using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MarkdownToMindmap
{
    internal class Program
    {
        static StringBuilder _puml = new StringBuilder();
        static string _headingPattern = @"#+ .*\n";

        static void MarkdownToMindmap(string dir)
        {
            string markdownContent;
            int headingCount = 0;
            int line = 0;
            bool isFliped = false;

            using (StreamReader reader = new StreamReader(dir))
            {
                markdownContent = reader.ReadToEnd();
            }

            MatchCollection headings = Regex.Matches(markdownContent, _headingPattern);
            headingCount = headings.Count;

            _puml.Append("@startmindmap\n");
            foreach (Match heading in headings)
            {
                line++;
                Regex subheadingRegex = new Regex(@"^## ");
                if (!isFliped && subheadingRegex.IsMatch(heading.ToString()) && line > headingCount / 2)
                {
                    _puml.Append("left side\n");
                    isFliped = true;
                }
                Regex hashToAsteriskRegex = new Regex("#");
                _puml.Append(hashToAsteriskRegex.Replace(heading.ToString(), @"*"));
            }
            _puml.Append("@endmindmap");
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Please input the absolute path of markdown files:");
            string targetDir = Console.ReadLine();
            
            if(targetDir.EndsWith(".md") ? true : false)
            {
                FileInfo file = new FileInfo(targetDir);
                Directory.SetCurrentDirectory(file.DirectoryName);

                MarkdownToMindmap(targetDir);

                Console.WriteLine("Please input the output name:");
                string output = Console.ReadLine();

                using (StreamWriter writer = new StreamWriter($"{output}.puml"))
                {
                    writer.Write(_puml.ToString());
                }
            }
            else
            {
                Console.WriteLine("Wrong! Target file is not a markdown file.");
            }
        }
    }
}
