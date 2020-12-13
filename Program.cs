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
        static string _listPattern = @"(-|\*|\+) .*\n";
        static string _pattern = @$"{_headingPattern}|{_listPattern}";
        static bool _listMode = false;
        static int _headingLevel = 1;

        static void MarkdownToMindmap(string dir)
        {
            string markdownContent;
            int headingCount = 0;
            int line = 0;
            bool isFliped = false;
            Regex subheadingRegex = new Regex(@"^## ");
            Regex listRegex = new Regex(@"^(-|\*|\+) ");
            Regex hashToAsteriskRegex = new Regex("#");

            using (StreamReader reader = new StreamReader(dir))
            {
                markdownContent = reader.ReadToEnd();
            }

            string pattern = _listMode? _pattern : _headingPattern;
            MatchCollection headings = Regex.Matches(markdownContent, pattern);
            headingCount = headings.Count;

            _puml.Append("@startmindmap\n");
            foreach (Match heading in headings)
            {
                line++;
                string header = heading.ToString();
                if (!isFliped && subheadingRegex.IsMatch(header) && line > headingCount / 2)
                {
                    _puml.Append("left side\n");
                    isFliped = true;
                }

                if(_listMode)
                {
                    CheckHeadingLevel(heading.ToString());
                }

                if(_listMode && listRegex.IsMatch(header))
                    _puml.Append(listRegex.Replace(header, new String('*', _headingLevel+1) + "_ "));
                else
                    _puml.Append(hashToAsteriskRegex.Replace(header, @"*"));
            }
            _puml.Append("@endmindmap");
        }

        static void CheckHeadingLevel(string heading)
        {
            MatchCollection hashs = Regex.Matches(heading, @"#");
            if(hashs.Count != 0) _headingLevel = hashs.Count;
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Please input the absolute path of markdown files:");
            string targetDir = Console.ReadLine();
            Console.WriteLine("Use list mode? (Y|else no)");
            if(Console.ReadLine() == "Y")
                _listMode = true;
            
            if(targetDir.EndsWith(".md") ? true : false)
            {
                FileInfo file = new FileInfo(targetDir);
                Directory.SetCurrentDirectory(file.DirectoryName);
                string output = Regex.Match(file.Name, @".*?(?=\.md)").ToString();

                MarkdownToMindmap(targetDir);

                using (StreamWriter writer = new StreamWriter($"{output}.puml"))
                    writer.Write(_puml.ToString());
            }
            else
            {
                Console.WriteLine("Wrong! Target file is not a markdown file.");
            }
        }
    }
}
