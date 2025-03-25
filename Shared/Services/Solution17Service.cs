using System.Globalization;
using System.Text;

namespace I18NPuzzles.Services
{
    class GridPoint {
        public int X {get; set;}
        public int Y {get; set;}
        public byte Value {get; set;}
    }

    // (ctrl/command + click) the link to open the input file
    // file://./../../Inputs/17.txt
    public class Solution17Service : ISolutionDayService
    {
        class GridPoint {
            public int X {get; set;}
            public int Y {get; set;}
            public byte? Value {get; set;}
        }

        public string RunSolution(bool example)
        {
            List<string> lines = FileUtility.GetInputLines(17, example);
            List<List<List<byte>>> pieces = lines.ChunkByExclusive(string.IsNullOrWhiteSpace).Select(p => p.Select(p => p.Chunk(2).Select(b => byte.Parse(new string(b.ToArray()), NumberStyles.HexNumber)).ToList()).ToList()).ToList();
            int totalPieces = pieces.Count;

            // Get dimensions
            int height = pieces.SelectMany().Select(line => Encoding.UTF8.GetString(line.ToArray()).First()).Count(c => c == '|' || c == '║') + 2;
            int totalBytes = pieces.SelectMany().SelectMany().Count();
            int widthInBytes = totalBytes / height;

            // Populate grid
            List<List<byte?>> grid = [];

            foreach (int y in height) {
                List<byte?> row = [];
                foreach (int x in widthInBytes) {
                    row.Add(null);
                }
                grid.Add(row);
            }

            // Place corners
            List<List<byte>> topLeftPiece = pieces.First(p => Encoding.UTF8.GetString(p.First().ToArray()).First() == '╔');
            pieces.Remove(topLeftPiece);
            List<List<byte>> topRightPiece = pieces.First(p => Encoding.UTF8.GetString(p.First().ToArray()).Last() == '╗');
            pieces.Remove(topRightPiece);
            List<List<byte>> bottomLeftPiece = pieces.First(p => Encoding.UTF8.GetString(p.Last().ToArray()).First() == '╚');
            pieces.Remove(bottomLeftPiece);
            List<List<byte>> bottomRightPiece = pieces.First(p => Encoding.UTF8.GetString(p.Last().ToArray()).Last() == '╝');
            pieces.Remove(bottomRightPiece);

            for (int y = 0; y < topLeftPiece.Count; y++) {
                List<byte> row = topLeftPiece[y];
                for (int x = 0; x < row.Count; x++) {
                    grid[y][x] = row[x];
                }
            }

            for (int y = 0; y < topRightPiece.Count; y++) {
                List<byte> row = topRightPiece[y];
                for (int x = 0; x < row.Count; x++) {
                    grid[y][widthInBytes - row.Count + x] = row[x];
                }
            }

            for (int y = 0; y < bottomLeftPiece.Count; y++) {
                List<byte> row = bottomLeftPiece[y];
                for (int x = 0; x < row.Count; x++) {
                    grid[height - bottomLeftPiece.Count + y][x] = row[x];
                }
            }
            
            for (int y = 0; y < bottomRightPiece.Count; y++) {
                List<byte> row = bottomRightPiece[y];
                for (int x = 0; x < row.Count; x++) {
                    grid[height - bottomRightPiece.Count + y][widthInBytes - row.Count + x] = row[x];
                }
            }

            // Get edge pieces
            List<List<List<byte>>> topEdges = pieces.Where(p => Encoding.UTF8.GetString(p.First().ToArray()).Contains('═')).ToList();
            foreach (List<List<byte>> piece in topEdges) {
                pieces.Remove(piece);
            }

            List<List<List<byte>>> bottomEdges = pieces.Where(p => Encoding.UTF8.GetString(p.Last().ToArray()).Contains('═')).ToList();
            foreach (List<List<byte>> piece in bottomEdges) {
                pieces.Remove(piece);
            }

            List<List<List<byte>>> leftEdges = pieces.Where(p => p.Select(r => Encoding.UTF8.GetString(r.ToArray()).First()).Contains('|')).ToList();
            foreach (List<List<byte>> piece in leftEdges) {
                pieces.Remove(piece);
            }
            
            List<List<List<byte>>> rightEdges = pieces.Where(p => p.Select(r => Encoding.UTF8.GetString(r.ToArray()).Last()).Contains('|')).ToList();
            foreach (List<List<byte>> piece in rightEdges) {
                pieces.Remove(piece);
            }

            List<List<List<byte>>> centerPieces = pieces.ToList();

            while (grid.Any(r => r.Any(c => c == null))) {
                int currentY = grid.FindIndex(r => r.Contains(null));
                int currentX = grid[currentY].IndexOf(null);

                List<List<List<byte>>> candidates = GetCandidates(grid, currentX, currentY, widthInBytes, height, topEdges, bottomEdges, leftEdges, rightEdges, centerPieces);

                // I guess just selecting the first option works??
                List<List<byte>> candidate = candidates[0];

                // Remove from list of available pieces
                if (currentY == 0) {
                    topEdges.Remove(candidate);
                }
                else if (currentX == 0) {
                    leftEdges.Remove(candidate);
                }
                else if (rightEdges.Count > 0 && currentX == (widthInBytes - rightEdges[0][0].Count)) {
                    rightEdges.Remove(candidate);
                }
                else if (bottomEdges.Count > 0 && currentY == (height - bottomEdges[0].Count)) {
                    bottomEdges.Remove(candidate);
                }
                else {
                    centerPieces.Remove(candidate);
                }

                // Set the value of the piece
                foreach (int y in candidate.Count) {
                    List<byte> row = candidate[y];
                    foreach (int x in row.Count) {
                        grid[currentY + y][currentX + x] = row[x];
                    }
                }
            }
            
            PrintGrid(grid);

            List<string> gridText = grid.Select(r => Encoding.UTF8.GetString(r.Select(b => b!.Value).ToArray())).ToList();

            int targetY = gridText.FindIndex(r => r.Contains('╳'));
            int targetX = gridText[targetY].ToTextElementList().FindIndex(c => c == "╳");

            int answer = targetX * targetY;

            return answer.ToString();
        }

