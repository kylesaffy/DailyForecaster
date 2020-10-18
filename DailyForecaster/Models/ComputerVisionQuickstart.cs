using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Linq;

namespace DailyForecaster.Models
{

	public class ComputerVisionQuickstart
	{
		
		
	}
	public class RunReader
	{
		// Add your Computer Vision subscription key and endpoint
		static string subscriptionKey = "3eb11a61121b44569ee2dae902b8dac9";
		static string endpoint = "https://mminvoicereader.cognitiveservices.azure.com/";
		// URL image used for analyzing an image (image of puppy)
		private const string ANALYZE_URL_IMAGE = "https://moderatorsampleimages.blob.core.windows.net/samples/sample16.png";
		//private const string READ_TEXT_URL_IMAGE = "https://storageaccountmoney9367.blob.core.windows.net/inoices/KS 003.jpeg";
		public List<string> Results { get; set; }
		public async Task<RunReader> GetRunReader(string url)
		{
			// Create a client
			ComputerVisionClient client = Authenticate(endpoint, subscriptionKey);
			// Analyze an image to get features and other properties.
			//AnalyzeImageUrl(client, ANALYZE_URL_IMAGE).Wait();

			// Extract text (OCR) from a URL image using the Read API
			RunReader reader = new RunReader();
			reader.Results = await ReadFileUrl(client, url);
			// Extract text (OCR) from a local image using the Read API
			//ReadFileLocal(client, READ_TEXT_LOCAL_IMAGE).Wait();
			return reader;
		}
		/*
		 * AUTHENTICATE
		 * Creates a Computer Vision client used by each example.
		 */
		public static ComputerVisionClient Authenticate(string endpoint, string key)
		{
			ComputerVisionClient client =
			  new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
			  { Endpoint = endpoint };
			return client;
		}
		/* 
		 * ANALYZE IMAGE - URL IMAGE
		 * Analyze URL image. Extracts captions, categories, tags, objects, faces, racy/adult content,
		 * brands, celebrities, landmarks, color scheme, and image types.
		 */
		public static async Task AnalyzeImageUrl(ComputerVisionClient client, string imageUrl)
		{
			Console.WriteLine("----------------------------------------------------------");
			Console.WriteLine("ANALYZE IMAGE - URL");
			Console.WriteLine();

			// Creating a list that defines the features to be extracted from the image. 

			List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
			{
				VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
				VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
				VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
				VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
				VisualFeatureTypes.Objects
			};
		}
		/*
		 * READ FILE - URL 
		 * Extracts text. 
		 */
		public async Task<List<string>> ReadFileUrl(ComputerVisionClient client, string urlFile)
		{
			Console.WriteLine("----------------------------------------------------------");
			Console.WriteLine("READ FILE FROM URL");
			Console.WriteLine();

			// Read text from URL
			var textHeaders = await client.ReadAsync(urlFile, language: "en");
			// After the request, get the operation location (operation ID)
			string operationLocation = textHeaders.OperationLocation;
			Thread.Sleep(2000);

			// Retrieve the URI where the extracted text will be stored from the Operation-Location header.
			// We only need the ID and not the full URL
			const int numberOfCharsInOperationId = 36;
			string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

			// Extract the text
			ReadOperationResult results;
			Console.WriteLine($"Extracting text from URL file {Path.GetFileName(urlFile)}...");
			Console.WriteLine();
			do
			{
				results = await client.GetReadResultAsync(Guid.Parse(operationId));
			}
			while ((results.Status == OperationStatusCodes.Running ||
				results.Status == OperationStatusCodes.NotStarted));

			// Display the found text.
			Console.WriteLine();
			
			var textUrlFileResults = results.AnalyzeResult.ReadResults;
			List<string> txtOutput = new List<string>(); ;
			foreach (ReadResult page in textUrlFileResults)
			{
				foreach (Line line in page.Lines)
				{
					Console.WriteLine(line.Text);
					txtOutput.Add(line.Text);
				}
			}
			Console.WriteLine();
			return txtOutput; 
		}
	}
}
