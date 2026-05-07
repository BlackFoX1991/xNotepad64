using System.Buffers;
using System.Text;

namespace xNotepad64
{
    public sealed class Chunk
    {
        public Chunk(int index, long start, long length)
        {
            Index = index;
            Start = start;
            Length = length;
        }

        public int Index { get; }

        public long Start { get; }

        public long Length { get; }

        public long EndExclusive => Start + Length;
    }

    public sealed class SearchResult
    {
        public int ChunkIndex { get; set; }

        public int PositionInChunk { get; set; }

        public int MatchLength { get; set; }

        public string Preview { get; set; } = string.Empty;
    }

    internal enum DocumentEncodingKind
    {
        Utf8,
        Utf16LittleEndian,
        Utf16BigEndian,
        Utf32LittleEndian,
        Utf32BigEndian,
        SingleByte
    }

    public static class Textfile
    {
        private static readonly Dictionary<int, string> ModifiedChunks = [];
        private static readonly Dictionary<int, string> OriginalChunkSnapshots = [];
        private static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        private static DocumentEncodingKind _encodingKind = DocumentEncodingKind.Utf8;
        private static byte[] _filePreamble = [];
        private static long _openedFileLength;

        public static string? FilePath { get; private set; }

        public static Encoding FileEncoding { get; private set; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static long MaximumChunkSize { get; set; } = EditorSettings.DefaultChunkSizeBytes;

        public static bool AllowAbruptChunkCutoff { get; set; }

        public static List<Chunk> ChunkBlocks { get; private set; } = [];

        public static int CurrentChunkIndex { get; set; } = -1;

        public static bool HasOpenFile => !string.IsNullOrWhiteSpace(FilePath);

        public static bool HasUnsavedChanges => ModifiedChunks.Count > 0;

        public static int ModifiedChunkCount => ModifiedChunks.Count;

        public static string FileName => FilePath is null ? string.Empty : Path.GetFileName(FilePath);

        public static long SavedFileLength => _openedFileLength;

        public static long CurrentDocumentLength => !HasOpenFile ? 0 : _openedFileLength + GetModifiedLengthDelta();

        public static async Task OpenAsync(string filePath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("A valid file path is required.", nameof(filePath));
            }

            progress?.Report(new OperationProgress("Datei wird analysiert", "Zeichencodierung wird erkannt."));

            (Encoding encoding, DocumentEncodingKind encodingKind, byte[] preamble) = await DetectEncodingAsync(filePath, cancellationToken);
            List<Chunk> chunkBlocks = await InitializeChunkBlocksAsync(filePath, encodingKind, preamble.Length, progress, cancellationToken);

            FilePath = filePath;
            FileEncoding = encoding;
            _encodingKind = encodingKind;
            _filePreamble = preamble;
            _openedFileLength = new FileInfo(filePath).Length;
            ChunkBlocks = chunkBlocks;
            CurrentChunkIndex = chunkBlocks.Count == 0 ? -1 : 0;
            ModifiedChunks.Clear();
            OriginalChunkSnapshots.Clear();
        }

        public static bool IsChunkModified(int chunkIndex)
        {
            return ModifiedChunks.ContainsKey(chunkIndex);
        }

        public static bool TryGetOriginalChunkText(int chunkIndex, out string originalText)
        {
            return OriginalChunkSnapshots.TryGetValue(chunkIndex, out originalText!);
        }

        public static async Task<string> GetChunkTextAsync(int chunkIndex, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            if (chunkIndex < 0 || chunkIndex >= ChunkBlocks.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkIndex));
            }

            if (ModifiedChunks.TryGetValue(chunkIndex, out string? draft))
            {
                progress?.Report(new OperationProgress("Chunk geladen", $"Chunk {chunkIndex + 1} wurde aus dem Zwischenspeicher geladen.", 1, 1));
                return draft;
            }

