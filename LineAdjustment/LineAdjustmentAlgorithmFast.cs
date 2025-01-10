using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineAdjustment
{
    public static class RangeExtensions
    {
        public static int GetLength(this Range range, int collectionLength)
        {
            var (_, length) = range.GetOffsetAndLength(collectionLength);
            return length;
        }
    }

    public class LineAdjustmentAlgorithmFast
    {
        public string Transform(string input, int lineWidth)
        {
            if (lineWidth <= 0)
                throw new ArgumentException("Line width should be positive number.", nameof(lineWidth));
            
            if (string.IsNullOrWhiteSpace(input))
                return "";

            var inputSpan = input.AsSpan();
            
            Span<Range> words = stackalloc Range[100];
            var wordsCount = inputSpan.Split(words, ' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            
            if (wordsCount == 0)
                return "";

            // Length of each word shouldn't be longer than line width.

            if (true)//words.Any(word => word.Length > lineWidth))
            {
                words = NormalizeWordList(words, wordsCount, lineWidth, inputSpan);
                wordsCount = words.Length;
            }
            
            var sb = new StringBuilder(input.Length * 2); // Enough capacity for only 1 memory allocation
           
            var lineLength = 0;
            var lineWords = new List<Range>(10);
            
            //foreach (var word in words)
            for (var i = 0; i < wordsCount; i++)
            {
                var word = words[i];
                // If we collected enough words then we will compose a line.
                if (lineLength + word.GetLength(inputSpan.Length) + lineWords.Count > lineWidth)
                {
                    if (sb.Length > 0)
                        sb.AppendLine();
                    
                    ComposeLineAndAppend(lineWords, lineWidth, sb, inputSpan);
                    
                    lineLength = 0;
                    lineWords.Clear();
                }
                
                lineWords.Add(word);
                lineLength += word.GetLength(inputSpan.Length);
            }

            // Process the last line if any words left.
            
            if (lineWords.Count > 0)
            {
                if (sb.Length > 0)
                    sb.AppendLine();
                
                ComposeLineAndAppend(lineWords, lineWidth, sb, inputSpan);
            }

            return sb.ToString();
        }

        private Span<Range> NormalizeWordList(Span<Range> words, int wordsCount, int lineWidth, ReadOnlySpan<char> inputSpan)
        {
            var normalized = new List<Range>(words.Length * 2);
            
            //foreach (var word in words)
            for (var w = 0; w < wordsCount; w++)
            {
                var word = words[w];
                
                if (word.GetLength(inputSpan.Length) > lineWidth)
                {
                    // Word can be longer than 2+ lineWidth.
                    var parts = word.GetLength(inputSpan.Length) / lineWidth;

                    var i = 0;
                    for (; i < parts; i++)
                    {
                        normalized.Add(new Range(word.Start.Value + i * lineWidth,
                            word.Start.Value + i * lineWidth + lineWidth));
                        //normalized.Add(word.Substring(i * lineWidth, lineWidth));
                    }
                    
                    normalized.Add(new Range(word.Start.Value + i * lineWidth, word.End.Value));
                    //normalized.Add(Range.StartAt(word.Start.Value + i * lineWidth));
                    //normalized.Add(word.Substring(i * lineWidth));
                }
                else
                {
                    normalized.Add(word);
                }
            }

            return normalized.ToArray();
        }

        private void ComposeLineAndAppend(List<Range> words, int lineWidth, StringBuilder sb, ReadOnlySpan<char> inputSpan)
        {
            int inputLength = inputSpan.Length;
            
            // Расстояние между словами нужно заполнять равным количеством пробелов, если же это не возможно, то добавляем
            // еще по пробелу между словами слева направо. Если в строке помещается только 1 слово, то дополнить строку 
            // пробелами справа.

            if (words.Count == 1)
            {
                sb.Append(inputSpan[words[0]]);
                sb.Append(' ', lineWidth - words[0].GetLength(inputLength));
                return;
            }
            
            var total = lineWidth - words.Sum(w => w.GetLength(inputLength));
            var equal = total / (words.Count - 1); // Spaces between words we always add
            var extra = total % (words.Count - 1); // Extra spaces that we'll use to add from left to right 

            for (var i = 0; i < words.Count - 1; i++)
            {
                sb.Append(inputSpan[words[i]]);
                sb.Append(' ', equal);

                if (extra != 0)
                {
                    sb.Append(' ');
                    extra--;
                }
            }
            
            // Last word we add without spaces after.
            sb.Append(inputSpan[words[^1]]);
        }
    }
}