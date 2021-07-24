using System.Collections.Generic;
using System.Linq;

namespace TextAnalysis
{
    public static class FrequencyAnalysisTask
    {
        public static Dictionary<string, string> GetMostFrequentNextWords(List<List<string>> text)
        {
            var nonFilteredNgrams = new Dictionary<string, Dictionary<string, int>>();

            foreach (var sentence in text)
            {
                var bigrams = GetBigramsFromSentence(sentence);
                var trigrams = GetTrigramsFromSentence(sentence);
                var ngrams = CombineFrequencyDictionaries(bigrams, trigrams);
                nonFilteredNgrams = CombineFrequencyDictionaries(nonFilteredNgrams, ngrams);
            }

            return FilterNgramDictionary(nonFilteredNgrams);
        }

        private static Dictionary<string, Dictionary<string, int>> GetBigramsFromSentence(List<string> sentence)
        {
            var bigrams = new Dictionary<string, Dictionary<string, int>>();

            for (var i = 1; i < sentence.Count; i++)
            {
                var bigramStart = sentence[i - 1];
                var bigramEnd = sentence[i];

                if (!bigrams.ContainsKey(bigramStart))
                {
                    bigrams.Add(bigramStart, new Dictionary<string, int>());
                }

                if (!bigrams[bigramStart].ContainsKey(bigramEnd))
                {
                    bigrams[bigramStart].Add(bigramEnd, 0);
                }

                bigrams[bigramStart][bigramEnd]++;
            }

            return bigrams;
        }

        private static Dictionary<string, Dictionary<string, int>> GetTrigramsFromSentence(List<string> sentence)
        {
            var trigrams = new Dictionary<string, Dictionary<string, int>>();

            for (var i = 2; i < sentence.Count; i++)
            {
                var trigramStart = string.Join(" ", sentence[i - 2], sentence[i - 1]);
                var trigramEnd = sentence[i];

                if (!trigrams.ContainsKey(trigramStart))
                {
                    trigrams.Add(trigramStart, new Dictionary<string, int>());
                }

                if (!trigrams[trigramStart].ContainsKey(trigramEnd))
                {
                    trigrams[trigramStart].Add(trigramEnd, 0);
                }

                trigrams[trigramStart][trigramEnd]++;
            }

            return trigrams;
        }

        private static Dictionary<string, string> FilterNgramDictionary(
            Dictionary<string, Dictionary<string, int>> bigramDictionary)
        {
            var frequencyFilteredDictionary = GetFrequencyFilteredDictionary(bigramDictionary);
            var filteredBigramDictionary = GetOrdinalFilteredDictionary(frequencyFilteredDictionary);
            return filteredBigramDictionary;
        }

        private static Dictionary<string, List<string>> GetFrequencyFilteredDictionary(
            Dictionary<string, Dictionary<string, int>> ngramDictionary)
        {
            //словарь самых частых N-грамм
            var filteredNgrams = new Dictionary<string, List<string>>();

            foreach (var ngrams in ngramDictionary)
            {
                var ngramsStart = ngrams.Key;

                filteredNgrams.Add(ngramsStart, new List<string>());

                var maxFrequency = ngrams.Value.Select(ngramEnd => ngramEnd.Value).Max();

                foreach (var ngramEnd in ngrams.Value)
                {
                    if (ngramEnd.Value == maxFrequency)
                    {
                        filteredNgrams[ngramsStart].Add(ngramEnd.Key);
                    }
                }
            }

            return filteredNgrams;
        }

        private static Dictionary<string, string> GetOrdinalFilteredDictionary(
            Dictionary<string, List<string>> frequencyFilteredDictionary)
        {
            //из списка самых частых N-грамм составляем словарь лексикографически меньших N-грамм
            var ordinalFilteredDictionary = new Dictionary<string, string>();

            foreach (var ngrams in frequencyFilteredDictionary)
            {
                var ngramStart = ngrams.Key;
                var ngramEnd = FindMinWord(ngrams.Value);
                ordinalFilteredDictionary.Add(ngramStart, ngramEnd);
            }

            return ordinalFilteredDictionary;
        }

        private static string FindMinWord(List<string> words)
        {
            string minWord = null;

            foreach (var bigramEnd in words)
            {
                if (string.IsNullOrEmpty(minWord))
                {
                    minWord = bigramEnd;
                }

                var compareResult = string.CompareOrdinal(minWord, bigramEnd);
                if (compareResult > 0)
                {
                    minWord = bigramEnd;
                }
            }

            return minWord;
        }

        private static Dictionary<string, Dictionary<string, int>> CombineFrequencyDictionaries(
            Dictionary<string, Dictionary<string, int>> basicDictionary,
            Dictionary<string, Dictionary<string, int>> addedDictionary)
        {
            foreach (var pair in addedDictionary)
            {
                var ngramStart = pair.Key;
                if (!basicDictionary.ContainsKey(ngramStart))
                {
                    basicDictionary.Add(ngramStart, new Dictionary<string, int>());
                }

                foreach (var frequencyPair in addedDictionary[ngramStart])
                {
                    var ngramEnd = frequencyPair.Key;
                    if (!basicDictionary[ngramStart].ContainsKey(ngramEnd))
                    {
                        basicDictionary[ngramStart].Add(ngramEnd, 0);
                    }

                    basicDictionary[ngramStart][ngramEnd] += addedDictionary[ngramStart][ngramEnd];
                }
            }

            return basicDictionary;
        }
    }
}