        private static void PrintGrid(List<List<byte?>> grid) {
            Console.OutputEncoding = Encoding.UTF8;

            foreach (List<byte?> row in grid) {
                List<List<byte?>> chunks = row.ChunkByInclusive(b => b == null);
                List<string> parts = chunks.Select(c => Encoding.UTF8.GetString(c.Select(b => b ?? (byte)' ').ToArray())).ToList();
                string line = string.Join("", parts);

                Console.WriteLine(line);
            }
        }

        private static List<List<List<byte>>> GetCandidates(List<List<byte?>> grid, int currentX, int currentY, int widthInBytes, int height, List<List<List<byte>>> topEdges, List<List<List<byte>>> bottomEdges, List<List<List<byte>>> leftEdges, List<List<List<byte>>> rightEdges, List<List<List<byte>>> centerPieces) {
            List<List<List<byte>>> candidates = [];

            List<List<List<byte>>> pieces;

            if (currentY == 0) {
                pieces = topEdges;
            }
            else if (currentX == 0) {
                pieces = leftEdges;
            }
            else if (rightEdges.Count > 0 && currentX == (widthInBytes - rightEdges[0][0].Count)) {
                pieces = rightEdges;
            }
            else if (bottomEdges.Count > 0 && bottomEdges.Any(be => currentY == (height - be.Count))) {
                pieces = bottomEdges.Where(be => currentY == (height - be.Count)).ToList().Concat(centerPieces).ToList();
            }
            else {
                pieces = centerPieces;
            }

            foreach (List<List<byte>> candidate in pieces) {
                bool match = true;
                for (int y = 0; y < candidate.Count; y++) {
                    List<byte?> right = candidate[y].Select(b => (byte?)b).ToList();

                    if (currentX != 0) {
                        List<byte?> left = [grid[currentY + y][currentX - 4], grid[currentY + y][currentX - 3], grid[currentY + y][currentX - 2], grid[currentY + y][currentX - 1]];
                        
                        match = IsValidMatch(left, right);

                        if (!match) {
                            break;
                        }
                    }
                    
                    // Check on the right edge if there's space on the grid
                    if (currentX + right.Count < grid[currentY + y].Count) {
                        List<byte?> afterRight = grid[currentY + y].Skip(currentX + right.Count).Take(4).ToList();

                        if (afterRight.Any(b => b != null)) {
                            match = IsValidMatch(right, afterRight);

                            if (!match) {
                                break;
                            }
                        }
                    }
                }

                if (match) {
                    candidates.Add(candidate);
                }
            }
        
            return candidates;
        }

