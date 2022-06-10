using System.Diagnostics;

// settings
int width = 3;
int height = 3;
int start = 1;
int limit = 30;

// init
int x;
int y;
int prx;
int pry;
int loc = 0;
int wins = 0;
int range = limit + start;
long iterations = 0;
string buff;
int count = (width * height);
int[] square = new int[width * height];
int[] sums = new int[width + height + 2];

// magic formula n^9-2n^8+n^7 or n^7*(n-1)^2 or n^7*(n^2-2n+1)
long cycles = (long)Math.Pow(limit,7) * ((limit - 1)*(limit - 1));

// zero out arrays of sums and squares
for (int i = 0; i < (width * height); i++) { square[i] = start; }
for (int i = 0; i < (width + height + 2); i++) { sums[i] = 0; }

// calculate how many solves to find, thanks Zaslavsky :)
var t = range;
int tosolve = (range % 12) switch
{
	0 or 2 or 6 or 8 => ((t * t * t) - 16 * t * t + 76 * t - 96) / 6,
	1 => ((t * t * t) - 16 * t * t + 73 * t - 58) / 6,
	3 or 11 => ((t * t * t) - 16 * t * t + 73 * t - 102) / 6,
	4 or 10 => ((t * t * t) - 16 * t * t + 76 * t - 112) / 6,
	5 or 9 => ((t * t * t) - 16 * t * t + 73 * t - 90) / 6,
	7 => ((t * t * t) - 16 * t * t + 73 * t - 70) / 6,
	_ => 0,
};

// test code ----------------
/*
t = 0;
while (t < 100)
{
	tosolve = (t % 12) switch
	{
		0 or 2 or 6 or 8 => ((t * t * t) - 16 * t * t + 76 * t - 96) / 6,
		1 => ((t * t * t) - 16 * t * t + 73 * t - 58) / 6,
		3 or 11 => ((t * t * t) - 16 * t * t + 73 * t - 102) / 6,
		4 or 10 => ((t * t * t) - 16 * t * t + 76 * t - 112) / 6,
		5 or 9 => ((t * t * t) - 16 * t * t + 73 * t - 90) / 6,
		7 => ((t * t * t) - 16 * t * t + 73 * t - 70) / 6,
		_ => 0,
	};
	pr(tosolve.ToString());
	t++;
}
*/
// test code ----------------

// start timer
var s1 = Stopwatch.StartNew();

// main loop, stops when final cell == limit, replaces while(iterations < total)
while (square[count -1] != limit) { 
	square[0]++;
	iterations++;

	
	// check if square has equal columns before continuing
	if ((square[0] + square[1] + square[2] == square[3] + square[4] + square[5]) &&  
		(square[0] + square[1] + square[2] == square[6] + square[7] + square[8]) &&
		(square[0] + square[3] + square[6] == square[2] + square[5] + square[8]) 
		)
	
	/*
	// alternate test, not as fast
	if ((square[0] + square[1] + square[2] == square[0] + square[4] + square[8]) &&
		(square[0] + square[1] + square[2] == square[6] + square[7] + square[8]) &&
		(square[0] + square[1] + square[2] == square[2] + square[5] + square[8]))
	*/
		{
		// check if square has unique values using function FindUnique(array)
		IEnumerable<int> unique = FindUnique(square);
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
				// Distinct() appears to be much faster here

				// int n = sums.Length / 1;
				// var dist = countDistinct(square, n);

				
				if (sums.Distinct().Count() == 1) 
				unique = FindUnique(sums);
				if (unique.Count() == 1)
				{
					// clearing seems to be very slow, but fun on higher solves (>13?)
					// if (range > 13) { Console.Clear(); }

					// WIN! print square, n, progress
					prx = 0;
					pry = 0;
					buff = "				";
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
						buff = "				";
					}
					wins++;
					int percentComplete = (int)(0.5f + ((100f * wins) / tosolve));
					pr(" n=" + sums[0] + " | Solved " + wins.ToString() + " of " + tosolve.ToString());
					// progress bar
					string pleft = new string((char)35, percentComplete / 2);
					string pright = new string((char)45, 50 - (percentComplete / 2));
					pr("[" + pleft + pright + "] " + percentComplete.ToString() + "% \n");
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

int countDistinct(int[] arr, int n)
{
	int res = 1;
	for (int i = 1; i < n; i++)
	{
		int j = 0;
		for (j = 0; j < i; j++)
			if (arr[i] == arr[j])
				break;
		if (i == j)
			res++;
	}
	return res;
}