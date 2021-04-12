﻿using DevilDaggersAssetEditor.BinaryFileHandlers;
using DevilDaggersAssetEditor.Chunks;
using DevilDaggersAssetEditor.Wpf.Extensions;
using DevilDaggersCore.Wpf.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersAssetEditor.Wpf.Gui.Windows
{
	public partial class TrimBinaryWindow : Window
	{
		private string? _originalFilePath;
		private string? _compareFilePath;
		private string? _outputFilePath;

		public TrimBinaryWindow()
		{
			InitializeComponent();
		}

		private void BrowseOriginalButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog fileDialog = new();
			fileDialog.OpenDevilDaggersModsFolder();

			if (fileDialog.ShowDialog() == true)
			{
				_originalFilePath = fileDialog.FileName;
				TextBoxOriginal.Text = _originalFilePath;
				UpdateButtonTrimBinary();
			}
		}

		private void TextBoxOriginal_TextChanged(object sender, TextChangedEventArgs e)
		{
			_originalFilePath = TextBoxOriginal.Text;
			UpdateButtonTrimBinary();
		}

		private void BrowseCompareButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog fileDialog = new();
			fileDialog.OpenDevilDaggersModsFolder();

			if (fileDialog.ShowDialog() == true)
			{
				_compareFilePath = fileDialog.FileName;
				TextBoxCompare.Text = _compareFilePath;
				UpdateButtonTrimBinary();
			}
		}

		private void TextBoxCompare_TextChanged(object sender, TextChangedEventArgs e)
		{
			_compareFilePath = TextBoxCompare.Text;
			UpdateButtonTrimBinary();
		}

		private void BrowseOutputButton_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog fileDialog = new();
			fileDialog.OpenDevilDaggersModsFolder();

			if (fileDialog.ShowDialog() == true)
			{
				_outputFilePath = fileDialog.FileName;
				TextBoxOutput.Text = _outputFilePath;
				UpdateButtonTrimBinary();
			}
		}

		private void TextBoxOutput_TextChanged(object sender, TextChangedEventArgs e)
		{
			_outputFilePath = TextBoxOutput.Text;
			UpdateButtonTrimBinary();
		}

		public void UpdateButtonTrimBinary()
		{
			ButtonTrimBinary.IsEnabled = !string.IsNullOrWhiteSpace(_originalFilePath) && !string.IsNullOrWhiteSpace(_compareFilePath) && !string.IsNullOrWhiteSpace(_outputFilePath) && File.Exists(_originalFilePath) && File.Exists(_compareFilePath);
		}

		private void TrimBinary_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(_outputFilePath) || !ValidateFileExists(_originalFilePath) || !ValidateFileExists(_compareFilePath))
				return;

			List<Chunk> originalChunks = GetChunksFromFile(_originalFilePath!);
			List<Chunk> compareChunks = GetChunksFromFile(_compareFilePath!);
			if (originalChunks.Count == 0 || compareChunks.Count == 0)
				return;

			List<Chunk> remainingChunks = new();

			foreach (Chunk chunk in originalChunks)
			{
				Chunk? compareChunk = compareChunks.Find(c => c.AssetType == chunk.AssetType && c.Name == chunk.Name);
				if (!HaveIdenticalAssets(chunk, compareChunk))
					remainingChunks.Add(chunk);
			}

			BinaryFileHandler.CreateTocStream(remainingChunks, out byte[] tocBuffer, out Dictionary<Chunk, long> startOffsetBytePositions);
			byte[] assetBuffer = BinaryFileHandler.CreateAssetStream(remainingChunks, tocBuffer, startOffsetBytePositions, null);
			byte[] binaryBytes = BinaryFileHandler.CreateBinary(tocBuffer, assetBuffer);
			File.WriteAllBytes(_outputFilePath, binaryBytes);

			static bool ValidateFileExists(string? filePath)
			{
				if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
				{
					MessageWindow window = new("File not found.", $"The file at {filePath} was not found.");
					window.ShowDialog();
					return false;
				}

				return true;
			}

			static List<Chunk> GetChunksFromFile(string filePath)
			{
				byte[] fileBytes = File.ReadAllBytes(filePath);
				if (!BinaryFileHandler.IsValidFile(fileBytes))
				{
					MessageWindow window = new("Invalid file format.", "Make sure to open one of the following binary files: audio, core, dd");
					window.ShowDialog();
					return new();
				}

				byte[] tocBuffer = BinaryFileHandler.ReadTocBuffer(fileBytes);
				List<Chunk> chunks = BinaryFileHandler.ReadChunks(tocBuffer);

				foreach (Chunk chunk in chunks)
				{
					if (chunk.Size == 0) // Filter empty chunks (garbage in TOC buffers).
						continue;

					chunk.Buffer = new byte[chunk.Size];
					Buffer.BlockCopy(fileBytes, (int)chunk.StartOffset, chunk.Buffer, 0, (int)chunk.Size);
				}

				return chunks;
			}

			static bool HaveIdenticalAssets(Chunk chunk, Chunk? compareChunk)
			{
				if (compareChunk == null || chunk.Buffer.Length != compareChunk.Buffer.Length)
					return false;

				int fileLength = chunk.Buffer.Length;

				// Ignore mipmap data when comparing textures.
				if (chunk is TextureChunk textureChunk)
				{
					int width = BitConverter.ToInt32(chunk.Buffer, 2);
					int height = BitConverter.ToInt32(chunk.Buffer, 6);
					fileLength = width * height * 4;
				}

				for (int i = 0; i < fileLength; i++)
				{
					if (chunk.Buffer[i] != compareChunk.Buffer[i])
						return false;
				}

				return true;
			}
		}
	}
}