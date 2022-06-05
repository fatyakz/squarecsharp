using System.Diagnostics;

// settings
int width = 3;
int height = 3;
int start = 1;
int limit = 11;

// init
int x;
int y;
int prx;
int pry;
int loc = 0;
int wins = 0;
int tosolve;
int range = limit + start;
long iterations = 0;
string buff;
int count = (width * height);
// uint total = ((uint)Math.Pow(limit, count));
int[] square = new int[width * height];
int[] sums = new int[width + height + 2];

// zero out arrays of sums and squares
for (int i = 0; i < (width * height); i++) { square[i] = start; }
for (int i = 0; i < (width + height + 2); i++) { sums[i] = 0; }

// calculate how many solves to find, thanks Zaslavsky :)
var t = range;
tosolve = (range % 12) switch
{
	0 or 2 or 6 or 8 => ((t * t * t) - 16 * t * t + 76 * t - 96) / 6,
	1 => ((t * t * t) - 16 * t * t + 73 * t - 58) / 6,
	3 or 11 => ((t * t * t) - 16 * t * t + 73 * t - 102) / 6,
	4 or 10 => ((t * t * t) - 16 * t * t + 76 * t - 112) / 6,
	5 or 9 => ((t * t * t) - 16 * t * t + 73 * t - 90) / 6,
	7 => ((t * t * t) - 16 * t * t + 73 * t - 70) / 6,
	_ => 0,
};

// start timer
var s1 = Stopwatch.StartNew();

// main loop, stops when final cell == limit, replaces while(iterations < total)
while (square[count -1] != limit) { 
	square[0]++;
	iterations++;
	// check if square has equal columns before continuing
	if ((square[0] + square[1] + square[2] == square[3] + square[4] + square[5]) && (square[0] + square[1] + square[2] == square[6] + square[7] + square[8]))
	{
		// check if square has unique values using function FindUnique(array)
		var unique = FindUnique(square);
		if (unique.Count() == count)
		{
			// columns
			x = 0;
			y = 0;
			while (x < width)
			{
				while (y < count)
				{
					sums[x] += square[y];
					y += width;
				}
				x++;
				y = x;
			}
			// attempt at breaking before full sums[] is complete [successful]
			if (sums[0] == sums[1])
			{
				// rows
				x = 0;
				y = 0;
				while (y < height)
				{
					while (x < width)
					{
						sums[y + width] += square[x + (width * y)];
						x++;
					}
					y++;
					x = 0;
				}

				// diagonal right to left
				y = 0;
				while (y < height)
				{
					sums[width + height] += square[(width * (y + 1)) - (y + 1)];
					y++;
				}

				// diagonal left to right
				y = 0;
				while (y < height)
				{
					sums[width + height + 1] += square[(width * y) + y];
					y++;
				}

				// test sums for unique
				if (sums.Distinct().Count() == 1)
				{
					// WIN! print square, n, progress
					prx = 0;
					pry = 0;
					buff = "";
					while (pry < height)
					{
						while (prx < width)
						{
							buff += (square[prx + (pry * height)].ToString() + " ");
							prx++;
						}
						pr(buff);
						pry++;
						prx = 0;
						buff = "";
					}
					pr("n=" + sums[0]);
					wins++;
					pr("Solved " + wins.ToString() + " of " + tosolve.ToString() + "\n");
				}
			}
		}
		// reset sums
		sums = new int[width + height + 2];
	}
	// cascade the square
	while (loc < count)
	{
		if (square[loc] >= limit)
		{
			square[loc] = start;
			loc++;
		}
		else
		{
			if (loc != 0) { square[loc]++; }
			break;
		}
	}
	loc = 0;
	// loop, increment, continue
}

// stop timer, print diagnostics
s1.Stop();
pr(width.ToString() + "x" + height.ToString() + " square from " + start.ToString() + " to " + limit.ToString());
pr("Total solved: " + wins.ToString());
pr("Total time: " + ((double)(s1.Elapsed.TotalMilliseconds / 1000)).ToString("0.000 s"));
pr("Finished with " + iterations.ToString("N0") + " cycles");
pr((iterations / (s1.Elapsed.TotalMilliseconds / 1000)).ToString("N0") + " cycles per second");

// function to print to console
static void pr(string str)
{
	Console.WriteLine(str);
}

// function to count duplicates in an array
static IEnumerable<int> FindUnique(int[] array)
{
	// Use nested loop to check for duplicates.
	List<int> result = new();
	for (int i = 0; i < array.Length; i++)
	{
		// Check for duplicates in all following elements.
		bool isDuplicate = false;
		for (int y = i + 1; y < array.Length; y++)
		{
			if (array[i] == array[y])
			{
				isDuplicate = true;
				break;
			}
		}
		if (!isDuplicate)
		{
			result.Add(array[i]);
		}
	}
	return result;
}