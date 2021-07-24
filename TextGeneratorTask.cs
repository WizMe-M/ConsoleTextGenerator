using System.Collections.Generic;
using System.Text;

namespace TextAnalysis
{
    public static class TextGeneratorTask
    {
        public static string ContinuePhrase(Dictionary<string, string> nextWords,
            string phraseBeginning, int wordsCount)
        {
            var phrase = new List<string>(phraseBeginning.Split(' '));

            for (var i = 0; i < wordsCount; i++)
            {
                var addedWord = GetWordToAddToPhrase(nextWords, phrase);

                if (string.IsNullOrEmpty(addedWord)) continue;

                phrase.Add(addedWord);
            }

            return ConvertToString(phrase);
        }

        private static string GetWordToAddToPhrase(Dictionary<string, string> nextWords, List<string> phrase)
        {
            var length = phrase.Count;
            string addedWord = null;

            if (length >= 2)
            {
                var doubleKey = string.Join(" ", phrase[length - 2], phrase[length - 1]);
                var key = phrase[length - 1];
                addedWord = FindTwoWordContinue(nextWords, doubleKey, key);
            }
            else if (length == 1)
            {
                var key = phrase[length - 1];
                addedWord = FindOneWordContinue(nextWords, key);
            }

            return addedWord;
        }

        private static string FindTwoWordContinue(Dictionary<string, string> nextWords, string doubleKey, string key)
        {
            var addedWord = nextWords.ContainsKey(doubleKey)
                ? nextWords[doubleKey]
                : FindOneWordContinue(nextWords, key);

            return addedWord;
        }

        private static string FindOneWordContinue(Dictionary<string, string> nextWords, string key)
        {
            string phraseContinuation = null;

            if (nextWords.ContainsKey(key))
            {
                phraseContinuation = nextWords[key];
            }

            return phraseContinuation;
        }

        private static string ConvertToString(List<string> words)
        {
            var sentence = new StringBuilder();
            for (var i = 0; i < words.Count; i++)
            {
                sentence.Append(words[i]);

                if (i == words.Count - 1) continue;
                sentence.Append(" ");
            }

            return sentence.ToString();
        }
    }
}