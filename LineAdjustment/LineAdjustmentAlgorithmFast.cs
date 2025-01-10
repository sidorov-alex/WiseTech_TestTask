using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LineAdjustment
{
    public class LineAdjustmentAlgorithmFast
    {
        public string Transform(string input, int lineWidth)
        {
            if (lineWidth <= 0)
                throw new ArgumentException("Line width should be positive number.", nameof(lineWidth));
            
            if (string.IsNullOrWhiteSpace(input))
                return "";
            
            // Разбиваем исходный текст на слова.

            var inputSpan = input.AsSpan();
            var words = SplitIntoWords(inputSpan);

            // Если хотя бы одно слово длиннее чем результирующая строка, то проводим нормализацию.

            if (words.Any(word => word.GetLength(input.Length) > lineWidth))
            {
                words = NormalizeWordList(words, lineWidth, inputSpan);
            }
            
            var result = new StringBuilder(input.Length * 2); // Достаточная емкость для ровно одного выделения памяти

            // Накапливаем слова, которые вмещаются в строку, после этого создаем из них строку и добавляем
            // в результат. Так, пока слова не закончатся.
            
            var currentLineLength = 0;
            var currentLineWords = new List<Range>(10);
            
            foreach (var word in words)
            {
                if (currentLineLength + word.GetLength(inputSpan.Length) + currentLineWords.Count > lineWidth)
                {
                    ComposeLineAndAppend(currentLineWords, lineWidth, inputSpan, result);
                    
                    currentLineLength = 0;
                    currentLineWords.Clear();
                }
                
                currentLineWords.Add(word);
                currentLineLength += word.GetLength(inputSpan.Length);
            }

            // Добавляем последнюю строку, если слова остались в буфере.
            
            if (currentLineWords.Count > 0)
            {   
                ComposeLineAndAppend(currentLineWords, lineWidth, inputSpan, result);
            }

            return result.ToString();
        }
        
        /// <summary>
        /// Создает строку из указанных слов и добавляет её в указанный <see cref="StringBuilder"/>. 
        /// </summary>
        /// <param name="words">Список слов.</param>
        /// <param name="lineWidth">Длина строки.</param>
        /// <param name="inputSpan">Входной массив символов, из которого берутся слова.</param>
        /// <param name="result">Результат, в который будет добавлена строка.</param>
        private void ComposeLineAndAppend(List<Range> words, int lineWidth, ReadOnlySpan<char> inputSpan, StringBuilder result)
        {
            // Расстояние между словами нужно заполнять равным количеством пробелов, если же это не возможно, то добавляем
            // еще по пробелу между словами слева направо. Если в строке помещается только 1 слово, то дополнить строку 
            // пробелами справа.
            
            if (result.Length > 0)
                result.AppendLine();
            
            var inputLength = inputSpan.Length; // inputSpan нельзя использовать в Sum()

            if (words.Count == 1)
            {
                result.Append(inputSpan[words[0]]);
                result.Append(' ', lineWidth - words[0].GetLength(inputLength));
                return;
            }
            
            var total = lineWidth - words.Sum(w => w.GetLength(inputLength));
            var equal = total / (words.Count - 1); // Обязательные пробелы между словами
            var extra = total % (words.Count - 1); // Дополнительные пробелы, добавляемые слева направо 

            for (var i = 0; i < words.Count - 1; i++)
            {
                result.Append(inputSpan[words[i]]);
                result.Append(' ', equal);

                if (extra != 0)
                {
                    result.Append(' ');
                    extra--;
                }
            }
            
            // Последнее слово добавляем без пробелов после него.
            
            result.Append(inputSpan[words[^1]]);
        }
        
        /// <summary>
        /// Нормализует список слов так, чтобы они помещались в строку.
        /// </summary>
        /// <param name="words">Список слов.</param>
        /// <param name="lineWidth">Длина строки.</param>
        /// <param name="inputSpan">Входной массив символов, из которого берутся слова.</param>
        /// <returns>Возвращает список, в котором все слова не длиннее строки.</returns>
        private List<Range> NormalizeWordList(List<Range> words, int lineWidth, ReadOnlySpan<char> inputSpan)
        {
            var normalized = new List<Range>(words.Count * 2);
            
            foreach (var word in words)
            {
                var wordLength = word.GetLength(inputSpan.Length);
                
                if (wordLength > lineWidth)
                {
                    // Разбиваем слишком длинное слово на кусочки равные длине строки + остаток.
                    
                    var fullChunks= wordLength / lineWidth;
                    var remainder = wordLength % lineWidth;

                    for (var i = 0; i < fullChunks ; i++)
                    {
                        var start = word.Start.Value + i * lineWidth;
                        normalized.Add(new Range(start, start + lineWidth));
                    }
                    
                    if (remainder > 0)
                    {
                        var start = word.Start.Value + fullChunks * lineWidth;
                        normalized.Add(new Range(start, word.End.Value));
                    }
                }
                else
                {
                    normalized.Add(word);
                }
            }

            return normalized;
        }
        
        /// <summary>
        /// Разбивает текст на слова.
        /// </summary>
        /// <param name="input">Исходный текст в виде <see cref="ReadOnlySpan{T}"/>.</param>
        /// <returns>Возвращает список указателей на слова в виде <see cref="Range"/>.</returns>
        private List<Range> SplitIntoWords(ReadOnlySpan<char> input)
        {
            var result = new List<Range>(input.Length / 6); // Средняя длина слова

            // Разбиваем за несколько итераций, потому что так работает Split() у ReadOnlySpan.
            
            Span<Range> tempRanges = stackalloc Range[32];

            while (true)
            {
                var count = input.Split(tempRanges, ' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (count == 0)
                    break;

                for (var i = 0; i < count; i++)
                {
                    result.Add(tempRanges[i]);
                }

                // Если еще остались слова, то переходим к следующей части исходного текста.

                if (count < tempRanges.Length)
                    break;

                input = input[tempRanges[^1].End..];
            }

            return result;
        }
    }
}