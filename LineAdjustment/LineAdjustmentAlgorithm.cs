using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineAdjustment
{
    public class LineAdjustmentAlgorithm
    {
        public string Transform(string input, int lineWidth)
        {
            if (lineWidth <= 0)
                throw new ArgumentException("Line width should be positive number.", nameof(lineWidth));
            
            if (string.IsNullOrWhiteSpace(input))
                return "";
            
            var words = input.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            
            if (words.Length == 0)
                return "";

            // Length of each word shouldn't be longer than line width.

            if (words.Any(word => word.Length > lineWidth))
            {
                words = NormalizeWordList(words, lineWidth);
            }
            
            var sb = new StringBuilder(input.Length * 2); // Enough capacity for only 1 memory allocation
           
            var lineLength = 0;
            var lineWords = new List<string>(10);
            
            foreach (var word in words)
            {
                // If we collected enough words then we will compose a line.
                if (lineLength + word.Length + lineWords.Count > lineWidth)
                {
                    if (sb.Length > 0)
                        sb.AppendLine();
                    
                    ComposeLineAndAppend(lineWords, lineWidth, sb);
                    
                    lineLength = 0;
                    lineWords.Clear();
                }
                
                lineWords.Add(word);
                lineLength += word.Length;
            }

            // Process the last line if any words left.
            
            if (lineWords.Count > 0)
            {
                if (sb.Length > 0)
                    sb.AppendLine();
                
                ComposeLineAndAppend(lineWords, lineWidth, sb);
            }

            return sb.ToString();
        }

        private string[] NormalizeWordList(string[] words, int lineWidth)
        {
            var normalized = new List<string>(words.Length * 2);
            
            foreach (var word in words)
            {
                if (word.Length > lineWidth)
                {
                    // Word can be longer than 2+ lineWidth.
                    var parts = word.Length / lineWidth;

                    var i = 0;
                    for (; i < parts; i++)
                    {
                        normalized.Add(word.Substring(i * lineWidth, lineWidth));
                    }

                    normalized.Add(word.Substring(i * lineWidth));
                }
                else
                {
                    normalized.Add(word);
                }
            }

            return normalized.ToArray();
        }

        private void ComposeLineAndAppend(List<string> words, int lineWidth, StringBuilder sb)
        {
            // Расстояние между словами нужно заполнять равным количеством пробелов, если же это не возможно, то добавляем
            // еще по пробелу между словами слева направо. Если в строке помещается только 1 слово, то дополнить строку 
            // пробелами справа.

            if (words.Count == 1)
            {
                sb.Append(words[0]);
                sb.Append(' ', lineWidth - words[0].Length);
                return;
            }
            
            var total = lineWidth - words.Sum(w => w.Length);
            var equal = total / (words.Count - 1); // Spaces between words we always add
            var extra = total % (words.Count - 1); // Extra spaces that we'll use to add from left to right 

            for (var i = 0; i < words.Count - 1; i++)
            {
                sb.Append(words[i]);
                sb.Append(' ', equal);

                if (extra != 0)
                {
                    sb.Append(' ');
                    extra--;
                }
            }
            
            // Last word we add without spaces after.
            sb.Append(words[^1]);
        }
    }
}