            return await ReadChunkAsync(ChunkBlocks[chunkIndex], progress, cancellationToken);
        }

        public static void SetChunkDraft(int chunkIndex, string content, string originalContent)
        {
            if (chunkIndex < 0 || chunkIndex >= ChunkBlocks.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkIndex));
            }

            if (string.Equals(content, originalContent, StringComparison.Ordinal))
            {
                ModifiedChunks.Remove(chunkIndex);
                OriginalChunkSnapshots.Remove(chunkIndex);
                return;
            }

            OriginalChunkSnapshots[chunkIndex] = originalContent;
            ModifiedChunks[chunkIndex] = content;
        }

        public static async Task SaveAsync(IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            EnsureFileIsOpen();

            if (ModifiedChunks.Count == 0)
            {
                progress?.Report(new OperationProgress("Speichern uebersprungen", "Es liegen keine Aenderungen vor.", 1, 1));
                return;
            }

            await SaveToPathAsync(FilePath!, replaceExistingTarget: true, progress, cancellationToken);
        }

        public static async Task SaveAsAsync(string destinationPath, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            EnsureFileIsOpen();

            if (string.IsNullOrWhiteSpace(destinationPath))
            {
                throw new ArgumentException("A valid destination path is required.", nameof(destinationPath));
            }

            string normalizedDestinationPath = Path.GetFullPath(destinationPath);
            string normalizedCurrentPath = Path.GetFullPath(FilePath!);

            if (string.Equals(normalizedDestinationPath, normalizedCurrentPath, StringComparison.OrdinalIgnoreCase))
            {
                await SaveAsync(progress, cancellationToken);
                return;
            }

            await SaveToPathAsync(normalizedDestinationPath, replaceExistingTarget: false, progress, cancellationToken);
        }

        private static async Task SaveToPathAsync(string destinationPath, bool replaceExistingTarget, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            string directoryPath = Path.GetDirectoryName(destinationPath) ?? AppContext.BaseDirectory;
            string tempPath = Path.Combine(directoryPath, $"{Path.GetFileName(destinationPath)}.{Guid.NewGuid():N}.tmp");
            string backupPath = Path.Combine(directoryPath, $"{Path.GetFileName(destinationPath)}.bak");

            byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 1024);
            long totalBytes = ChunkBlocks.Sum(chunk => chunk.Length);
            long writtenSourceBytes = 0;

            try
            {
                Directory.CreateDirectory(directoryPath);

                await using (FileStream sourceStream = OpenReadStream(useAsync: true))
                await using (var targetStream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: 1024 * 1024, options: FileOptions.SequentialScan | FileOptions.Asynchronous))
                {
                    if (_filePreamble.Length > 0)
                    {
                        await targetStream.WriteAsync(_filePreamble, cancellationToken);
                    }

                    foreach (Chunk chunk in ChunkBlocks)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (ModifiedChunks.TryGetValue(chunk.Index, out string? replacement))
                        {
                            byte[] encodedContent = FileEncoding.GetBytes(replacement);
                            await targetStream.WriteAsync(encodedContent, cancellationToken);
                            writtenSourceBytes += chunk.Length;
                            progress?.Report(new OperationProgress("Datei wird gespeichert", $"Chunk {chunk.Index + 1}/{ChunkBlocks.Count} wird ersetzt.", writtenSourceBytes, totalBytes));
                            continue;
                        }

                        await CopyChunkAsync(sourceStream, targetStream, chunk, buffer, totalBytes, progress, cancellationToken);
                        writtenSourceBytes += chunk.Length;
                    }
                }

                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                await ReplaceOrMoveTargetAsync(tempPath, destinationPath, backupPath, replaceExistingTarget, cancellationToken);

                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                FilePath = destinationPath;
                _openedFileLength = new FileInfo(destinationPath).Length;
                ModifiedChunks.Clear();
                OriginalChunkSnapshots.Clear();
                ChunkBlocks = await InitializeChunkBlocksAsync(destinationPath, _encodingKind, _filePreamble.Length, progress, cancellationToken);
                CurrentChunkIndex = ChunkBlocks.Count == 0 ? -1 : Math.Clamp(CurrentChunkIndex, 0, ChunkBlocks.Count - 1);
            }
            catch
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

                throw;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public static async Task<List<SearchResult>> SearchAsync(SearchQueryOptions query, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            EnsureFileIsOpen();

            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (query.SearchTerm.Length == 0)
            {
                throw new ArgumentException("A non-empty search term is required.", nameof(query));
            }

            var results = new List<SearchResult>();
            StringComparison comparison = query.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            long totalBytes = ChunkBlocks.Sum(chunk => chunk.Length);
            long processedBytes = 0;
            char? previousTrailingChar = null;

            string? nextChunkContent = ChunkBlocks.Count > 0
                ? await LoadChunkContentForSearchAsync(0, cancellationToken)
                : null;

            for (int chunkIndex = 0; chunkIndex < ChunkBlocks.Count; chunkIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Chunk chunk = ChunkBlocks[chunkIndex];
                string chunkContent = nextChunkContent ?? string.Empty;

                nextChunkContent = chunkIndex + 1 < ChunkBlocks.Count
                    ? await LoadChunkContentForSearchAsync(chunkIndex + 1, cancellationToken)
                    : null;

                char? nextLeadingChar = string.IsNullOrEmpty(nextChunkContent) ? null : nextChunkContent[0];
                int index = chunkContent.IndexOf(query.SearchTerm, comparison);

                while (index != -1)
                {
                    if (!query.WholeWord || IsWholeWordMatch(chunkContent, index, query.SearchTerm.Length, previousTrailingChar, nextLeadingChar))
                    {
                        results.Add(new SearchResult
                        {
                            ChunkIndex = chunk.Index,
                            PositionInChunk = index,
                            MatchLength = query.SearchTerm.Length,
                            Preview = BuildPreview(chunkContent, index, query.SearchTerm.Length)
                        });
                    }

                    index = chunkContent.IndexOf(query.SearchTerm, index + query.SearchTerm.Length, comparison);
                }

                processedBytes += chunk.Length;
                progress?.Report(new OperationProgress("Suche laeuft", $"Chunk {chunk.Index + 1}/{ChunkBlocks.Count} wird durchsucht.", processedBytes, totalBytes));
                previousTrailingChar = string.IsNullOrEmpty(chunkContent) ? previousTrailingChar : chunkContent[^1];
            }

            progress?.Report(new OperationProgress("Suche abgeschlossen", $"{results.Count} Treffer gefunden.", totalBytes, totalBytes));
            return results;
        }

        public static int FindChunkIndexByOffset(long offset)
        {
            if (ChunkBlocks.Count == 0)
            {
                return -1;
            }

            int low = 0;
            int high = ChunkBlocks.Count - 1;

            while (low <= high)
            {
                int mid = low + ((high - low) / 2);
                Chunk chunk = ChunkBlocks[mid];

                if (offset < chunk.Start)
                {
                    high = mid - 1;
                    continue;
                }

                if (offset >= chunk.EndExclusive)
                {
                    low = mid + 1;
                    continue;
                }

                return mid;
            }

            return Math.Clamp(low, 0, ChunkBlocks.Count - 1);
        }

        public static string FormatBytes(long bytes)
        {
            string[] units = ["B", "KiB", "MiB", "GiB", "TiB"];
            double value = bytes;
            int unitIndex = 0;

            while (value >= 1024 && unitIndex < units.Length - 1)
            {
                value /= 1024;
                unitIndex++;
            }

            return $"{value:0.##} {units[unitIndex]}";
        }

        public static string FormatOffset(long offset)
        {
            return offset.ToString("N0");
        }

        private static async Task<List<Chunk>> InitializeChunkBlocksAsync(string filePath, DocumentEncodingKind encodingKind, int preambleLength, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                if (MaximumChunkSize <= 0)
                {
                    throw new InvalidOperationException("MaximumChunkSize must be greater than zero.");
                }

                var chunkBlocks = new List<Chunk>();
                using FileStream stream = OpenReadStream(filePath, useAsync: false);

                long fileLength = stream.Length;
                long contentStart = Math.Min(fileLength, preambleLength);
                long contentLength = Math.Max(0, fileLength - contentStart);

                if (contentStart == fileLength)
                {
                    chunkBlocks.Add(new Chunk(0, contentStart, 0));
                    progress?.Report(new OperationProgress("Datei vorbereitet", "Leere Datei erkannt.", 1, 1));
                    return chunkBlocks;
                }

                long currentPosition = contentStart;
                int index = 0;

                while (currentPosition < fileLength)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    long proposedEnd = Math.Min(currentPosition + MaximumChunkSize, fileLength);
                    long end = proposedEnd;

                    if (end < fileLength)
                    {
                        end = AlignChunkEnd(stream, currentPosition, end, fileLength, encodingKind);
                    }

                    if (end <= currentPosition)
                    {
                        end = MoveToNextBoundary(stream, proposedEnd, fileLength, encodingKind);
                    }

                    if (end <= currentPosition)
                    {
                        end = fileLength;
                    }

                    if (end < fileLength && !AllowAbruptChunkCutoff)
                    {
                        end = ExtendChunkToWordBoundary(stream, end, fileLength, encodingKind);
                    }

                    chunkBlocks.Add(new Chunk(index, currentPosition, end - currentPosition));
                    currentPosition = end;
                    index++;

                    progress?.Report(new OperationProgress(
                        "Chunks werden vorbereitet",
                        $"Chunk {index} endet bei Byte {FormatOffset(end)}.",
                        currentPosition - contentStart,
                        contentLength));
                }

                if (chunkBlocks.Count == 0)
                {
                    chunkBlocks.Add(new Chunk(0, contentStart, 0));
                }

                progress?.Report(new OperationProgress("Datei vorbereitet", $"{chunkBlocks.Count} Chunks erstellt.", contentLength, contentLength));
                return chunkBlocks;
            }, cancellationToken);
        }

        private static async Task<string> ReadChunkAsync(Chunk chunk, IProgress<OperationProgress>? progress, CancellationToken cancellationToken, FileStream? existingStream = null)
        {
            if (chunk.Length > int.MaxValue)
            {
                throw new InvalidOperationException("The chunk is too large for the editor control. Reduce MaximumChunkSize.");
            }

            bool ownsStream = existingStream is null;
            await using FileStream? ownedStream = ownsStream ? OpenReadStream(useAsync: true) : null;
            FileStream stream = existingStream ?? ownedStream!;

            stream.Position = chunk.Start;

            byte[] buffer = new byte[chunk.Length];
            int totalRead = 0;

            while (totalRead < buffer.Length)
            {
                int bytesRead = await stream.ReadAsync(buffer.AsMemory(totalRead, buffer.Length - totalRead), cancellationToken);
                if (bytesRead == 0)
                {
                    throw new EndOfStreamException("Unexpected end of stream while reading the chunk.");
                }

                totalRead += bytesRead;
                progress?.Report(new OperationProgress(
                    "Chunk wird geladen",
                    $"Chunk {chunk.Index + 1}/{ChunkBlocks.Count} wird gelesen.",
                    totalRead,
                    buffer.Length));
            }

            progress?.Report(new OperationProgress("Chunk geladen", $"Chunk {chunk.Index + 1} ist bereit.", buffer.Length, buffer.Length));
            return FileEncoding.GetString(buffer);
        }

        private static async Task<string> LoadChunkContentForSearchAsync(int chunkIndex, CancellationToken cancellationToken)
        {
            if (ModifiedChunks.TryGetValue(chunkIndex, out string? draft))
            {
                return draft;
            }

            return await ReadChunkAsync(ChunkBlocks[chunkIndex], progress: null, cancellationToken);
        }

        private static async Task<(Encoding Encoding, DocumentEncodingKind EncodingKind, byte[] Preamble)> DetectEncodingAsync(string filePath, CancellationToken cancellationToken)
        {
            byte[] header = new byte[4];

            await using (FileStream stream = OpenReadStream(filePath, useAsync: true))
            {
                int read = await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);

                if (read >= 4 && header[0] == 0xFF && header[1] == 0xFE && header[2] == 0x00 && header[3] == 0x00)
                {
                    return (new UTF32Encoding(bigEndian: false, byteOrderMark: true), DocumentEncodingKind.Utf32LittleEndian, [0xFF, 0xFE, 0x00, 0x00]);
                }

                if (read >= 4 && header[0] == 0x00 && header[1] == 0x00 && header[2] == 0xFE && header[3] == 0xFF)
                {
                    return (new UTF32Encoding(bigEndian: true, byteOrderMark: true), DocumentEncodingKind.Utf32BigEndian, [0x00, 0x00, 0xFE, 0xFF]);
                }

                if (read >= 3 && header[0] == 0xEF && header[1] == 0xBB && header[2] == 0xBF)
                {
                    return (new UTF8Encoding(encoderShouldEmitUTF8Identifier: true), DocumentEncodingKind.Utf8, [0xEF, 0xBB, 0xBF]);
                }

                if (read >= 2 && header[0] == 0xFF && header[1] == 0xFE)
                {
                    return (new UnicodeEncoding(bigEndian: false, byteOrderMark: true), DocumentEncodingKind.Utf16LittleEndian, [0xFF, 0xFE]);
                }

                if (read >= 2 && header[0] == 0xFE && header[1] == 0xFF)
                {
                    return (new UnicodeEncoding(bigEndian: true, byteOrderMark: true), DocumentEncodingKind.Utf16BigEndian, [0xFE, 0xFF]);
                }
            }

            if (await LooksLikeUtf8Async(filePath, cancellationToken))
            {
                return (new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), DocumentEncodingKind.Utf8, []);
            }

            return (Encoding.Default, DocumentEncodingKind.SingleByte, []);
        }

        private static async Task<bool> LooksLikeUtf8Async(string filePath, CancellationToken cancellationToken)
        {
            await using FileStream stream = OpenReadStream(filePath, useAsync: true);
            int sampleLength = (int)Math.Min(stream.Length, 128 * 1024);

            if (sampleLength == 0)
            {
                return true;
            }

            byte[] sample = new byte[sampleLength];
            await stream.ReadExactlyAsync(sample.AsMemory(), cancellationToken);

            for (int trimBytes = 0; trimBytes <= 3 && sampleLength - trimBytes > 0; trimBytes++)
            {
                try
                {
                    StrictUtf8.GetCharCount(sample, 0, sampleLength - trimBytes);
                    return true;
                }
                catch (DecoderFallbackException)
                {
                    // Continue trimming a few trailing bytes in case the sample ended mid-sequence.
                }
            }

            return false;
        }

        private static long AlignChunkEnd(FileStream stream, long start, long proposedEnd, long fileLength, DocumentEncodingKind encodingKind)
        {
            return encodingKind switch
            {
                DocumentEncodingKind.Utf8 => AlignUtf8ChunkEnd(stream, start, proposedEnd, fileLength),
                DocumentEncodingKind.Utf16LittleEndian => AlignUtf16ChunkEnd(stream, start, proposedEnd, fileLength, littleEndian: true),
                DocumentEncodingKind.Utf16BigEndian => AlignUtf16ChunkEnd(stream, start, proposedEnd, fileLength, littleEndian: false),
                DocumentEncodingKind.Utf32LittleEndian or DocumentEncodingKind.Utf32BigEndian => AlignUtf32ChunkEnd(start, proposedEnd, fileLength),
                _ => proposedEnd
            };
        }

        private static long MoveToNextBoundary(FileStream stream, long start, long fileLength, DocumentEncodingKind encodingKind)
        {
            if (start >= fileLength)
            {
                return fileLength;
            }

            return encodingKind switch
            {
                DocumentEncodingKind.Utf8 => MoveUtf8BoundaryForward(stream, start, fileLength),
                DocumentEncodingKind.Utf16LittleEndian or DocumentEncodingKind.Utf16BigEndian => Math.Min(fileLength, start + 2),
                DocumentEncodingKind.Utf32LittleEndian or DocumentEncodingKind.Utf32BigEndian => Math.Min(fileLength, start + 4),
                _ => Math.Min(fileLength, start + 1)
            };
        }

        private static long ExtendChunkToWordBoundary(FileStream stream, long end, long fileLength, DocumentEncodingKind encodingKind)
        {
            if (end <= 0 || end >= fileLength)
            {
                return end;
            }

            if (!TryReadPreviousCharacter(stream, end, fileLength, encodingKind, out char previousChar))
            {
                return end;
            }

            if (!TryReadCharacterAt(stream, end, fileLength, encodingKind, out char currentChar, out _))
            {
                return end;
            }

            if (!IsWordChar(previousChar) || !IsWordChar(currentChar))
            {
                return end;
            }

            long position = end;

            while (position < fileLength)
            {
                if (!TryReadCharacterAt(stream, position, fileLength, encodingKind, out char character, out long nextPosition))
                {
                    break;
                }

                if (!IsWordChar(character))
                {
                    break;
                }

                position = nextPosition;
            }

            return position;
        }

        private static long AlignUtf8ChunkEnd(FileStream stream, long start, long proposedEnd, long fileLength)
        {
            long end = proposedEnd;

            while (end > start && end < fileLength && IsUtf8ContinuationByte(ReadByteAt(stream, end)))
            {
                end--;
            }

            if (end > start)
            {
                return end;
            }

            return MoveUtf8BoundaryForward(stream, proposedEnd, fileLength);
        }

        private static long MoveUtf8BoundaryForward(FileStream stream, long start, long fileLength)
        {
            long position = start;

            while (position < fileLength && IsUtf8ContinuationByte(ReadByteAt(stream, position)))
            {
                position++;
            }

            return position;
        }

        private static long AlignUtf16ChunkEnd(FileStream stream, long start, long proposedEnd, long fileLength, bool littleEndian)
        {
            long end = proposedEnd - ((proposedEnd - start) % 2);

            if (end <= start)
            {
                return Math.Min(fileLength, start + 2);
            }

            if (end >= fileLength || end - 2 < start)
            {
                return Math.Min(end, fileLength);
            }

            if (end + 1 >= fileLength)
            {
                return end;
            }

            ushort previousCodeUnit = ReadUInt16At(stream, end - 2, littleEndian);
            ushort nextCodeUnit = ReadUInt16At(stream, end, littleEndian);

            if (char.IsHighSurrogate((char)previousCodeUnit) && char.IsLowSurrogate((char)nextCodeUnit))
            {
                if (end - 2 > start)
                {
                    return end - 2;
                }

                return Math.Min(fileLength, end + 2);
            }

            return end;
        }

        private static long AlignUtf32ChunkEnd(long start, long proposedEnd, long fileLength)
        {
            long end = proposedEnd - ((proposedEnd - start) % 4);

            if (end <= start)
            {
                return Math.Min(fileLength, start + 4);
            }

            return Math.Min(end, fileLength);
        }

        private static bool TryReadPreviousCharacter(FileStream stream, long position, long fileLength, DocumentEncodingKind encodingKind, out char character)
        {
            character = '\0';

            return encodingKind switch
            {
                DocumentEncodingKind.Utf8 => TryReadPreviousUtf8Character(stream, position, out character),
                DocumentEncodingKind.Utf16LittleEndian => TryReadPreviousUtf16Character(stream, position, littleEndian: true, out character),
                DocumentEncodingKind.Utf16BigEndian => TryReadPreviousUtf16Character(stream, position, littleEndian: false, out character),
                DocumentEncodingKind.Utf32LittleEndian => TryReadPreviousUtf32Character(stream, position, littleEndian: true, out character),
                DocumentEncodingKind.Utf32BigEndian => TryReadPreviousUtf32Character(stream, position, littleEndian: false, out character),
                _ => TryReadPreviousSingleByteCharacter(stream, position, out character)
            };
        }

        private static bool TryReadCharacterAt(FileStream stream, long position, long fileLength, DocumentEncodingKind encodingKind, out char character, out long nextPosition)
        {
            character = '\0';
            nextPosition = position;

            return encodingKind switch
            {
                DocumentEncodingKind.Utf8 => TryReadUtf8CharacterAt(stream, position, fileLength, out character, out nextPosition),
                DocumentEncodingKind.Utf16LittleEndian => TryReadUtf16CharacterAt(stream, position, fileLength, littleEndian: true, out character, out nextPosition),
                DocumentEncodingKind.Utf16BigEndian => TryReadUtf16CharacterAt(stream, position, fileLength, littleEndian: false, out character, out nextPosition),
                DocumentEncodingKind.Utf32LittleEndian => TryReadUtf32CharacterAt(stream, position, fileLength, littleEndian: true, out character, out nextPosition),
                DocumentEncodingKind.Utf32BigEndian => TryReadUtf32CharacterAt(stream, position, fileLength, littleEndian: false, out character, out nextPosition),
                _ => TryReadSingleByteCharacterAt(stream, position, fileLength, out character, out nextPosition)
            };
        }

        private static bool TryReadPreviousSingleByteCharacter(FileStream stream, long position, out char character)
        {
            character = '\0';
            if (position <= 0)
            {
                return false;
            }

            return TryReadSingleByteCharacterAt(stream, position - 1, position, out character, out _);
        }

        private static bool TryReadSingleByteCharacterAt(FileStream stream, long position, long fileLength, out char character, out long nextPosition)
        {
            character = '\0';
            nextPosition = position;

            if (position >= fileLength)
            {
                return false;
            }

            byte[] buffer = ReadBytesAt(stream, position, 1);
            string decoded = FileEncoding.GetString(buffer);
            if (decoded.Length == 0)
            {
                return false;
            }

            character = decoded[0];
            nextPosition = position + 1;
            return true;
        }

        private static bool TryReadPreviousUtf8Character(FileStream stream, long position, out char character)
        {
            character = '\0';
            if (position <= 0)
            {
                return false;
            }

            long start = position - 1;
            while (start > 0 && IsUtf8ContinuationByte(ReadByteAt(stream, start)))
            {
                start--;
            }

            int length = (int)(position - start);
            if (length <= 0)
            {
                return false;
            }

            byte[] buffer = ReadBytesAt(stream, start, length);
            string decoded = FileEncoding.GetString(buffer);
            if (decoded.Length == 0)
            {
                return false;
            }

            character = decoded[0];
            return true;
        }

        private static bool TryReadUtf8CharacterAt(FileStream stream, long position, long fileLength, out char character, out long nextPosition)
        {
            character = '\0';
            nextPosition = position;

            if (position >= fileLength)
            {
                return false;
            }

            int firstByte = ReadByteAt(stream, position);
            int sequenceLength = GetUtf8SequenceLength(firstByte);
            if (position + sequenceLength > fileLength)
            {
                return false;
            }

            byte[] buffer = ReadBytesAt(stream, position, sequenceLength);
            string decoded = FileEncoding.GetString(buffer);
            if (decoded.Length == 0)
            {
                return false;
            }

            character = decoded[0];
            nextPosition = position + sequenceLength;
            return true;
        }

        private static bool TryReadPreviousUtf16Character(FileStream stream, long position, bool littleEndian, out char character)
        {
            character = '\0';
            if (position < 2)
            {
                return false;
            }

            long start = position - 2;
            ushort codeUnit = ReadUInt16At(stream, start, littleEndian);

            if (char.IsLowSurrogate((char)codeUnit) && position >= 4)
            {
                ushort previousCodeUnit = ReadUInt16At(stream, position - 4, littleEndian);
                if (char.IsHighSurrogate((char)previousCodeUnit))
                {
                    codeUnit = previousCodeUnit;
                }
            }

            character = (char)codeUnit;
            return true;
        }

        private static bool TryReadUtf16CharacterAt(FileStream stream, long position, long fileLength, bool littleEndian, out char character, out long nextPosition)
        {
            character = '\0';
            nextPosition = position;

            if (position + 2 > fileLength)
            {
                return false;
            }

            ushort codeUnit = ReadUInt16At(stream, position, littleEndian);
            character = (char)codeUnit;
            nextPosition = position + 2;

            if (char.IsHighSurrogate(character) && position + 4 <= fileLength)
            {
                ushort nextCodeUnit = ReadUInt16At(stream, position + 2, littleEndian);
                if (char.IsLowSurrogate((char)nextCodeUnit))
                {
                    nextPosition = position + 4;
                }
            }

            return true;
        }

        private static bool TryReadPreviousUtf32Character(FileStream stream, long position, bool littleEndian, out char character)
        {
            character = '\0';
            if (position < 4)
            {
                return false;
            }

            uint codePoint = ReadUInt32At(stream, position - 4, littleEndian);
            character = ConvertCodePointToChar(codePoint);
            return true;
        }

        private static bool TryReadUtf32CharacterAt(FileStream stream, long position, long fileLength, bool littleEndian, out char character, out long nextPosition)
        {
            character = '\0';
            nextPosition = position;

            if (position + 4 > fileLength)
            {
                return false;
            }

            uint codePoint = ReadUInt32At(stream, position, littleEndian);
            character = ConvertCodePointToChar(codePoint);
            nextPosition = position + 4;
            return true;
        }

        private static int GetUtf8SequenceLength(int firstByte)
        {
            return firstByte switch
            {
                <= 0x7F => 1,
                >= 0xC2 and <= 0xDF => 2,
                >= 0xE0 and <= 0xEF => 3,
                >= 0xF0 and <= 0xF4 => 4,
                _ => 1
            };
        }

        private static ushort ReadUInt16At(FileStream stream, long position, bool littleEndian)
        {
            int first = ReadByteAt(stream, position);
            int second = ReadByteAt(stream, position + 1);

            return littleEndian
                ? (ushort)(first | (second << 8))
                : (ushort)((first << 8) | second);
        }

        private static uint ReadUInt32At(FileStream stream, long position, bool littleEndian)
        {
            uint first = (uint)ReadByteAt(stream, position);
            uint second = (uint)ReadByteAt(stream, position + 1);
            uint third = (uint)ReadByteAt(stream, position + 2);
            uint fourth = (uint)ReadByteAt(stream, position + 3);

            return littleEndian
                ? first | (second << 8) | (third << 16) | (fourth << 24)
                : (first << 24) | (second << 16) | (third << 8) | fourth;
        }

        private static byte[] ReadBytesAt(FileStream stream, long position, int count)
        {
            var buffer = new byte[count];
            stream.Position = position;
            stream.ReadExactly(buffer.AsSpan());
            return buffer;
        }

        private static char ConvertCodePointToChar(uint codePoint)
        {
            return codePoint <= char.MaxValue ? (char)codePoint : '\0';
        }

        private static int ReadByteAt(FileStream stream, long position)
        {
            stream.Position = position;
            int value = stream.ReadByte();

            if (value < 0)
            {
                throw new EndOfStreamException("Unexpected end of stream while aligning chunk boundaries.");
            }

            return value;
        }

        private static bool IsUtf8ContinuationByte(int value)
        {
            return value is >= 0x80 and <= 0xBF;
        }

        private static async Task CopyChunkAsync(FileStream sourceStream, FileStream targetStream, Chunk chunk, byte[] buffer, long totalBytes, IProgress<OperationProgress>? progress, CancellationToken cancellationToken)
        {
            sourceStream.Position = chunk.Start;
            long remaining = chunk.Length;
            long processedWithinChunk = 0;

            while (remaining > 0)
            {
                int bytesToRead = (int)Math.Min(remaining, buffer.Length);
                int bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, bytesToRead), cancellationToken);

                if (bytesRead == 0)
                {
                    throw new EndOfStreamException("Unexpected end of stream while copying the original file.");
                }

                await targetStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                remaining -= bytesRead;
                processedWithinChunk += bytesRead;

                progress?.Report(new OperationProgress(
                    "Datei wird gespeichert",
                    $"Chunk {chunk.Index + 1}/{ChunkBlocks.Count} wird geschrieben.",
                    Math.Min(totalBytes, chunk.Start - _filePreamble.Length + processedWithinChunk),
                    totalBytes));
            }
        }

        private static async Task ReplaceOrMoveTargetAsync(string tempPath, string destinationPath, string backupPath, bool replaceExistingTarget, CancellationToken cancellationToken)
        {
            if (replaceExistingTarget || File.Exists(destinationPath))
            {
                await Task.Run(() => File.Replace(tempPath, destinationPath, backupPath, ignoreMetadataErrors: true), cancellationToken);
                return;
            }

            await Task.Run(() => File.Move(tempPath, destinationPath), cancellationToken);
        }

        private static long GetModifiedLengthDelta()
        {
            long delta = 0;

            foreach ((int chunkIndex, string replacement) in ModifiedChunks)
            {
                if (chunkIndex < 0 || chunkIndex >= ChunkBlocks.Count)
                {
                    continue;
                }

                delta += FileEncoding.GetByteCount(replacement) - ChunkBlocks[chunkIndex].Length;
            }

            return delta;
        }

        private static bool IsWholeWordMatch(string content, int index, int matchLength, char? previousTrailingChar, char? nextLeadingChar)
        {
            char? leftChar = index > 0 ? content[index - 1] : previousTrailingChar;
            char? rightChar = index + matchLength < content.Length ? content[index + matchLength] : nextLeadingChar;

            return !IsWordChar(leftChar) && !IsWordChar(rightChar);
        }

        private static bool IsWordChar(char? value)
        {
            return value.HasValue && (char.IsLetterOrDigit(value.Value) || value.Value == '_');
        }

        private static string BuildPreview(string chunkContent, int index, int matchLength)
        {
            int previewStart = Math.Max(0, index - 30);
            int previewEnd = Math.Min(chunkContent.Length, index + matchLength + 30);
            string preview = chunkContent[previewStart..previewEnd]
                .Replace(Environment.NewLine, " ")
                .Replace('\r', ' ')
                .Replace('\n', ' ');

            if (previewStart > 0)
            {
                preview = $"...{preview}";
            }

            if (previewEnd < chunkContent.Length)
            {
                preview = $"{preview}...";
            }

            return preview;
        }

        private static FileStream OpenReadStream(bool useAsync)
        {
            EnsureFileIsOpen();
            return OpenReadStream(FilePath!, useAsync);
        }

        private static FileStream OpenReadStream(string filePath, bool useAsync)
        {
            FileOptions options = FileOptions.SequentialScan;
            if (useAsync)
            {
                options |= FileOptions.Asynchronous;
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize: 1024 * 1024, options: options);
        }

        private static void EnsureFileIsOpen()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                throw new InvalidOperationException("No file is currently open.");
            }
        }
    }
}
