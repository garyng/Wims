<Query Kind="Program" />

void Main()
{
	var recursionCount = 0;
	var recursionLimit = 10;
	var matches = new List<int>();
	var maxMatches = 20;
	var pattern = "B+A";
	var str = "A+B";

	FuzzyMatch(pattern, str, 0, 0, new List<int>(), matches, maxMatches, 0, recursionCount, recursionLimit)
		.Dump();

	string.Join("\n", new[] { string.Join("", str.Select((_, i) => $"| {i,2} ")), 
		string.Join("", str.Select(c => $"| {c,2} ")), 
		string.Join("", str.Select((_, i) => matches.Contains(i) ? "|  x " : "|    ")) }).Dump();
}

const int SEQUENTIAL_BONUS = 15; // bonus for adjacent matches
const int SEPARATOR_BONUS = 30; // bonus if match occurs after a separator
const int CAMEL_BONUS = 30; // bonus if match is uppercase and prev is lower
const int FIRST_LETTER_BONUS = 15; // bonus if the first letter is matched

const int LEADING_LETTER_PENALTY = -5; // penalty applied for every letter in str before the first match
const int MAX_LEADING_LETTER_PENALTY = -15; // maximum penalty for leading letters
const int UNMATCHED_LETTER_PENALTY = -1;

string RemoveSeparator(string input)
{
	return input.Replace(",", " ").Replace("+", " ");
}

(bool matched, int score) FuzzyMatch(string pattern, string strToSearch, int patternCurIndex, int strCurrentIndex,
	List<int> srcMatches, List<int> matches, int maxMatches, int nextMatch, int recursionCount, int recursionLimit)
{
	var score = 0;
	$"{new string(' ', recursionCount)}{recursionCount}".Dump();
	recursionCount++;

	if (recursionCount >= recursionLimit) return (false, score);

	if (patternCurIndex == pattern.Length || strCurrentIndex == strToSearch.Length) return (false, score);

	var recursiveMatch = false;
	var bestRecursiveMatches = new List<int>();
	var bestRecursiveScore = 0;

	var firstMatch = true;
	while (patternCurIndex < pattern.Length && strCurrentIndex < strToSearch.Length)
	{
		if (IsSeparator(pattern[patternCurIndex])) patternCurIndex++;
		if (IsSeparator(strToSearch[strCurrentIndex])) strCurrentIndex++;

		var currP = char.ToLower(pattern[patternCurIndex]);
		var currS = char.ToLower(strToSearch[strCurrentIndex]);
		
		if (currP == currS)
		{
			if (nextMatch >= maxMatches) return (false, score);
			if (firstMatch && srcMatches?.Any() == true)
			{
				matches.Clear();
				matches.AddRange(srcMatches);
				firstMatch = false;
			}

			var recursiveMatches = new List<int>();
			var (matched, recursiveScore) = FuzzyMatch(pattern, strToSearch, patternCurIndex, strCurrentIndex + 1, matches, recursiveMatches, maxMatches,
															nextMatch, recursionCount, recursionLimit);

			if (matched)
			{
				if (!recursiveMatch || recursiveScore > bestRecursiveScore)
				{
					bestRecursiveMatches = new List<int>(recursiveMatches);
					bestRecursiveScore = recursiveScore;
				}
				recursiveMatch = true;
			}

			nextMatch++;
			matches.Add(strCurrentIndex);
			patternCurIndex++;
		}

		strCurrentIndex++;
	}

	var fullMatched = patternCurIndex == pattern.Length;

	if (fullMatched)
	{
		score = 100;

		// apply leading letter penalty
		var penalty = LEADING_LETTER_PENALTY * matches[0];
		penalty = penalty < MAX_LEADING_LETTER_PENALTY ? penalty : MAX_LEADING_LETTER_PENALTY;
		score += penalty;

		// apply unmatched penalty
		var unmatched = strToSearch.Length - nextMatch;
		score += UNMATCHED_LETTER_PENALTY * unmatched;

		// apply ordering bonuses
		for (int i = 0; i < nextMatch; i++)
		{
			var currentIdx = matches[i];

			if (i > 0)
			{
				var prevIdx = matches[i - 1];
				if (currentIdx == prevIdx + 1)
				{
					score += SEQUENTIAL_BONUS;
				}
			}

			// check for bonuses based on neighbor character value
			if (currentIdx > 0)
			{
				// camel case
				var neighbor = strToSearch[currentIdx - 1];
				var curr = strToSearch[currentIdx];
				if (char.IsLower(neighbor) && char.IsUpper(curr))
				{
					score += CAMEL_BONUS;
				}

				// separator
				//bool neighborSeparator = neighbor == ' ';
				//if (neighborSeparator)
				//{
				//	score += SEPARATOR_BONUS;
				//}

			}
			else
			{
				// first letter
				score += FIRST_LETTER_BONUS;
			}
		}

		if (recursiveMatch && (!fullMatched || bestRecursiveScore > score))
		{
			// recusive score is better than "this"
			matches.Clear();
			matches.AddRange(bestRecursiveMatches);
			score = bestRecursiveScore;

			return (true, score);
		}
		else if (fullMatched)
		{
			// "this" score is better than recursive
			return (true, score);
		}
		else
		{
			return (false, score);
		}
	}
	return (false, score);
}

bool IsSeparator(char c )
{
	return c == ',' || c == '+' || c == ' ';
}