        private static bool IsValidMatch(List<byte?> left, List<byte?> right) {
            if (left.Contains(null)) {
                return true;
            }

            if (left[^1] >= 0b1111_0000) {
                if (right[0] >= 0b1000_0000 && right[0] < 0b1100_0000 && right[1] >= 0b1000_0000 && right[1] < 0b1100_0000 && right[2] >= 0b1000_0000 && right[2] < 0b1100_0000) {
                    // Found 3 following bytes
                    return true;
                }
                else {
                    return false;
                }
            }
            else if (left[^1] >= 0b1110_0000) {
                if (right[0] >= 0b1000_0000 && right[0] < 0b1100_0000 && right[1] >= 0b1000_0000 && right[1] < 0b1100_0000) {
                    // Found 2 following bytes
                    return true;
                }
                else {
                    return false;
                }
            }
            else if (left[^1] >= 0b1100_0000) {
                if (right[0] >= 0b1000_0000 && right[0] < 0b1100_0000) {
                    // Found 1 following byte
                    return true;
                }
                else {
                    return false;
                }
            }
            else if (left[^1] >= 0b1000_0000) {
                // Check the previous bit
                if (left[^2] >= 0b1111_0000) {
                    if (right[0] >= 0b1000_0000 && right[0] < 0b1100_0000 && right[1] >= 0b1000_0000 && right[1] < 0b1100_0000) {
                        // Found 3 following bytes
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else if (left[^2] >= 0b1110_0000) {
                    if (right[0] >= 0b1000_0000 && right[0] < 0b1100_0000) {
                        // Found 2 following bytes
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else if (left[^2] >= 0b1100_0000) {
                    // Found 1 following byte
                    if (right[0] < 0b1000_0000 || right[0] >= 0b1100_0000) {
                        // Single byte
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else if (left[^2] >= 0b1000_0000) {
                    // Check the previous bit
                    if (left[^3] >= 0b1111_0000) {
                        if (right[0] >= 0b1000_0000 && right[0] < 0b1100_0000) {
                            // Found 3 following bytes
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else if (left[^3] >= 0b1110_0000) {
                        // Found 2 following bytes
                        if (right[0] < 0b1000_0000 || right[0] >= 0b1100_0000) {
                            // Single byte
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    else if (left[^3] >= 0b1100_0000) {
                        throw new Exception("Unexpected byte");
                    }
                    else if (left[^3] >= 0b1000_0000) {
                        // Check the previous bit
                        if (left[^4] >= 0b1111_0000) {
                            // Found 3 following bytes
                            if (right[0] < 0b1000_0000 || right[0] >= 0b1100_0000) {
                                // Single byte
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                        else {
                            throw new Exception("Unexpected byte");
                        }
                    }
                    else {
                        throw new Exception("Unexpected byte");
                    }
                }
                else {
                    throw new Exception("Unexpected byte");
                }
            }
            else if (right[0] < 0b1000_0000 || right[0] >= 0b1100_0000) {
                // Single byte
                return true;
            }
            else {
                return false;
            }
        }
    }
}