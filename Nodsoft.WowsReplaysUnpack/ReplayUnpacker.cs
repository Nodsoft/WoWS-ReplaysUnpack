using Nodsoft.WowsReplaysUnpack.Data;
using Nodsoft.WowsReplaysUnpack.Infrastructure;
using Nodsoft.WowsReplaysUnpack.Infrastructure.Exceptions;
using Nodsoft.WowsReplaysUnpack.Infrastructure.ReplayParser;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Nodsoft.WowsReplaysUnpack;


public class ReplayUnpacker
{
	private static readonly byte[] ReplaySignature = Encoding.UTF8.GetBytes("\x12\x32\x34\x11");

	/// <summary>
	/// Unpacks a replay from a stream.
	/// Uses the default <see cref="ReplayParserProvider.Instance">ReplayParserProvider instance</see>.
	/// </summary>
	/// <param name="stream">The stream containing the replay file content.</param>
	/// <returns>The unpacked replay, wrapped in a <see cref="ReplayRaw"/> instance.</returns>
	/// <exception cref="InvalidReplayException">Occurs if the replay file is not valid.</exception>
	public ReplayRaw UnpackReplay(Stream stream)
	{
		return UnpackReplay(stream, ReplayParserProvider.Instance);
	}
	
	/// <summary>
	/// Unpacks a replay from a stream.
	/// Uses a custom <see cref="IReplayParserProvider"/> implementation.
	/// </summary>
	/// <param name="stream">The stream containing the replay file content.</param>
	/// <param name="parserProvider">The <see cref="IReplayParserProvider"/> implementation to use to retrieve the necessary <see cref="IReplayParser"/>.</param>
	/// <returns>The unpacked replay, wrapped in a <see cref="ReplayRaw"/> instance.</returns>
	/// <exception cref="InvalidReplayException">Occurs if the replay file is not valid.</exception>
	public ReplayRaw UnpackReplay(Stream stream, IReplayParserProvider parserProvider)
	{
		byte[] bReplaySignature = new byte[4];
		byte[] bReplayBlockCount = new byte[4];
		byte[] bReplayBlockSize = new byte[4];

		stream.Read(bReplaySignature, 0, 4);
		stream.Read(bReplayBlockCount, 0, 4);
		stream.Read(bReplayBlockSize, 0, 4);

		// Verify replay signature
		if (!bReplaySignature.SequenceEqual(ReplaySignature))
		{
			throw new InvalidReplayException("Invalid replay signature.");
		}

		int jsonDataSize = BitConverter.ToInt32(bReplayBlockSize, 0);
		byte[] bReplayJsonData = new byte[jsonDataSize];
		stream.Read(bReplayJsonData, 0, jsonDataSize);
		int blockCount = BitConverter.ToInt32(bReplayBlockCount);
		int blockSize = BitConverter.ToInt32(bReplayBlockSize);

		JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
		options.Converters.Add(new DateTimeJsonConverter());
		Utf8JsonReader reader = new(bReplayJsonData);
		ReplayMetadata metadata = new()
		{
			ArenaInfo = JsonSerializer.Deserialize<ArenaInfo>(ref reader, options) ?? throw new InvalidReplayException(),
			BReplaySignature = bReplaySignature,
			ReplayBlockCount = blockCount,
			ReplayBlockSize = blockSize,
		};

		// Read through extra data
		for (int i = 0; i < blockCount - 1; i++)
		{
			byte[] bBlockSize = new byte[4];
			stream.Read(bBlockSize);
			int extraBlockSize = BitConverter.ToInt32(bBlockSize);
			byte[] bData = new byte[extraBlockSize];
			stream.Read(bData);
		}

		using MemoryStream memoryStream = new();
		stream.CopyTo(memoryStream);
		MemoryStream decryptedData = DecryptAndDecompressData(memoryStream);

		Version replayVersion = Version.Parse(string.Join('.', metadata.ArenaInfo.ClientVersionFromExe.Split(',')[..3]));
		IReplayParser replayParser = parserProvider.FromReplayVersion(replayVersion);
		
		ReplayRaw replay = replayParser.ParseReplay(decryptedData, metadata);

		return replay;
	}

	private MemoryStream DecryptAndDecompressData(Stream dirtyData)
	{
		byte[] byteBlowfishKey = GlobalConstants.BlowfishKey.Select(Convert.ToByte).ToArray();
		Blowfish blowfish = new(byteBlowfishKey);
		long prev = 0;

		using MemoryStream compressedData = new();
		dirtyData.Seek(8, SeekOrigin.Begin);

		foreach (byte[] chunk in Utilities.ChunkData(dirtyData))
		{
			try
			{
				long decryptedBlock = BitConverter.ToInt64(blowfish.Decrypt_ECB(chunk));

				if (prev is not 0)
				{
					decryptedBlock ^= prev;
				}

				prev = decryptedBlock;
				compressedData.Write(BitConverter.GetBytes(decryptedBlock));
			}
			catch (ArgumentOutOfRangeException) { }
		}
		
		compressedData.Seek(2, SeekOrigin.Begin); //DeflateStream doesn't strip the header so we strip it manually.
		MemoryStream decompressedData = new();

		using (DeflateStream df = new(compressedData, CompressionMode.Decompress))
		{
			df.CopyTo(decompressedData);
		}

		decompressedData.Seek(0, SeekOrigin.Begin);
		return decompressedData;
	}
}
