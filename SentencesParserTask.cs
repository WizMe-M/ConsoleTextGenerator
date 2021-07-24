using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextAnalysis
{
    public static class SentencesParserTask
    {
        public static List<List<string>> ParseSentences(string text)
        {
            var sentences = text.Split(new char[] {'.', '!', '?', ';', ':', '(', ')'},
                StringSplitOptions.RemoveEmptyEntries);
            var sentencesList = sentences.Select(GetWordsFromSentence).ToList();
            sentencesList.RemoveAll(words => words.Count == 0);
            return sentencesList;
        }

        private static List<string> GetWordsFromSentence(string sentence)
        {
            var words = new List<string>();
            var word = new StringBuilder();
            foreach (var letter in sentence)
            {
                if (char.IsLetter(letter) || letter == '\'')
                {
                    var s = letter.ToString();
                    word.Append(s.ToLower());
                }
                else
                {
                    words.Add(word.ToString());
                    word.Clear();
                }
            }
            words.Add(word.ToString());
            words.RemoveAll(s => s == string.Empty);
            return words;
        }
    }
}